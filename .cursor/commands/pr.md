---
description: PR：取证 → 拟分支/标题正文 → 你确认后 push 与发起 PR
---

在仓库根目录**实际跑 git**（可选 **GitHub `gh` / GitLab `glab`**），并遵守 `.cursor/skills/git-commit-dev-analysis/SKILL.md`（含 **经确认后的执行** 一节）。

## 流程（必须按序）

1. **只读取证**：`git fetch origin`；确认 **BASE**（如 `main` / `develop`，对应 `origin/BASE`）。用 `git status`、`git branch -vv` 看清当前分支与是否有未提交改动。
2. **分支**：
   - 若你已在合适的功能分支上且改动已提交：跳过创建分支。
   - 若需要从 BASE **新建分支**：提议分支名（如 `feat/xxx`），**等你明确同意**后再执行 `git switch BASE`（或 `git checkout BASE`）、`git pull`（如需要）、`git switch -c <分支名>`。
   - 若有未提交改动：先问你是要 **先 `/commit`** 还是 **stash**；不要擅自丢弃改动。
3. **PR 文案**：用 `git log origin/BASE..HEAD --oneline`、`git diff origin/BASE...HEAD --stat`（必要时片段 diff）写 **PR 标题 + 正文**（可对齐仓库 PR 模板）。
4. **停顿等你确认**：展示将执行的 **`git push`** 形式（含 `-u` 与远程、分支名）以及 **`gh pr create ...` 或 `glab mr create ...` 的完整参数**（base、head、title、body）。未收到你的明确同意前，**禁止** `push` 与创建 PR/MR。
5. **你同意后再执行**：
   - 先 `git push -u origin HEAD`（或你确认的分支名）
   - 再 `gh pr create` / `glab mr create`（若未安装 CLI 或你拒绝 CLI，则给出浏览器创建 PR 的链接与粘贴用标题/正文）
6. **收尾**：输出 PR/MR 链接或 `gh pr view --web` 等结果。

禁止在未确认时 `push --force` / `--force-with-lease`，除非你明确要求。
