using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Win32.SafeHandles;

var app = new WorkboxApp();
return app.Run(args);

internal sealed class WorkboxApp
{
    private readonly WorkboxStore _store = new();

    public int Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
        {
            PrintHelp();
            return 0;
        }

        return args[0] switch
        {
            "run" => RunBox(args[1..]),
            "list" => ListBoxes(),
            "inspect" => InspectBox(args[1..]),
            "doctor" => Doctor(args[1..]),
            "stop" => StopBox(args[1..]),
            "prune" => PruneBoxes(),
            _ => Fail($"unknown command: {args[0]}")
        };
    }

    private int RunBox(string[] args)
    {
        var name = "";
        int? timeoutSeconds = null;
        var commandStart = Array.IndexOf(args, "--");

        if (commandStart < 0 || commandStart == args.Length - 1)
        {
            return Fail("usage: workbox run --name <name> [--timeout-seconds <seconds>] -- <command> [args...]");
        }

        for (var i = 0; i < commandStart; i++)
        {
            switch (args[i])
            {
                case "--name" when i + 1 < commandStart:
                    name = args[++i];
                    break;
                case "--timeout-seconds" when i + 1 < commandStart && int.TryParse(args[++i], out var seconds):
                    timeoutSeconds = seconds;
                    break;
                default:
                    return Fail($"unknown run option: {args[i]}");
            }
        }

        if (!WorkboxName.TryCreate(name, out var safeName, out var error))
        {
            return Fail(error);
        }

        var command = args[(commandStart + 1)..];
        var jobName = $@"Local\AiWorkbox_{safeName}_{Guid.NewGuid():N}";
        using var job = NativeJob.Create(jobName);
        job.EnableKillOnClose();

        var processStart = new ProcessStartInfo
        {
            FileName = command[0],
            UseShellExecute = false
        };

        foreach (var arg in command[1..])
        {
            processStart.ArgumentList.Add(arg);
        }

        using var process = new Process { StartInfo = processStart, EnableRaisingEvents = true };
        var startedAtUtc = DateTimeOffset.UtcNow;

        try
        {
            process.Start();
            job.Assign(process);
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            return Fail($"failed to start command: {ex.Message}");
        }

        var record = new WorkboxRecord(
            safeName,
            jobName,
            process.Id,
            string.Join(" ", command.Select(CommandLine.QuoteForDisplay)),
            startedAtUtc,
            null,
            "running");
        _store.Save(record);

        Console.WriteLine($"started box '{safeName}'");
        Console.WriteLine($"  pid: {process.Id}");
        Console.WriteLine($"  job: {jobName}");
        Console.WriteLine($"  command: {record.CommandLine}");

        var exited = timeoutSeconds is null
            ? WaitUntilExit(process)
            : process.WaitForExit(timeoutSeconds.Value * 1000);
        var timedOut = !exited;

        if (timedOut)
        {
            Console.WriteLine($"timeout reached; stopping box '{safeName}'");
            job.Terminate(124);
            process.WaitForExit();
        }

        var exitCode = timedOut ? 124 : process.ExitCode;
        _store.Save(record with
        {
            FinishedAtUtc = DateTimeOffset.UtcNow,
            State = exitCode == 0 ? "exited" : $"exited:{exitCode}"
        });

        return exitCode;
    }

    private static bool WaitUntilExit(Process process)
    {
        process.WaitForExit();
        return true;
    }

    private int ListBoxes()
    {
        var records = _store.LoadAll().OrderByDescending(r => r.StartedAtUtc).ToArray();
        if (records.Length == 0)
        {
            Console.WriteLine("no boxes");
            return 0;
        }

        foreach (var record in records)
        {
            var alive = ProcessLookup.IsAlive(record.RootProcessId);
            var state = record.State == "running" && !alive ? "lost" : record.State;
            Console.WriteLine($"{record.Name}\t{state}\tpid={record.RootProcessId}\tstarted={record.StartedAtUtc.LocalDateTime:yyyy-MM-dd HH:mm:ss}\t{record.CommandLine}");
        }

        return 0;
    }

    private int InspectBox(string[] args)
    {
        if (args.Length != 1)
        {
            return Fail("usage: workbox inspect <name>");
        }

        if (!WorkboxName.TryCreate(args[0], out var safeName, out var error))
        {
            return Fail(error);
        }

        var record = _store.Load(safeName);
        if (record is null)
        {
            return Fail($"box not found: {safeName}");
        }

        var allProcesses = ProcessSnapshot.Capture();
        var processTree = allProcesses.TreeFrom(record.RootProcessId).ToArray();
        var tcpPorts = TcpTable.GetListenersByPid();

        Console.WriteLine($"box: {record.Name}");
        Console.WriteLine($"state: {record.State}");
        Console.WriteLine($"root_pid: {record.RootProcessId}");
        Console.WriteLine($"started: {record.StartedAtUtc.LocalDateTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"command: {record.CommandLine}");

        if (processTree.Length == 0)
        {
            Console.WriteLine("processes: none alive");
            return 0;
        }

        Console.WriteLine("processes:");
        foreach (var entry in processTree)
        {
            var indent = new string(' ', entry.Depth * 2);
            var ports = tcpPorts.TryGetValue(entry.Process.Id, out var values)
                ? $" ports={string.Join(",", values.Order())}"
                : "";
            Console.WriteLine($"{indent}- pid={entry.Process.Id} ppid={entry.Process.ParentProcessId} name={entry.Process.Name}{ports}");
        }

        return 0;
    }

    private int Doctor(string[] args)
    {
        if (args.Length == 0)
        {
            return Fail("usage: workbox doctor nextjs [--path <project-dir>]");
        }

        return args[0] switch
        {
            "nextjs" => DoctorNextJs(args[1..]),
            _ => Fail($"unknown doctor target: {args[0]}")
        };
    }

    private int DoctorNextJs(string[] args)
    {
        var projectPath = Environment.CurrentDirectory;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--path" when i + 1 < args.Length:
                    projectPath = args[++i];
                    break;
                default:
                    return Fail($"unknown nextjs doctor option: {args[i]}");
            }
        }

        projectPath = Path.GetFullPath(projectPath);
        var devDir = Path.Combine(projectPath, ".next", "dev");
        var lockPath = Path.Combine(devDir, "lock");
        var logPath = Path.Combine(devDir, "logs", "next-development.log");
        var candidates = WindowsProcessCommandLines.FindNodeProcessesContaining(projectPath).ToArray();
        var tcpPorts = TcpTable.GetListenersByPid();

        Console.WriteLine("doctor: nextjs");
        Console.WriteLine($"project: {projectPath}");
        Console.WriteLine($"dev_dir: {devDir}");
        Console.WriteLine(File.Exists(lockPath)
            ? $"lock: present ({DescribeLock(lockPath)})"
            : "lock: missing");
        Console.WriteLine(File.Exists(logPath)
            ? $"log: present ({logPath})"
            : "log: missing");

        if (candidates.Length == 0)
        {
            Console.WriteLine("node_processes: none matching project path");
            return 0;
        }

        Console.WriteLine("node_processes:");
        foreach (var candidate in candidates.OrderBy(candidate => candidate.ProcessId))
        {
            var ports = tcpPorts.TryGetValue(candidate.ProcessId, out var values)
                ? $" ports={string.Join(",", values.Order())}"
                : "";
            Console.WriteLine($"- pid={candidate.ProcessId}{ports}");
            Console.WriteLine($"  exe={candidate.ExecutablePath}");
            Console.WriteLine($"  command={candidate.CommandLine}");
        }

        return 0;
    }

    private static string DescribeLock(string lockPath)
    {
        try
        {
            using var stream = new FileStream(lockPath, FileMode.Open, FileAccess.Read, FileShare.None);
            return $"readable length={stream.Length}";
        }
        catch (IOException)
        {
            return "locked by another process";
        }
        catch (UnauthorizedAccessException)
        {
            return "not readable";
        }
    }

    private int StopBox(string[] args)
    {
        if (args.Length != 1)
        {
            return Fail("usage: workbox stop <name>");
        }

        if (!WorkboxName.TryCreate(args[0], out var safeName, out var error))
        {
            return Fail(error);
        }

        var record = _store.Load(safeName);
        if (record is null)
        {
            return Fail($"box not found: {safeName}");
        }

        using var job = NativeJob.Open(record.JobName);
        if (job is null)
        {
            _store.Save(record with
            {
                FinishedAtUtc = DateTimeOffset.UtcNow,
                State = "missing"
            });
            return Fail($"box exists in state file, but its Windows job is no longer open: {safeName}");
        }

        job.Terminate(130);
        _store.Save(record with
        {
            FinishedAtUtc = DateTimeOffset.UtcNow,
            State = "stopped"
        });
        Console.WriteLine($"stopped box '{safeName}'");
        return 0;
    }

    private int PruneBoxes()
    {
        var removed = 0;
        foreach (var record in _store.LoadAll())
        {
            var alive = ProcessLookup.IsAlive(record.RootProcessId);
            if (record.State == "running" && alive)
            {
                continue;
            }

            _store.Delete(record.Name);
            removed++;
        }

        Console.WriteLine($"pruned {removed} inactive box record(s)");
        return 0;
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine($"workbox: {message}");
        return 1;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""
        AI Workbox v0

        Usage:
          workbox run --name <name> [--timeout-seconds <seconds>] -- <command> [args...]
          workbox list
          workbox inspect <name>
          workbox doctor nextjs [--path <project-dir>]
          workbox stop <name>
          workbox prune

        v0 boundary:
          This is not a security sandbox. It groups processes with a Windows Job Object
          so they can be stopped together. It does not isolate files, network, secrets,
          or credentials.
        """);
    }
}

internal static class WorkboxName
{
    public static bool TryCreate(string value, out string safeName, out string error)
    {
        safeName = "";
        error = "";

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "box name is required";
            return false;
        }

        if (value.Length > 48)
        {
            error = "box name must be 48 characters or fewer";
            return false;
        }

        if (value.Any(ch => !(char.IsAsciiLetterOrDigit(ch) || ch is '-' or '_')))
        {
            error = "box name may contain only letters, numbers, '-' and '_'";
            return false;
        }

        safeName = value;
        return true;
    }
}

internal sealed class WorkboxStore
{
    private readonly string _dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AiWorkbox",
        "boxes");

    public WorkboxStore()
    {
        Directory.CreateDirectory(_dir);
    }

    public void Save(WorkboxRecord record)
    {
        var path = Path.Combine(_dir, $"{record.Name}.json");
        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public WorkboxRecord? Load(string name)
    {
        var path = Path.Combine(_dir, $"{name}.json");
        if (!File.Exists(path))
        {
            return null;
        }

        return JsonSerializer.Deserialize<WorkboxRecord>(File.ReadAllText(path));
    }

    public IEnumerable<WorkboxRecord> LoadAll()
    {
        foreach (var path in Directory.EnumerateFiles(_dir, "*.json"))
        {
            WorkboxRecord? record = null;
            try
            {
                record = JsonSerializer.Deserialize<WorkboxRecord>(File.ReadAllText(path));
            }
            catch (JsonException)
            {
                // Ignore a corrupt state file in v0; the tool should still list other boxes.
            }

            if (record is not null)
            {
                yield return record;
            }
        }
    }

    public void Delete(string name)
    {
        var path = Path.Combine(_dir, $"{name}.json");
        File.Delete(path);
    }
}

internal sealed record WorkboxRecord(
    string Name,
    string JobName,
    int RootProcessId,
    string CommandLine,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? FinishedAtUtc,
    string State);

internal static class ProcessLookup
{
    public static bool IsAlive(int pid)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            return !process.HasExited;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}

internal sealed record ProcessTreeEntry(ProcessSnapshotEntry Process, int Depth);

internal sealed class ProcessSnapshot
{
    private readonly Dictionary<int, ProcessSnapshotEntry> _byPid;
    private readonly Dictionary<int, List<ProcessSnapshotEntry>> _byParentPid;

    private ProcessSnapshot(IEnumerable<ProcessSnapshotEntry> entries)
    {
        var materialized = entries.ToArray();
        _byPid = materialized.ToDictionary(entry => entry.Id);
        _byParentPid = materialized
            .GroupBy(entry => entry.ParentProcessId)
            .ToDictionary(group => group.Key, group => group.OrderBy(entry => entry.Id).ToList());
    }

    public static ProcessSnapshot Capture()
    {
        return new ProcessSnapshot(NativeProcessSnapshot.Capture());
    }

    public IEnumerable<ProcessTreeEntry> TreeFrom(int rootPid)
    {
        if (!_byPid.TryGetValue(rootPid, out var root))
        {
            yield break;
        }

        foreach (var entry in Walk(root, 0, new HashSet<int>()))
        {
            yield return entry;
        }
    }

    private IEnumerable<ProcessTreeEntry> Walk(ProcessSnapshotEntry current, int depth, HashSet<int> seen)
    {
        if (!seen.Add(current.Id))
        {
            yield break;
        }

        yield return new ProcessTreeEntry(current, depth);

        if (!_byParentPid.TryGetValue(current.Id, out var children))
        {
            yield break;
        }

        foreach (var child in children)
        {
            foreach (var entry in Walk(child, depth + 1, seen))
            {
                yield return entry;
            }
        }
    }
}

internal sealed record ProcessSnapshotEntry(int Id, int ParentProcessId, string Name);

internal static class NativeProcessSnapshot
{
    private const uint Th32CsSnapProcess = 0x00000002;
    private const int InvalidHandleValue = -1;

    public static IEnumerable<ProcessSnapshotEntry> Capture()
    {
        using var snapshot = CreateToolhelp32Snapshot(Th32CsSnapProcess, 0);
        if (snapshot.IsInvalid)
        {
            yield break;
        }

        var entry = new ProcessEntry32 { Size = (uint)Marshal.SizeOf<ProcessEntry32>() };
        if (!Process32FirstW(snapshot, ref entry))
        {
            yield break;
        }

        do
        {
            yield return new ProcessSnapshotEntry(
                (int)entry.ProcessId,
                (int)entry.ParentProcessId,
                entry.ExeFile);
        }
        while (Process32NextW(snapshot, ref entry));
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern SafeSnapshotHandle CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool Process32FirstW(SafeSnapshotHandle hSnapshot, ref ProcessEntry32 lppe);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool Process32NextW(SafeSnapshotHandle hSnapshot, ref ProcessEntry32 lppe);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct ProcessEntry32
    {
        public uint Size;
        public uint Usage;
        public uint ProcessId;
        public IntPtr DefaultHeapId;
        public uint ModuleId;
        public uint Threads;
        public uint ParentProcessId;
        public int PriClassBase;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string ExeFile;
    }

    private sealed class SafeSnapshotHandle : SafeHandle
    {
        public SafeSnapshotHandle()
            : base(new IntPtr(InvalidHandleValue), true)
        {
        }

        public override bool IsInvalid => handle == new IntPtr(InvalidHandleValue);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);
}

internal static class TcpTable
{
    public static Dictionary<int, List<int>> GetListenersByPid()
    {
        var result = new Dictionary<int, List<int>>();
        var startInfo = new ProcessStartInfo
        {
            FileName = "netstat.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("-ano");
        startInfo.ArgumentList.Add("-p");
        startInfo.ArgumentList.Add("tcp");

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return result;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(3000);
            foreach (var line in output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5 || !parts[0].Equals("TCP", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var state = parts[^2];
                if (!state.Equals("LISTENING", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!int.TryParse(parts[^1], out var pid))
                {
                    continue;
                }

                var localAddress = parts[1];
                var portSeparator = localAddress.LastIndexOf(':');
                if (portSeparator < 0 || !int.TryParse(localAddress[(portSeparator + 1)..], out var port))
                {
                    continue;
                }

                if (!result.TryGetValue(pid, out var ports))
                {
                    ports = [];
                    result[pid] = ports;
                }

                ports.Add(port);
            }
        }
        catch (Win32Exception)
        {
            return result;
        }

        return result;
    }
}

internal sealed record WindowsProcessInfo(
    int ProcessId,
    string ExecutablePath,
    string CommandLine);

internal static class WindowsProcessCommandLines
{
    public static IEnumerable<WindowsProcessInfo> FindNodeProcessesContaining(string needle)
    {
        var script = $$"""
        $needle = '{{PowerShellSingleQuotedString.Escape(needle)}}'
        Get-CimInstance Win32_Process -Filter "Name = 'node.exe'" |
          Where-Object { $_.CommandLine -and $_.CommandLine.Contains($needle) } |
          ForEach-Object {
            "$($_.ProcessId)`t$($_.ExecutablePath)`t$($_.CommandLine)"
          }
        """;

        var encoded = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encoded);

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            yield break;
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit(5000);

        foreach (var line in output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('\t', 3);
            if (parts.Length != 3 || !int.TryParse(parts[0], out var pid))
            {
                continue;
            }

            yield return new WindowsProcessInfo(pid, parts[1], parts[2]);
        }
    }
}

internal static class PowerShellSingleQuotedString
{
    public static string Escape(string value)
    {
        return value.Replace("'", "''");
    }
}

internal static class CommandLine
{
    public static string QuoteForDisplay(string value)
    {
        return value.Any(char.IsWhiteSpace) ? $"\"{value.Replace("\"", "\\\"")}\"" : value;
    }
}

internal sealed class NativeJob : IDisposable
{
    private const uint JobObjectInfoClassExtendedLimitInformation = 9;
    private const uint JobObjectLimitKillOnJobClose = 0x00002000;
    private const uint JobObjectTerminate = 0x0008;
    private const uint JobObjectAssignProcess = 0x0001;
    private readonly SafeFileHandle _handle;

    private NativeJob(SafeFileHandle handle)
    {
        _handle = handle;
    }

    public static NativeJob Create(string name)
    {
        var handle = CreateJobObjectW(IntPtr.Zero, name);
        if (handle.IsInvalid)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return new NativeJob(handle);
    }

    public static NativeJob? Open(string name)
    {
        var handle = OpenJobObjectW(JobObjectTerminate, false, name);
        if (handle.IsInvalid)
        {
            return null;
        }

        return new NativeJob(handle);
    }

    public void EnableKillOnClose()
    {
        var info = new JobObjectExtendedLimitInformation
        {
            BasicLimitInformation = new JobObjectBasicLimitInformation
            {
                LimitFlags = JobObjectLimitKillOnJobClose
            }
        };

        var length = Marshal.SizeOf<JobObjectExtendedLimitInformation>();
        var pointer = Marshal.AllocHGlobal(length);
        try
        {
            Marshal.StructureToPtr(info, pointer, false);
            if (!SetInformationJobObject(_handle, JobObjectInfoClassExtendedLimitInformation, pointer, (uint)length))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    public void Assign(Process process)
    {
        if (!AssignProcessToJobObject(_handle, process.Handle))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public void Terminate(uint exitCode)
    {
        if (!TerminateJobObject(_handle, exitCode))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public void Dispose()
    {
        _handle.Dispose();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateJobObjectW(IntPtr lpJobAttributes, string? lpName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle OpenJobObjectW(uint dwDesiredAccess, bool bInheritHandle, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AssignProcessToJobObject(SafeFileHandle hJob, IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateJobObject(SafeFileHandle hJob, uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetInformationJobObject(
        SafeFileHandle hJob,
        uint jobObjectInfoClass,
        IntPtr lpJobObjectInfo,
        uint cbJobObjectInfoLength);

    [StructLayout(LayoutKind.Sequential)]
    private struct IoCounters
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JobObjectBasicLimitInformation
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JobObjectExtendedLimitInformation
    {
        public JobObjectBasicLimitInformation BasicLimitInformation;
        public IoCounters IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }
}
