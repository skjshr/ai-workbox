using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace AiWorkbox.Tray;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new TrayContext());
    }
}

internal sealed class TrayContext : ApplicationContext
{
    private readonly WorkboxCli _cli = new();
    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private StatusForm? _statusForm;
    private IReadOnlyList<WorkboxRow> _rows = [];

    public TrayContext()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "AI Workbox",
            Visible = true
        };
        _notifyIcon.DoubleClick += (_, _) => ShowStatus();

        _timer = new System.Windows.Forms.Timer { Interval = 10_000 };
        _timer.Tick += async (_, _) => await RefreshAsync();

        RebuildMenu();
        _ = RefreshAsync();
        _timer.Start();
    }

    private async Task RefreshAsync()
    {
        var result = await _cli.ListAsync();
        if (result.ExitCode == 0)
        {
            _rows = WorkboxListParser.Parse(result.Output).ToArray();
            var running = _rows.Count(row => row.State == "running");
            _notifyIcon.Text = running == 0
                ? "AI Workbox: no running boxes"
                : $"AI Workbox: {running} running";
            _statusForm?.SetRows(_rows);
        }
        else
        {
            _notifyIcon.Text = "AI Workbox: CLI unavailable";
            _statusForm?.SetError(result.Error);
        }

        RebuildMenu();
    }

    private void RebuildMenu()
    {
        var menu = new ContextMenuStrip();
        var running = _rows.Where(row => row.State == "running").Take(8).ToArray();

        menu.Items.Add("Open Status", null, (_, _) => ShowStatus());
        menu.Items.Add("Refresh", null, async (_, _) => await RefreshAsync());

        if (running.Length > 0)
        {
            menu.Items.Add(new ToolStripSeparator());
            foreach (var row in running)
            {
                menu.Items.Add($"Stop {row.Name}", null, async (_, _) => await StopAsync(row.Name));
            }
        }

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Prune Inactive Records", null, async (_, _) => await PruneAsync());
        menu.Items.Add("Exit", null, (_, _) => Exit());

        _notifyIcon.ContextMenuStrip = menu;
    }

    private void ShowStatus()
    {
        if (_statusForm is { IsDisposed: false })
        {
            _statusForm.Activate();
            return;
        }

        _statusForm = new StatusForm(_cli);
        _statusForm.SetRows(_rows);
        _statusForm.RefreshRequested += async (_, _) => await RefreshAsync();
        _statusForm.StopRequested += async (_, name) => await StopAsync(name);
        _statusForm.PruneRequested += async (_, _) => await PruneAsync();
        _statusForm.Show();
    }

    private async Task StopAsync(string name)
    {
        var confirm = MessageBox.Show(
            $"Stop workbox '{name}' and its process tree?",
            "AI Workbox",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        var result = await _cli.StopAsync(name);
        if (result.ExitCode != 0)
        {
            MessageBox.Show(result.Error, "AI Workbox stop failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        await RefreshAsync();
    }

    private async Task PruneAsync()
    {
        var result = await _cli.PruneAsync();
        if (result.ExitCode != 0)
        {
            MessageBox.Show(result.Error, "AI Workbox prune failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        await RefreshAsync();
    }

    private void Exit()
    {
        _timer.Stop();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _statusForm?.Close();
        Application.Exit();
    }
}

internal sealed class StatusForm : Form
{
    private readonly WorkboxCli _cli;
    private readonly ListView _list = new();
    private readonly TextBox _details = new();
    private readonly Button _refresh = new() { Text = "Refresh" };
    private readonly Button _inspect = new() { Text = "Inspect" };
    private readonly Button _stop = new() { Text = "Stop" };
    private readonly Button _prune = new() { Text = "Prune" };

    public event EventHandler? RefreshRequested;
    public event EventHandler<string>? StopRequested;
    public event EventHandler? PruneRequested;

    public StatusForm(WorkboxCli cli)
    {
        _cli = cli;
        Text = "AI Workbox";
        Width = 900;
        Height = 560;
        StartPosition = FormStartPosition.CenterScreen;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(8),
            FlowDirection = FlowDirection.LeftToRight
        };
        buttons.Controls.AddRange([_refresh, _inspect, _stop, _prune]);

        _list.Dock = DockStyle.Top;
        _list.Height = 230;
        _list.View = View.Details;
        _list.FullRowSelect = true;
        _list.MultiSelect = false;
        _list.Columns.Add("Name", 140);
        _list.Columns.Add("State", 90);
        _list.Columns.Add("PID", 80);
        _list.Columns.Add("Started", 150);
        _list.Columns.Add("Command", 430);

        _details.Dock = DockStyle.Fill;
        _details.Multiline = true;
        _details.ScrollBars = ScrollBars.Both;
        _details.ReadOnly = true;
        _details.Font = new Font(FontFamily.GenericMonospace, 9);

        Controls.Add(_details);
        Controls.Add(_list);
        Controls.Add(buttons);

        _refresh.Click += (_, _) => RefreshRequested?.Invoke(this, EventArgs.Empty);
        _inspect.Click += async (_, _) => await InspectSelectedAsync();
        _stop.Click += (_, _) =>
        {
            var name = SelectedName();
            if (name is not null)
            {
                StopRequested?.Invoke(this, name);
            }
        };
        _prune.Click += (_, _) => PruneRequested?.Invoke(this, EventArgs.Empty);
        _list.DoubleClick += async (_, _) => await InspectSelectedAsync();
    }

    public void SetRows(IReadOnlyList<WorkboxRow> rows)
    {
        _list.Items.Clear();
        foreach (var row in rows)
        {
            var item = new ListViewItem(row.Name);
            item.SubItems.Add(row.State);
            item.SubItems.Add(row.ProcessId?.ToString() ?? "");
            item.SubItems.Add(row.Started);
            item.SubItems.Add(row.Command);
            _list.Items.Add(item);
        }

        if (rows.Count == 0)
        {
            _details.Text = "No workboxes. Start one with the CLI, then refresh.";
        }
    }

    public void SetError(string error)
    {
        _details.Text = string.IsNullOrWhiteSpace(error) ? "AI Workbox CLI failed." : error;
    }

    private async Task InspectSelectedAsync()
    {
        var name = SelectedName();
        if (name is null)
        {
            return;
        }

        var result = await _cli.InspectAsync(name);
        _details.Text = result.ExitCode == 0 ? result.Output : result.Error;
    }

    private string? SelectedName()
    {
        return _list.SelectedItems.Count == 0 ? null : _list.SelectedItems[0].Text;
    }
}

internal sealed record WorkboxRow(string Name, string State, int? ProcessId, string Started, string Command);

internal static class WorkboxListParser
{
    public static IEnumerable<WorkboxRow> Parse(string output)
    {
        foreach (var line in output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line == "no boxes")
            {
                yield break;
            }

            var parts = line.Split('\t', 5);
            if (parts.Length < 5)
            {
                continue;
            }

            yield return new WorkboxRow(
                parts[0],
                parts[1],
                ParsePid(parts[2]),
                parts[3].Replace("started=", "", StringComparison.OrdinalIgnoreCase),
                parts[4]);
        }
    }

    private static int? ParsePid(string value)
    {
        return value.StartsWith("pid=", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(value[4..], out var pid)
            ? pid
            : null;
    }
}

internal sealed class WorkboxCli
{
    private readonly string _exePath = WorkboxLocator.Find();

    public Task<CliResult> ListAsync() => RunAsync("list");

    public Task<CliResult> InspectAsync(string name) => RunAsync("inspect", name);

    public Task<CliResult> StopAsync(string name) => RunAsync("stop", name);

    public Task<CliResult> PruneAsync() => RunAsync("prune");

    private async Task<CliResult> RunAsync(params string[] args)
    {
        if (!File.Exists(_exePath))
        {
            return new CliResult(1, "", $"workbox.exe was not found. Expected: {_exePath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _exePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            return new CliResult(1, "", "failed to start workbox.exe");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return new CliResult(process.ExitCode, await outputTask, await errorTask);
    }
}

internal sealed record CliResult(int ExitCode, string Output, string Error);

internal static class WorkboxLocator
{
    public static string Find()
    {
        var baseDir = AppContext.BaseDirectory;
        var sameDir = Path.Combine(baseDir, "workbox.exe");
        if (File.Exists(sameDir))
        {
            return sameDir;
        }

        var siblingBin = Path.Combine(baseDir, "bin", "workbox.exe");
        if (File.Exists(siblingBin))
        {
            return siblingBin;
        }

        var repoDebug = Path.GetFullPath(Path.Combine(
            baseDir,
            "..", "..", "..", "..",
            "AiWorkbox.Cli", "bin", "Debug", "net10.0",
            "workbox.exe"));
        return repoDebug;
    }
}
