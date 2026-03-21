---
name: git-review-commit
description: >-
  Stage intended paths, present PR-style per-file git diff --staged, run inline
  code review in Simplified Chinese (per-file notes plus overall summary), propose
  commit title and impact, then git commit only after explicit user confirmation.
  Commit messages default to Chinese. Use for commit/stage flows or when the user
  wants PR-like diffs and human-readable review before landing—not for delegating
  review to external gstack-style checklists unless the user explicitly asks elsewhere.
---

# Git：PR 式暂存对比 + 内联 Code review + 确认后提交

本技能专注两件事绑在一起：**下一颗 commit 里有什么**（和 PR「Files changed」一样按文件看），以及**基于这些 diff 的中文 code review**。**不使用 `gstack-review`**：审阅效果依赖本技能内的 PR 对比 + 逐文件/总体评注。

| 层次 | 对比基准 | 用途 |
|------|----------|------|
| **改动（PR 视图）** | `git diff --staged`（或约定路径下的工作区 diff） | 逐文件 unified diff，便于你像看 PR 一样核对 |
| **Code review** | 同上批 diff + 必要上下文 | **简体中文**：每文件要点 + 可选总体审阅 |
| **提交** | 用户确认后 | `git commit`，标题/正文默认 **简体中文** |

若项目有 `CONTRIBUTING`、`AGENTS.md`、`CLAUDE.md` 等，在**不扩大本次任务范围**的前提下与之对齐。

---

## 何时使用

- 提交、暂存、`git add`、写 message、amend、推送前自检。  
- 需要 **PR 那样按文件看改动**，并要 **code review 结论**（与 diff 同屏、可读）。  
- 用户**未**要求改用其他审查工具时，**始终**用本流程，不要改跑 `gstack-review` 等替代本技能的展示方式。

---

## 总流程（默认）

除非用户说**直接提交、跳过确认**（如「不用给我看，直接 commit」），否则**不要**先执行 `git commit`。

1. **查清范围**：`git status` / `git diff`，敲定本次拟提交的**路径**；勿混入无关改动。  
2. **建暂存（默认）**：对拟提交路径 `git add …`（可用 `git add -p`）。  
   - **例外**：用户要求不暂存、或暂存区混杂其他提交计划——则只用 `git diff -- <paths>`，并写明「未暂存」。  
   - 不提交时可提示 `git restore --staged <paths>`。  
3. **改动（PR 式逐文件）**：按「改动展示」输出 `--stat` / `--numstat` + **每文件**独立 `diff` 代码块。  
4. **Code review（内联，默认必做）**：按「Code review」：每个文件 diff 后 **2～5 条**中文要点；全部文件后可加 **总体审阅**。除非用户明确说「不要 review、只看 diff」，否则**不得省略**。  
5. **提案**：**Git 标题**（默认中文单行）+ **影响**（行为、风险、是否跑过测试）。  
6. **请用户确认**：如「确认后我执行 `git commit`」。已暂存则勿再说「确认后再 add」，除非范围又变。  
7. **仅在用户明确确认后** `git commit`（`-m` 默认中文）。若工作区再变，重新 `add` 并重新展示改动 + review。  
8. 用户改标题/范围/文件后，从相应步骤重来。

**多提交**：每一逻辑单元重复 2→4→5→6→7；批次间可用 `restore --staged` 再处理下一批。

---

## 改动展示（PR 式逐文件）

目标：与 GitHub/GitLab PR **按文件**浏览一致，用 **unified diff**。

1. **变更一览**：先 **`git diff --staged --stat`**（未暂存则用 `git diff --stat -- <paths>`），可选 **`git diff --staged --numstat`**。  
2. **逐文件小节**：对每个变更路径：  
   - 标题：`### 文件：path/to/file`；**新增 / 删除 / 重命名**在标题或行内注明。  
   - 单独 ```diff 代码块，内容为：  
     - 已暂存：`git diff --staged -- <该文件>`  
     - 未暂存：`git diff -- <该文件>`  
   - 保持 Git 原始输出（含 `diff --git`、`@@` 等）。  
3. **大文件**：保留 stat/行数说明；正文可截断并提示本地执行完整命令；**不得**用纯文字替代整块对比。  
4. **二进制**：说明路径与变更类型，配合 `status` / `--stat`。  
5. 展示范围须与即将 `commit` 的暂存（或声明路径）**一致**。

---

## Code review（内联）

- **时机**：紧接在 **PR 式逐文件 diff** 之后；**每个文件** diff 下跟 **「本文件审阅」**（无序列表，**2～5 条**，默认 **简体中文**）；全部结束后可加 **「总体审阅」**（跨文件一致性、提交粒度、测试缺口等）。  
- **原则**：只基于**可见 diff**与合理推断；不得编造未读代码的行为。信息不足写「需确认：…」。  
- **建议覆盖**（按需选用）：正确性与边界、错误处理、安全（相关时）、可维护性与命名、与项目风格/`CONTRIBUTING` 一致性、明显性能问题。  
- **分级**：标明 **阻断**（建议修后再提交）、**建议**、**可选**；若无问题，写明「未发现明显问题」，仍可提一条测试或回归提醒。  
- **协作**：用户坚持带阻断项也提交时，尊重选择，但阻断项**必须**写清楚。  

**说明**：若用户**另行**要求使用 `gstack-review`、`/review` 等，那是**单独请求**，不在本技能默认流程内；本技能仍以 **PR diff + 上述内联 review** 为主交付物。

---

## 提交说明风格（摘要）

- **默认中文**：标题、正文、给用户看的标题文案；项目或用户要求英文时例外。  
- **结构**：单行标题（可 `feat/fix(范围):` + 中文说明）+ 空行 + 正文完整句。  
- 避免空洞词：`更新`、`修一下`、`wip` 等。

## 执行提交（仅确认后）

- 已暂存且范围未变：直接 `git commit`。PowerShell 注意引号，可用 `git commit -F msg.txt`。  
- 完成后可 `git log -1 --oneline`（或 `--stat`）。

## 提交前自检

- `status` / diff / `--staged` 已核对；调试残留与误改已处理。  
- 按项目习惯说明是否跑过测试/构建。

## 暂存策略

- 默认先 `add` 再展示，便于 `git diff --staged`。  
- 按逻辑单元拆分提交；优先 `git add -p` 或按路径；勿擅自 `git add .`。

## 安全与边界

- 非经用户明确要求，不执行 `reset --hard`、`push --force` 等；共享分支 force push 须再确认。  
- 不提交密钥、令牌、不当生成物；敏感内容提醒并协助 `.gitignore` / 取消暂存。

## 快速检查清单

- [ ] **PR 式逐文件** `--staged`（或等价）已展示，与将提交范围一致  
- [ ] **内联 Code review** 已给出（每文件 + 必要时总体），默认中文，除非用户免审  
- [ ] **Git 标题**与**影响**已给出，用户已明确确认（或声明跳过确认）  
- [ ] `git commit` 仅在确认后执行；无不应入库文件  
