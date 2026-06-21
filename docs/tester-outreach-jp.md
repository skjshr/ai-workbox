# Tester Outreach JP

AI Workboxの外部信号を取るための送信用文面。

目的は拡散ではなく、関連テスター3人から反応を取ること。

## Send Order

1. Windows + Node/Next.js + AI codingに近い既知の技術者へ送る。
2. 反応があった人だけPreview Pack実行を頼む。
3. 3人分の反応が集まったら止めて、`docs/tester-validation-log.md` に記録する。

送らない相手:

- Windowsを使っていない
- Node/npm/Next.jsを使っていない
- AI codingやローカルdev serverの痛みがない
- セキュリティsandboxとして期待しそう

## DM 1: READMEだけ見てもらう

```text
WindowsでAI coding / Next.js / npm dev serverを使ってる人向けに、AI Workboxという小さいCLIを作ってます。

やることはシンプルで、コマンドを名前付きの作業箱で起動して、子プロセスとportを見て、必要なら箱ごと止める道具です。

セキュリティsandboxではなく、AI開発中に残ったnode.exeやdev serverを見失わないためのプロセス管理ツールです。

READMEだけ見て、
1. 何の道具か分かるか
2. 自分の環境で使う場面がありそうか
3. 有料の日本語セットアップ/事故復旧ガイドに価値がありそうか
を教えてほしいです。

Repo:
https://github.com/skjshr/ai-workbox
```

## DM 2: 実行してもらえそうな人向け

```text
もし実行まで見てもらえるなら、Preview Packはwin-x64自己完結zipにしてあります。
.NET Runtimeの事前インストールは不要です。

Release:
https://github.com/skjshr/ai-workbox/releases/tag/v0.1.0-preview

まずzipを展開して、PowerShellでこれだけお願いします。

.\try-smoke.ps1

その後、Next.js/npm環境があれば、以下のどちらかだけ見てもらえると助かります。

<preview-pack>\bin\workbox.exe doctor nextjs --path <project-dir>

または

<preview-pack>\bin\workbox.exe run --name web -- cmd /c "npm run dev"
<preview-pack>\bin\workbox.exe inspect web
<preview-pack>\bin\workbox.exe stop web

見たいのは、便利かどうかより先に、
1. 何をしている道具か誤解なく分かるか
2. process/portの見え方が役に立つか
3. 自分ならいつ使うか
です。
```

## DM 3: 英語の公開スレッド向け

古いスレッドや解決済みissueに貼らない。

相手の問題をAI Workboxが解決すると断言しない。

```text
I am testing a small Windows CLI for one narrow local-dev problem: running npm/Next.js commands in a named process group, then inspecting child processes and listening ports before stopping that group.

It is not a security sandbox and it will not fix app bugs, but it may help with orphaned dev servers or unclear node.exe/port state.

If this matches your pain, I am looking for feedback here:
https://github.com/skjshr/ai-workbox/issues/1
```

## 見てほしい順番

1. README
2. `.\try-smoke.ps1`
3. `docs/preview-guide-jp.md`
4. `docs/feedback-form-jp.md`

## 聞かないこと

- 「かっこいい？」
- 「売れそう？」
- 「GUIいる？」

## 聞くこと

- いつ使うか
- どの出力が分かりにくいか
- 何なら金を払うか
- セキュリティsandboxと誤解しないか

## 記録する最小情報

`docs/tester-validation-log.md` に匿名IDだけで書く。

```text
T1 / qualified yes-no / understood yes-no / ran yes-no / paid signal yes-no / main reason
```

実名、handle、メールアドレス、会社名、private repo名、スクショ、ローカルパスは書かない。

## 3人で止める

3人から反応が来たら、そこで一旦止める。

続行条件:

- 2/3が用途を理解した
- 1人が実行した
- 1人が有料Preview Pack/日本語ガイドに具体的興味を示した

どれも満たさないなら、AI Workboxは商用化せず、career OSS/toolingとして扱う。
