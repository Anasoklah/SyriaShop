# AGENTS.md — Working Rules for AI-Assisted Development

This document is the **contract between the human and any AI assistant** working in this
repository (OpenCode, GitHub Copilot, Cursor, or any other tool).

**Every AI agent MUST read this file before writing any code and MUST NOT violate its rules.**
If a task cannot be done while respecting these rules, the agent must stop and raise the
conflict to the human instead of silently breaking the architecture.

---

## 1. Project Context

- **SyriaShop** — a professional e-commerce REST API built in .NET 10.
- Scope is deliberately focused: a shop backend that is *finished, clean, and testable*,
  not a marketplace like Amazon. Quality over quantity.
- Development is intentionally **AI-assisted**: the human drives, AI implements. The agent
  is expected to explain decisions briefly and document its work in `PLAN.md`.

## 2. Architecture Rules (non-negotiable)

The solution uses Clean Architecture. Dependency direction is **inward only**:

```
Api ──> Infrastructure ──> Application ──> Domain
```

| Project | Contains | May reference | Must NOT contain |
|---|---|---|---|
| `SyriaShop.Domain` | Entities, enums, value objects, domain logic | Nothing | EF Core, HTTP, Database, any framework |
| `SyriaShop.Application` | Use cases, repository/service interfaces, DTOs, validation interfaces | Domain | Implementation details, SQL, DbContext |
| `SyriaShop.Infrastructure` | DbContext, entities configuration, migrations, external services, repository implementations | Application, Domain | Controllers, HTTP concerns |
| `SyriaShop.Api` | Endpoints (Controllers), DI wiring, middleware, auth setup | Application, Infrastructure (DI only) | Business logic, direct DbContext usage |

**Forbidden:** using EF Core types (`DbContext`, `IQueryable`) inside Domain/Application.
Application communicates with persistence through interfaces defined **in Application**
and implemented **in Infrastructure**.

## 3. Coding Conventions

- C#, .NET 10, nullable enabled — no `!` suppressions and no null-handling shortcuts.
- `async`/`await` for all I/O; keep `Program.cs` minimal.
- File names match the contained type; one public type per file.
- Private members: `_camelCase`; constants: `PascalCase`.
- All text must live in approved format:
  - **Interjections, explanations and real content** -> no placeholders like `TODO` comments.
- Error responses follow RFC 7807 (`ProblemDetails`).
- **Plugins or new libraries**: before adding a NuGet package, state why the existing
  stack can't do it; prefer BCL/EF Core features over third-party libs.

## 4. Definition of Done

A feature is done ONLY when:

1. `dotnet build` completes with **0 errors and 0 warnings**.
2. `dotnet test` passes (or the feature has accompanying tests that fail until fixed).
3. Feature is listed in `PLAN.md` and marked for the human to review.

## 5. Git Rules

- **GitHub Flow is the workflow:** feature work happens on branches named
  `feat/<scope>`, e.g. `feat/catalog-crud`. Merging into `main` uses `--no-ff` so
  branch history stays visible (via PR once CI exists; local merge until then).
- Docs/chore commits (`docs:`, `chore:`) may land directly on `main`.
- One logical feature per branch — no "changes and stuff" branches.
- No secrets, connection strings, or keys ever committed. Local config lives in
  `.env` (gitignored; see `.env.example`) or environment variables, never `appsettings.json`.

## 6. Agent Behavior

- If instructions are ambiguous: **ask first**, don't guess.
- Prefer small, reviewable changes over large rewrites.
- Never delete or rewrite files that the human did not ask to change.
- After significant work, update `PLAN.md` status and summarize what changed.
- Documentation in this repo is written in English; keep it short and honest.

---

*Last reviewed: 2026-11-06 — at least after every major phase.*
