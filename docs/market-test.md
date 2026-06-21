# Market Test

This test decides whether AI Workbox deserves commercial work.

## Test Object

Do not sell the CLI itself first.

Test this offer:

```text
AI Workbox Preview Pack

WindowsでCodex/Claude/Next.js/npm dev serverを使う人向けに、プロセスツリーとportを見て、名前付きの作業箱ごと止めるCLIと、日本語の事故復旧レシピをまとめたもの。
```

## Who To Ask

Ask people who match at least two:

- Windows main machine
- Next.js or Node dev server
- Codex, Claude Code, Cursor, or similar AI coding tools
- Has had stuck `node.exe`, occupied ports, or unclear dev servers
- Does not want full Docker/VM setup for every small project

Do not ask generic non-technical friends. Their answer will be noise.

## Message

```text
WindowsでAI coding / Next.js / npm dev serverを使ってる人向けに、プロセスを名前付きの箱で起動して、子プロセスとportを見て、箱ごと止める小さいCLIを作ってます。

セキュリティsandboxではなく、dev serverやAI作業で残ったnode.exe/portを見失わないための道具です。

READMEだけ見て「使いどころが分かるか」「自分の環境で困ったことに近いか」だけ教えてほしいです。
```

## What To Observe

Good signal:

- "This happened to me"
- "Can it show which Node owns port 3000?"
- "Can it work with Next/Cursor/Claude?"
- "I want a zip, not a build-from-source repo"
- "I would pay for a setup guide"

Weak signal:

- "Looks cool"
- "Nice portfolio"
- "You should make a GUI"
- "Is this like Docker?"

Bad signal:

- They expect malware containment.
- They expect company endpoint policy bypass.
- They only want generic PC cleanup.
- They cannot tell when they would run it.

## Pass Gate

Within 30 days, continue commercializing only if one is true:

- 3 relevant people understand the use case from README.
- 1 relevant person runs the tool.
- 1 relevant person says the paid Japanese setup pack would be useful.
- A hiring/interview conversation reacts positively to the technical story.

If none happens, keep AI Workbox as a career OSS/tooling project and do not build a paid package yet.
