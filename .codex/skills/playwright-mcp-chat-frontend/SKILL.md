---
name: playwright-mcp-chat-frontend
description: 使用 Playwright MCP 在 NovaClaw 前台（http://localhost:5173/chat/）做自动化 UI 验证与回归。只要用户提到“前台/Chat 页面/数字员工/编辑弹窗/自动化测试/Playwright MCP/浏览器操作/截图/断言提示文案”，就必须触发此 skill。默认打开 http://localhost:5173/chat/，处理登录后继续自动化步骤。
---

# NovaClaw 前台自动化（Playwright MCP）

目标：用 Playwright MCP 直接驱动浏览器完成前台 UI 流程验证（默认 http://localhost:5173/chat/）。

## 基本流程（必须按序）

1. **打开页面**
   - `mcp__playwright__browser_navigate` 打开 `http://localhost:5173/chat/`。

2. **抓取快照**
   - `mcp__playwright__browser_snapshot` 获取元素树。

3. **判断是否需要登录**
   - 如果页面包含「邮箱」「密码」「继续登录」等登录控件：
     - **暂停并提示用户手动登录**（不要尝试自动输入账号）。
     - 用户确认“已登录”后再继续。

4. **执行自动化步骤**
   - 按需求点击/输入/滚动。
   - 每次页面大变更后重新 snapshot。

5. **断言与结果**
   - 检查提示文案/元素是否出现。
   - 如需证据，使用 `mcp__playwright__browser_snapshot` 或 `mcp__playwright__browser_take_screenshot`。

## 默认目标页
- `http://localhost:5173/chat/`

## 登录信息说明
- MCP 的浏览器会话只在当前会话内保留登录态。
- 若用户需要“登录一次后长期保持”，需要依赖产品自身的 remember-me / token 逻辑；此 skill 只负责在 **当前会话** 内复用登录态。

## 常用操作模板

**打开页面并抓取快照**
- `browser_navigate`
- `browser_snapshot`

**点击元素**
- `browser_click` 使用 snapshot ref

**输入文本**
- `browser_type` 或 `browser_fill_form`

**等待与刷新**
- `browser_wait_for`
- `browser_press_key`（如 F5）

## Chrome 说明（重要）
- Playwright MCP 使用内置 Chromium。
- 如用户明确要求“系统 Chrome”，建议改用 Playwright CLI（`--channel chrome`）。

## 安全与体验
- 不要主动读取用户的账号信息。
- 登录步骤默认交给用户手动完成。
- 如果弹窗遮挡操作，优先尝试 `Escape`，再重新 snapshot。
