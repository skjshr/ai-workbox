# AI Workbox

AI Workbox は、AI coding tool や dev server を Windows 上の名前付き workbox として起動、確認、停止する小さな CLI です。

やることはかなり絞っています。

```powershell
workbox run --name web -- cmd /c "npm run dev"
workbox list
workbox inspect web
workbox stop web
```

名前を付けて起動し、プロセスツリーを見て、まとめて止める。まずはそこだけです。

セキュリティ sandbox ではありません。

## なぜ作ったか

AI coding を回していると、dev server、Node process、子プロセス、謎の port が残りがちです。

AI Workbox は、それらを「どの作業で起動したものか」分かる形に寄せます。壊れた状態を魔法のように直すツールではないです。散らかったプロセスを、少し見やすくして、止めやすくする道具です。

## Preview Pack を試す

まず試すなら、self-contained win-x64 の Preview Pack が楽です。

```text
https://github.com/skjshr/ai-workbox/releases/tag/v0.1.0-preview
```

zip を展開して、これを実行します。

```powershell
.\try-smoke.ps1
```

中ではだいたいこれを確認します。

- `bin\workbox.exe --help`
- 短い smoke 用 workbox の起動
- `list`
- `prune`

Preview Pack は .NET Runtime の追加インストールなしで動くようにしています。

Node や Next.js の project で試すなら、こんな感じです。

```powershell
<preview-pack>\bin\workbox.exe run --name web -- cmd /c "npm run dev"
<preview-pack>\bin\workbox.exe inspect web
<preview-pack>\bin\workbox.exe stop web
```

既に動いている Next.js dev server の状態だけ見ることもできます。

```powershell
<preview-pack>\bin\workbox.exe doctor nextjs --path C:\dev\example-next-app
```

tray monitor もあります。

```powershell
<preview-pack>\bin\workbox-tray.exe
```

tray app は、CLI で起動した名前付き workbox を見ます。box 一覧、process tree、停止、inactive record の掃除ができます。関係ない process を勝手に探して kill するものではありません。

フィードバックは [issue #1](https://github.com/skjshr/ai-workbox/issues/1) に置いています。

## できること

v0 でやることです。

- root process と child process を Windows Job Object にまとめる
- root process、child process tree、listening TCP port を見る
- workbox をまとめて止める
- timeout で止める
- `%LOCALAPPDATA%\AiWorkbox\boxes` に小さい状態ファイルを書く

## やらないこと

v0 ではここまでやりません。

- file isolation
- network isolation
- secret や credential の保護
- user が読める file を process から隠すこと
- セキュリティ保証
- file 削除
- registry 変更
- admin 権限の要求

## source から build

```powershell
dotnet build .\ai-workbox.sln
```

## local Preview Pack を作る

```powershell
.\scripts\package-preview.ps1 -Version "0.1.0-local"
```

## source から実行

```powershell
dotnet run --project .\src\AiWorkbox.Cli -- list
dotnet run --project .\src\AiWorkbox.Cli -- run --name smoke --timeout-seconds 5 -- powershell.exe -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
```

## Commands

```powershell
workbox run --name <name> [--timeout-seconds <seconds>] -- <command> [args...]
workbox list
workbox inspect <name>
workbox doctor nextjs [--path <project-dir>]
workbox stop <name>
workbox prune
```

box name に使えるのは、英数字、`-`、`_` です。

`prune` は inactive な Workbox state record だけを消します。project file を消したり、process を止めたりはしません。

`inspect` の例です。

```text
box: inspect_probe
state: running
root_pid: 108288
command: cmd /c npm run dev
processes:
- pid=108288 ppid=106676 name=cmd.exe
  - pid=108316 ppid=108288 name=node.exe
    - pid=64740 ppid=108316 name=cmd.exe
      - pid=106144 ppid=64740 name=node.exe ports=43124
        - pid=103612 ppid=106144 name=node.exe
```

`doctor nextjs` の例です。

```text
doctor: nextjs
project: C:\dev\example-next-app
lock: present (locked by another process)
log: present (...\.next\dev\logs\next-development.log)
node_processes:
- pid=12345 ports=3000,52263,52264,52273,52275
  exe=C:\Program Files\nodejs\node.exe
  command="C:\Program Files\nodejs\node.exe" ...\next\dist\server\lib\start-server.js
```

## 置いているもの

- [検証ログ](docs/verification.md)
- [Next.js recipe](docs/nextjs-recipe.md)
- [Preview guide JP](docs/preview-guide-jp.md)
- [Feedback form JP](docs/feedback-form-jp.md)
- [Safety boundaries](docs/safety-boundaries.md)
- [Security policy](SECURITY.md)
- [Release checklist](docs/release-checklist.md)
- [v0.1.0 preview release notes](docs/release-notes-v0.1.0-preview.md)
- [Tray use cases](docs/tray-use-cases.md)
- [Tray test plan](docs/tray-test-plan.md)
