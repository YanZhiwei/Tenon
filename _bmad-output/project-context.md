---
project_name: 'Tenon'
user_name: 'YanZhiwei'
date: '2025-02-25'
sections_completed:
  - technology_stack
  - language_rules
  - framework_rules
  - testing_rules
  - quality_rules
  - general_conventions
  - async_exceptions_resources
  - types_visibility_encapsulation
  - linq_collections_strings
  - logging_diagnostics
  - workflow_rules
  - anti_patterns
status: 'complete'
rule_count: 58
optimized_for_llm: true
---

# Project Context for AI Agents

_This file contains critical rules and patterns that AI agents must follow when implementing code in this project. Focus on unobvious details that agents might otherwise miss._

---

## Technology Stack & Versions

- **.NET**: 9.0 only. All `*.csproj` use `<TargetFramework>net9.0</TargetFramework>`. Do not introduce net8.0 or multi-targeting unless explicitly required.
- **C#**: `LangVersion` latest where set; **Nullable** and **ImplicitUsings** enabled globally via `common.props`.
- **Package versions**: Prefer Microsoft.* 9.0.x for framework packages; third-party (e.g. DotNetCore.CAP 8.3.x, AutoMapper 14, FluentValidation 11.x, Castle.Core 5.1, Refit 8.0). Check existing `.csproj` in the same layer before adding new dependencies.
- **Versioning**: Package version is driven by `src/version.props` (VersionPrefix e.g. 0.0.1-alpha). Do not hardcode package version in individual csproj.
- **Repository**: Single solution `src/Tenon.sln`. Build/restore from `./src`; tests live under `test/`, samples under `samples/`.

## Critical Implementation Rules

### Language-Specific Rules (C#)

- **Nullable**: Keep enabled; use `?` and `T?` consistently. Avoid `null!` unless required for DI or serialization.
- **Namespaces**: Match project/package identity (e.g. `Tenon.Helper`, `Tenon.Serialization.Json.Extensions`). Use file-scoped namespaces where the codebase already does.
- **Extension methods**: Place in static classes named `*Extension` or `*Extensions` under an `Extensions` subfolder or namespace; keep them in the same project as the type they extend or in the official Extensions project for that layer.
- **Implicit usings**: Rely on global usings; add project-level or solution-level usings only when necessary for the whole project.
- **注释 / XML 文档**：公开 API 写简短 `<summary>`，需要时加 `<param>`、`<returns>`。一句话说清用途即可，不堆砌废话。实现接口时优先自己写 summary；基类/接口已有完整文档时才用 `<inheritdoc />`。与文件其余注释语言一致（中文或英文）。**C# 标准**：写了 `<param>`、`<returns>`、`<typeparam>` 就必须写清含义，不留空标签；根据方法、类实际写，无内容可写则省略该标签。

### Framework-Specific Rules (Library / ASP.NET Core / EF Core)

- **Options pattern**: Prefer `IOptions<T>`, `IOptionsSnapshot<T>`, or `IOptionsMonitor<T>` and `IConfigureOptions<T>` for configuration. Options classes in `Configurations/` or `Options/` with PascalCase property names.
- **DI registration**: Extension methods on `IServiceCollection` in `*Extension(s).cs`; name pattern `AddXxx()` for the feature. Do not register services in host project without going through these extensions when a Tenon package exists for the concern.
- **Abstractions first**: New features that have multiple implementations must have an Abstractions project (e.g. under `src/Abstractions/`). Implementations reference abstractions only; do not reference another implementation from an implementation project.
- **ASP.NET Core**: Use existing Tenon.AspNetCore and Extensions packages for auth (Bearer/Basic), OpenAPI, identity, localization. Do not duplicate middleware or scheme names that already exist in the repo.
- **EF Core**: Use existing Repository and EfCore packages; follow existing patterns for DbContext, configurations, and migrations. Prefer Pomelo for MySQL, official provider for SQLite.

### Testing Rules

- **Project layout**: Test projects live under `test/`, one project per main library (e.g. `Tenon.Caching.Redis` → `Tenon.Caching.RedisTests`). Name pattern: `{AssemblyName}Tests`.
- **References**: Test projects reference the corresponding `src` project(s) and test frameworks (xUnit/NUnit/MSTest as used in repo). Do not add production appsettings; use test-specific config or in-memory values.
- **Build**: CI runs `dotnet build` from `./src`; run tests via `dotnet test` from repo root or `src`, or by targeting the test project path. Do not assume tests run from the test project directory.

### Code Quality & Style Rules

- **Naming**: PascalCase for types, methods, properties, constants; camelCase for local variables and parameters. Interfaces: `I` prefix. Private fields: optional `_camelCase` or same as property with camelCase.
- **File layout**: One primary type per file; file name matches type name. Group related small types (e.g. options, DTOs) in the same folder; keep `*Extension(s).cs` next to the feature they extend when in the same project.
- **Warnings**: `NoWarn` 1591 (missing XML comment), 0436 (type mismatch with editor). Do not add broad `NoWarn` for new rules without team agreement. Keep `EnablePackageValidation` enabled for library projects.
- **Resources**: `SatelliteResourceLanguages` include zh-Hans, zh-Hant, en. Add new resource keys in a way consistent with existing localization usage (e.g. under `Resources/` or as specified in the consuming package).

### General Code Conventions

- **Method complexity**: Keep cyclomatic complexity per method ≤ 10. If higher, extract helper methods or simplify conditionals (e.g. early returns, guard clauses).
- **Single responsibility**: One method does one thing. If the method name needs "and" or multiple verbs, split into smaller methods and compose.
- **方法体内不写注释**：不写 `// step 1` 这类行内注释。靠命名和小方法表达意图；只有需要说明“为什么”时才在方法上写 `<summary>`。
- **Method length**: Prefer short methods (e.g. under ~30 lines). Long logic should be broken into named private/static helpers with clear responsibilities.
- **Parameters**: Prefer at most 3–4 parameters per method. Use options object or builder when more are needed; avoid long parameter lists.
- **Early return / guard clauses**: Use early returns for validation and edge cases; avoid deep nesting (e.g. reduce `if` nesting beyond 2–3 levels).
- **No magic numbers/strings**: Use named constants, `const`, or configuration; avoid literal numbers or strings that express business or technical meaning without a name.

### Async, Exceptions & Resource Rules

- **Async naming**: Async methods must end with `Async` (e.g. `GetByIdAsync`). Do not add `Async` to non-async methods.
- **Async all the way**: Do not block on async code (e.g. `.GetAwaiter().GetResult()`, `.Result`, `.Wait()` in library code). Use `async`/`await` and let callers decide; expose both sync and async overloads only when the API design explicitly requires it.
- **ConfigureAwait**: In library code, use `ConfigureAwait(false)` on awaited calls unless the method needs to resume on the captured context (e.g. UI). Omit in ASP.NET Core request pipeline when the default context is desired.
- **Exceptions**: Throw specific exception types (e.g. `ArgumentNullException`, `InvalidOperationException`); avoid throwing raw `Exception`. Do not swallow exceptions (empty `catch`); log and rethrow or throw a new one with inner exception when appropriate.
- **Dispose**: Implement `IDisposable`/`IAsyncDisposable` when holding unmanaged or scoped resources; use `using`/`await using` at call sites. Do not dispose objects that the type does not own.

### Types, Visibility & Encapsulation

- **Visibility**: Prefer the most restrictive visibility. Use `internal` for types/members that are not part of the public API; use `private`/`private protected` where possible.
- **Sealing**: Prefer `sealed` for classes not designed for inheritance; leave unsealed only when the type is clearly an extension point (e.g. base options, abstract base).
- **Readonly / init**: Use `readonly` for fields that are set only in constructor; use `init` for immutable DTOs or options when appropriate. Prefer `readonly struct` for small value types that are not mutated.
- **Properties vs methods**: Use properties for pure data with no side effects and cheap computation; use methods when there is I/O, side effects, or non-trivial work.

### LINQ, Collections & Strings

- **LINQ**: Avoid multiple enumeration of the same `IEnumerable`; call `.ToList()` or `.ToArray()` when the sequence is used more than once, or use a single pass. Prefer `IEnumerable<T>` in public API when the caller may only enumerate once.
- **Collections**: Prefer generic collections (`List<T>`, `Dictionary<K,V>`, `HashSet<T>`). Use `ImmutableArray<T>` or `ImmutableList<T>` when returning immutable data from public API if the project already uses them.
- **Strings**: Prefer string interpolation (`$"..."`) or `string.Create` for formatting; avoid unnecessary concatenation in loops. Use `StringComparison.Ordinal` (or appropriate enum) in string comparison methods.

### Logging & Diagnostics

- **Structured logging**: When adding or using logging, prefer structured parameters (e.g. named parameters in message template) rather than string concatenation. Do not log sensitive data (passwords, tokens, PII) at normal levels.
- **Conditional compilation**: Use `#if DEBUG` or `[Conditional("DEBUG")]` only when necessary; prefer configuration or runtime checks for behavior that can be toggled in production.

### Development Workflow Rules

- **Branch**: Main development branch is `dev`. CI build runs on push/PR to `dev`. Do not assume `main` is the default branch for build.
- **Pack**: NuGet packages are produced in Release with `dotnet pack` on `src/Tenon.sln`. Sample and test projects are excluded from publish (workflow excludes names containing Sample, Tests). New library projects under `src/` that should be published must be included in the solution and not match the exclude list.
- **Version bump**: Change `src/version.props` (VersionMajor/Minor/Patch or suffix) when preparing a release; do not duplicate version in csproj.

### Critical Don't-Miss Rules

- **Do not** add a new package that duplicates an existing Tenon capability (e.g. caching, distributed lock, event bus) without going through the existing Abstractions and Extensions; prefer extending existing packages.
- **Do not** disable `Nullable` or `ImplicitUsings` in a library csproj; they are set centrally in `common.props`.
- **Do not** introduce a new Options class without registering it in the corresponding `AddXxx` extension and documenting the configuration section (e.g. in README or code comments).
- **Do not** put app-specific or environment-specific secrets in repo; use appsettings.Development.json (not committed with secrets) or environment variables. Samples may use placeholder connection strings.
- **Do not** remove or relax `EnablePackageValidation` for library projects; it enforces compatibility.
- **Do not** add a direct dependency on a different major version of a package already used in the same solution without aligning or documenting the reason (e.g. in release notes or README).
- **Do not** write methods with cyclomatic complexity > 10; refactor into smaller methods.
- **Do not** add comments inside method bodies; improve naming and extract methods instead.
- **Do not** let a single method do multiple unrelated things; split by responsibility.
- **Do not** block on async (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`) in library code; use async/await.
- **Do not** swallow exceptions in empty `catch`; at minimum log and rethrow or throw with inner exception.
- **Do not** expose public API that returns mutable internal state (e.g. returning a list that callers can mutate); return copies or immutable collections.
- **Do not** use `async void` except for event handlers; use `async Task` or `async ValueTask`.

---

## Usage Guidelines

**For AI Agents:**

- Read this file before implementing any code in the Tenon repository.
- Follow ALL rules exactly as documented.
- When in doubt, prefer the more restrictive option (e.g. keep nullable enabled, use existing abstractions).
- Update this file if you introduce a new pattern or convention that should be followed project-wide.

**For Humans:**

- Keep this file lean and focused on agent needs.
- Update when the technology stack or conventions change.
- Review periodically for outdated rules.
- Remove rules that become obvious over time.

Last Updated: 2025-02-25
