# Commercial Validation

AI Workbox should earn the right to become a paid product. Do not sell it just because the CLI builds.

## Target Buyer

Primary:

- Windows-based individual developers using Codex, Claude Code, Cursor, Next.js, Node.js, or local AI coding workflows.
- People who have multiple dev servers, stuck Node processes, ports, and tool sessions they no longer trust.

Secondary:

- Small teams adopting AI coding tools without mature local process hygiene.
- Engineers who want a lightweight local guardrail before moving to containers, VMs, or enterprise sandboxes.

## Pain

The paid pain is not "I want a template".

The paid pain is:

- "I do not know what is still running."
- "I do not know which Node owns this port."
- "AI tools started a process and I lost track."
- "I want to run a dev server in a named box and stop the whole thing."
- "I want a simple Windows-native tool before I learn heavier sandboxing."

## Free vs Paid

Free:

- Source code
- Core CLI
- Basic README
- Safety boundaries
- Basic Next.js recipe

Paid later:

- Signed or packaged Windows zip
- Japanese setup guide
- Codex/Claude/Next.js recipes
- Troubleshooting decision tree
- "AI dev environment recovery" checklist
- Profile examples for common projects

Do not put safety-critical behavior behind a paywall.

## First Offer Draft

Title:

```text
AI Workbox: Windows向けAI開発プロセス作業箱
```

Short description:

```text
Codex/Claude/Next.js/npm dev serverを名前付きの作業箱で起動し、プロセスツリーとportを見て、必要なら箱ごと止めるWindows CLIです。v0はセキュリティsandboxではなく、AI開発環境のプロセスライフサイクル管理に絞っています。
```

Price test:

- 0円: GitHub core
- 1,480円: packaged zip + Japanese recipes + recovery checklist
- 4,980円: later pro package only if profiles/logging become useful

## 30-Day Test

### Week 1

- Move from local output folder to a real project repo.
- Keep MIT license and security boundary.
- Record a short GIF or terminal log showing `run`, `inspect`, `doctor nextjs`, `stop`.

Pass:

- User can use it once on a real local project without breaking work.

### Week 2

- Publish free repo or private preview link.
- Ask 3 developers who use Windows + Node/Next.js/AI tools to try the README.

Pass:

- At least 1 person understands the value without a long explanation.
- At least 1 concrete bug or workflow request appears.

### Week 3

- Add only the top workflow request.
- Prepare Japanese paid guide draft.

Pass:

- The tool is still about process lifecycle, not generic cleanup.

### Week 4

- Decide paid test:
  - BOOTH/Ko-fi zip if external interest exists.
  - Keep as career OSS if no buyer signal exists.

Pass:

- One of these is true:
  - 1 paid purchase
  - 3 serious testers
  - a clear portfolio/interview story emerges

## Kill Conditions

Stop commercializing if:

- People only see it as a generic task manager.
- It requires too much support per user.
- Users expect security guarantees.
- The core cannot outperform "manual taskkill + netstat" enough to matter.
- It becomes a docs/portfolio project instead of a tool the user actually runs.
