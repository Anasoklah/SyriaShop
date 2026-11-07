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
4. The agent has delivered a **detailed work summary** for human review (see section 6).

## 5. Git Rules

- **GitHub Flow is the workflow — the human is the gatekeeper at every gate:**
  1. **Before creating a branch**, the agent MUST ask the human and wait for approval.
     Feature branches are named `feat/<scope>`, e.g. `feat/auth-jwt`.
  2. The agent implements on the branch and commits there, following
     **Conventional Commits** (`feat:`, `fix:`, `chore:`, `docs:`, `test:`) with scope,
     e.g. `feat(catalog): list products with filters`. One logical feature per branch.
  3. The agent MUST **present a detailed summary** of what was done — files changed,
     logic and decisions, test results — so the human can review code and behavior
     BEFORE anything leaves the machine.
  4. **Before pushing**, the agent MUST ask the human and wait for approval.
     The human reviews the diff first.
  5. **Merges happen on GitHub, done by the human** — the agent NEVER merges
     (`--no-ff`, squash or otherwise). After pushing, the human opens the Pull
     Request on GitHub and merges; the merge method the human chooses keeps the
     feature branch visible in history.
  6. After the human merges into `main`, the agent refreshes the local copy
     (`git switch main` + `git pull`) before starting new work.
- Docs/chore commits (`docs:`, `chore:`) may be created on `main` locally, but
  pushing them still requires the human's approval.
- No secrets, connection strings, or keys ever committed. Local config lives in
  `.env` (gitignored; see `.env.example`) or environment variables, never `appsettings.json`.

## 6. Agent Behavior

- If instructions are ambiguous: **ask first**, don't guess.
- Prefer small, reviewable changes over large rewrites.
- Never delete or rewrite files that the human did not ask to change.
- After each completed task or feature, the agent MUST deliver a **detailed report**:
  what was built, which files changed and why, key design decisions and trade-offs,
  test/build results, and what the human should manually verify. Vague one-liners
  do not count as a report.
- Update `PLAN.md` status as part of every significant piece of work.
- Documentation in this repo is written in English; keep it short and honest.

---

*Last reviewed: 2026-11-06 — at least after every major phase.*
