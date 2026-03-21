---
description: Commit：取证 → 拟稿 → 你确认后执行 git add/commit
---

在仓库根目录**实际跑 git**，并遵守 `.cursor/skills/git-commit-dev-analysis/SKILL.md`（含 **经确认后的执行** 一节）。

## 流程（必须按序）

1. **只读取证**：`git status`、`git diff`；若已有暂存则再看 `git diff --cached`。
2. **拟稿**：根据真实 diff 写出建议的 **完整 commit message**（subject；需要则 body），并列出建议 **`git add` 的路径**（或说明用 `git add -p` 由你本地交互处理）。
3. **停顿等你确认**：用一小段话汇总「将暂存的文件 / 将使用的 message」，**明确询问是否执行**。未收到你的明确同意（如「确认」「执行」「可以」）前，**禁止**执行 `git add`、`git commit`、`git push`。
4. **你同意后再执行**（逐条执行并展示输出）：
   - `git add <已确认的路径>`（或你指定的命令）
   - 提交优先用多行正文时：`git commit -F <临时文件>`，避免 shell 转义问题；单行可用 `git commit -m "..."`。
5. **收尾**：`git log -1 --stat` 或 `--oneline` 给你看结果。
6. **推送**：仅当你**再次明确确认**后执行 `git push`（或 `git push -u origin <branch>`）。禁止 `push --force` / `--force-with-lease`，除非你明确要求。

若工作区干净或无可提交内容，说明情况并停止，不要硬提交。
