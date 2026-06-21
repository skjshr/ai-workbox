# AI Workbox Preview Guide

対象: WindowsでCodex/Claude/Next.js/npm dev serverを使う個人開発者

## これは何か

AI Workboxは、AI codingやdev server用のコマンドを **名前付きの作業箱** として起動し、あとから中身を見て、必要なら箱ごと止めるWindows CLIです。

```powershell
workbox run --name web -- cmd /c "npm run dev"
workbox inspect web
workbox stop web
```

## これは何ではないか

v0はセキュリティsandboxではありません。

しないこと:

- ファイルアクセスの隔離
- ネットワーク遮断
- 秘密情報の保護
- マルウェア封じ込め
- 会社PCの管理ポリシー回避
- PC高速化保証

対象は **プロセスのライフサイクル管理** です。

## 必要なもの

- Windows 10/11
- PowerShell
- Node/npmを使う場合はNode.js

Preview Pack版は自己完結zipなので、.NET Runtimeの事前インストールは不要です。

## 最初の確認

zipを展開したフォルダで:

```powershell
.\bin\workbox.exe --help
```

期待:

```text
AI Workbox v0
Usage:
  workbox run ...
  workbox inspect ...
  workbox doctor nextjs ...
```

## 安全なsmoke test

```powershell
.\try-smoke.ps1
```

この段階ではプロジェクトファイルを触りません。

## タスクトレイで見る

zipを展開したフォルダで:

```powershell
.\bin\workbox-tray.exe
```

tray版でできること:

- named workboxの一覧を見る
- running workboxを右クリックメニューから止める
- status画面で `inspect` を見る
- inactive recordを `Prune` する

tray版でしないこと:

- 勝手にprocessをkillする
- unrelatedな `node.exe` を探して消す
- ファイルを削除する
- セキュリティsandboxとして隔離する

## npm dev serverを箱で起動する

プロジェクトフォルダへ移動して:

```powershell
<preview-pack>\bin\workbox.exe run --name web -- cmd /c "npm run dev"
```

別ターミナルから:

```powershell
<preview-pack>\bin\workbox.exe list
<preview-pack>\bin\workbox.exe inspect web
```

止める:

```powershell
<preview-pack>\bin\workbox.exe stop web
```

## Next.jsが既に動いているか見る

```powershell
<preview-pack>\bin\workbox.exe doctor nextjs --path C:\dev\example-next-app
```

見るもの:

- `.next/dev/lock` があるか
- Next.js logがあるか
- 関連するnode.exe
- port 3000などを掴んでいるPID

既存プロセスは勝手に止めません。

## 困った時

### `box not found`

指定した名前のWorkbox stateがありません。

```powershell
<preview-pack>\bin\workbox.exe list
```

で名前を確認してください。

### `box exists in state file, but its Windows job is no longer open`

Workboxの記録は残っていますが、Windows Job Objectはもうありません。

```powershell
<preview-pack>\bin\workbox.exe prune
```

で古い記録を消せます。プロジェクトファイルは消しません。

### Next.jsが `Another next dev server is already running` と言う

まず:

```powershell
<preview-pack>\bin\workbox.exe doctor nextjs --path <project-dir>
```

で既存PID/portを確認します。いきなりkillしないでください。

## フィードバックで欲しいこと

欲しい:

- どのコマンドで困ったか
- `inspect` の出力が分かりやすいか
- 自分の環境で本当に使いどころがあるか
- 有料日本語ガイド/zipに価値を感じるか

不要:

- 「なんとなく良い」
- セキュリティsandboxとしての期待
- GUI化だけの要望

## 使ってはいけない場面

- 会社管理端末で許可なく実行する
- 機密プロジェクトで動作ログを外部共有する
- 未信頼コードを安全に閉じ込める目的で使う
- ファイル削除や秘密情報保護を期待する
