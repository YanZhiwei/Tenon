---
name: git-pr-branch
description: >-
  On integration branch: propose PR branch name for user approval, then git switch -c /
  checkout -b (never overwrite existing branch silently). By default continue in the same
  session with git-review-commit (stage, diff, review, commit) and a suggested PR title.
  Use when starting work that merges via pull request, or slash /git-pr-branch.
---

# Git：PR 用分支（创建 / 切换）

在共享集成分支上准备**将通过 PR 合并**的改动时（新功能、**修 bug**、重构等），与用户确认并 **创建或切换到 PR 用分支**。暂存、PR 式 diff、内联 review、确认后 `git commit` 及 **PR 标题建议** 由 **`git-review-commit`** 承担；本技能在默认「一条龙」流程中 **自动衔接** 该技能，**不在此文件复制**其大段规则。

---

## 何时使用

- 当前在 `main` / `dev` 等**集成分支**（以仓库约定为准），即将做**独立改动且后续走 PR**，需要先落 **PR 分支**。  
- 用户触发 **`/git-pr-branch`** 或等价表述：期望 **自动建分支 → 接着走提交前 review**（见下节「默认一条龙」）。  
- 用户**只要**建分支、不要 review：执行本技能步骤 1～4 后结束即可（不强制衔接）。

---

## 流程

1. **看清上下文**：`git status` / `git branch --show-current`（或等价），确认是否在共享集成分支、是否有未提交改动需用户知情。  
2. **确认分支名**：与用户确认 **PR 分支名**。可先根据改动性质 **建议** 一条名称（如 `feat/...`、`fix/...`、`chore/...`）；用户已一次性写明完整名称或回复「确定」采用建议，可视为已确认。**不得**在未获确认时执行 `git switch -c`。  
3. **创建或切换**：  
   - 分支不存在：`git switch -c <branch>` / `git checkout -b <branch>`（或项目惯用等价命令）。  
   - 分支已存在：**不得**静默覆盖；提示 `git switch <existing>` 或改用新名称。  
4. **结束条件**：已处于合适的 PR 分支，或用户声明留在当前分支、无需新建——则跳过创建。  
5. **默认一条龙（须衔接 git-review-commit）**：用户未声明「只要分支」时，在完成步骤 1～4 后 **同一轮会话内自动继续**：从 **`git-review-commit`** 的步骤 **1（查清范围）** 起执行（暂存 → PR 式 diff → 中文 review → 提案含 **PR 标题建议** → 用户确认后 `git commit`）。**不得**仅提示「请自行打开 git-review-commit」即结束。

---

## 安全与边界

- 非经用户明确要求，不执行 `reset --hard`、`push --force` 等；共享分支 force push 须再确认。  
- 不静默删除、覆盖或强行移动已有分支引用。

## 仓库内副本

若项目在 `.cursor` 等路径下另有本技能的副本，修改时须与本文件（`.claude/skills/git-pr-branch/SKILL.md`）保持同步。
