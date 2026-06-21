# Validation Next Actions JP

AI Workboxの次の仕事は、実装でも証跡集めでもない。

次の仕事は、外部信号を3件だけ取って、金/キャリアの芯として続けるかを判定すること。

## Current State

Public repo:

```text
https://github.com/skjshr/ai-workbox
```

Preview release:

```text
https://github.com/skjshr/ai-workbox/releases/tag/v0.1.0-preview
```

Tester call issue:

```text
https://github.com/skjshr/ai-workbox/issues/1
```

Current evidence:

- CLI implementation exists.
- Windows process lifecycle behavior is locally verified.
- GitHub release exists.
- CI is green.
- Preview Pack exists.
- Release download count is currently `0`.

Therefore:

```text
career proof: enough for v0
commercial proof: missing
```

## Do Not Do Next

Do not:

- add a GUI
- add more commands
- make a sales page
- write more theory docs
- collect more local screenshots
- polish the release because it feels incomplete
- broaden the product into sandbox/security/PC cleanup

These are avoidance tasks until a real tester asks for them.

## The Only Gate

Continue commercial validation if one of these happens:

```text
3 relevant testers respond
1 relevant tester runs it
1 relevant tester asks for the packaged guide/paid preview
```

Otherwise, keep AI Workbox as OSS/career capital and do not build a paid product yet.

## Tester Definition

A relevant tester matches at least two:

- Windows main machine
- Node/npm or Next.js
- Codex/Claude/Cursor/AI coding workflow
- has had stuck `node.exe`, occupied ports, or unclear dev server state
- does not want Docker/VM overhead for every small project

## Send This

Short version:

```text
WindowsでAI coding / Next.js / npm dev serverを使ってる人向けに、AI Workboxという小さいCLIを作ってます。

やることは、コマンドを名前付きの作業箱で起動して、子プロセスとportを見て、必要なら箱ごと止めることです。

セキュリティsandboxではなく、AI開発中に残ったnode.exeやdev serverを見失わないためのプロセス管理ツールです。

READMEだけ見て、次の3つを教えてほしいです。

1. 何の道具か分かるか
2. 自分の環境で使う場面がありそうか
3. 有料の日本語セットアップ/事故復旧ガイドに価値がありそうか

Repo:
https://github.com/skjshr/ai-workbox
```

If they are willing to run it:

```text
Preview release:
https://github.com/skjshr/ai-workbox/releases/tag/v0.1.0-preview

試すなら、まず `workbox --help`、次にNext.js/npm環境で `workbox doctor nextjs` または `workbox run --name web -- npm run dev` だけで大丈夫です。
```

## Record Only This

Record anonymized results in `docs/tester-validation-log.md`.

Do not record real names, handles, company names, private repo names, screenshots, secrets, or local paths.

Use this minimum:

```text
T1 / qualified yes-no / understood yes-no / ran yes-no / paid signal yes-no / main reason
```

## Decision Rule

After 3 testers:

- If 2/3 understand the use case: continue as OSS + possible paid guide.
- If 1 tester runs it: improve install/run friction next.
- If 1 tester gives paid-pack signal: prepare a paid preview.
- If nobody can name when they would run it: stop commercialization.

No shame fallback:

```text
AI Workbox remains a career OSS/tooling asset even if it is not the money core.
```
