# AI Workbox

AI Workbox は、Codex や Claude Code まわりで増えがちな dev server と子プロセスを、名前付きの箱で起動して止めるための Windows CLI です。

やることは地味です。

```powershell
workbox run --name web -- cmd /c "npm run dev"
workbox list
workbox inspect web
workbox stop web
```

起動する。見る。止める。

まずはそれだけです。

セキュリティ sandbox ではありません。

## 何に困っていたか

AI に実装を投げていると、`npm run dev`、Next.js、Node の子プロセス、空いているのか分からない port が残りがちです。

ターミナルを閉じても残る。

別の agent がまた dev server を立てる。

気づいたら、どれを止めていいのか分からない。

AI Workbox は、その混乱を少しだけ扱いやすくする道具です。プロセスに名前を付けて、後から見て、まとめて止めます。

## Preview Pack

試すなら release の Preview Pack が早いです。

```text
https://github.com/skjshr/ai-workbox/releases/tag/v0.1.0-preview
```

zip を展開して、まずこれを実行します。

```powershell
.\try-smoke.ps1
```

この script は、help 表示、短い workbox の起動、`list`、`prune` までを確認します。

Preview Pack は self-contained win-x64 です。.NET Runtime を別で入れなくても動く想定です。

## 使い方

Node や Next.js の project で試すならこうです。

```powershell
<preview-pack>\bin\workbox.exe run --name web -- cmd /c "npm run dev"
<preview-pack>\bin\workbox.exe inspect web
<preview-pack>\bin\workbox.exe stop web
```

すでに動いている Next.js dev server を見るだけなら、`doctor` を使います。

```powershell
<preview-pack>\bin\workbox.exe doctor nextjs --path C:\dev\example-next-app
```

tray monitor も入っています。

```powershell
<preview-pack>\bin\workbox-tray.exe
```

tray app は、CLI で起動した workbox を見ます。関係ない process を勝手に探して kill するものではありません。

フィードバックは [issue #1](https://github.com/skjshr/ai-workbox/issues/1) にください。

## できること

- root process と child process を Windows Job Object にまとめる
- process tree と listening TCP port を見る
- workbox をまとめて止める
- timeout で止める
- `%LOCALAPPDATA%\AiWorkbox\boxes` に小さい状態ファイルを書く

## やらないこと

- file isolation
- network isolation
- secret や credential の保護
- user が読める file を process から隠すこと
- セキュリティ保証
- file 削除
- registry 変更
- admin 権限の要求

つまり、悪い code を閉じ込める箱ではありません。散らかった開発プロセスを見つけやすくして、止めやすくする箱です。

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

`prune` は inactive な state record だけを消します。project file を消したり、process を止めたりはしません。

## 出力例

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

## docs

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
