# AGENTS.md — SyriaShop Development Rules

This document defines the mandatory working rules for every AI assistant and agent working in this repository, including OpenCode, GitHub Copilot, Cursor, and other AI development tools.

Every agent MUST read and follow these instructions before implementing any code.

The human developer is the final decision-maker. AI agents assist with analysis, planning, implementation, and code review, but they do not independently approve architectural or business decisions.

If a task conflicts with these rules, stop and report the conflict instead of silently violating them.

---

## 1. Project Context

**Project:** SyriaShop

**Purpose:** A professional e-commerce REST API built with .NET 10 and ASP.NET Core.

The project is intentionally scoped as a single online shop, not a marketplace.

Core functionality:

- Product and category management.
- Customer registration and authentication.
- Shopping cart management.
- Order creation and management.
- Cash on delivery.
- Arabic and English localization.

**Development approach:**

AI-assisted development: the human drives the engineering process, while AI agents assist with implementation.

The human is responsible for:
- Defining requirements.
- Approving technical and architectural decisions.
- Reviewing implementations.
- Approving Git operations that require authorization.

AI agents are responsible for:
- Understanding existing implementations.
- Following project conventions.
- Implementing approved requirements.
- Writing and running relevant tests.
- Reporting their work accurately.

Quality, maintainability, correctness, and simplicity take priority over implementation speed.

---

## 2. Architecture Rules (Non-Negotiable)

The solution follows Clean Architecture.

### Dependency Direction

Dependencies must point inward:

- API depends on Application.
- API depends on Infrastructure for dependency injection and application composition only.
- Infrastructure depends on Application and Domain.
- Application depends on Domain.
- Domain depends on nothing outside itself.

Reference diagram:

```text
                 API
                /   \
               v     v
        Application  Infrastructure
               ^         |
               |         |
               +---------+
                  |
                  v
                Domain
```

More precisely:

```text
API              ---> Application
API              ---> Infrastructure (DI only)
Infrastructure   ---> Application
Infrastructure   ---> Domain
Application      ---> Domain
Domain           ---> Nothing
```

### Project Responsibilities

| Project | Responsibilities | Must NOT contain |
|---|---|---|
| SyriaShop.Domain | Entities, enums, value objects, domain rules | EF Core, HTTP, database implementations |
| SyriaShop.Application | Use cases, interfaces, DTOs, application logic | DbContext, SQL, persistence implementations |
| SyriaShop.Infrastructure | DbContext, EF configurations, migrations, repositories, external integrations | Controllers, HTTP endpoints, business use cases |
| SyriaShop.Api | Controllers, middleware, authentication configuration, HTTP concerns, DI composition | Business logic, direct database access |

### Strict Architecture Rules

- Domain must not depend on Application, Infrastructure, API, or external frameworks.
- Application must not depend on Infrastructure or API.
- Infrastructure implements abstractions defined by Application.
- Domain owns its business invariants and domain behavior.
- API communicates with Application through its public interfaces and use cases.
- API must not access DbContext or repositories directly.
- EF Core types, DbContext, provider-specific types, and persistence-related IQueryable abstractions must not leak into Domain or Application.
- Never introduce circular project dependencies.
- Do not move responsibilities between layers without an explicit architectural reason.

Follow the existing architecture and implementation patterns. Do not redesign the solution to accommodate personal preferences.

---

## 3. Coding Standards

### General

- Use C# and .NET 10.
- Nullable reference types must remain enabled.
- Use clear and descriptive naming.
- Follow the existing code style.
- Keep methods focused and reasonably small.
- Prefer readable and maintainable code over clever implementations.
- One public top-level type per file.
- File names must match their primary public type.

### Naming Conventions

- Classes, methods, properties, and constants: PascalCase.
- Private fields: _camelCase.
- Local variables and parameters: camelCase.
- Interfaces: IPascalCase.

### Nullability

- Avoid null-forgiving operators (!).
- Never use null suppression to hide an actual nullability problem.
- Handle nullable values explicitly.
- Use appropriate validation and domain constraints.

### Asynchronous Programming

- Use asynchronous APIs for I/O operations.
- Propagate CancellationToken through asynchronous workflows.
- Do not block asynchronous execution using .Result or .Wait().
- Avoid unnecessary Task.Run for I/O operations.
- Do not add async/await when directly returning an existing Task is sufficient.
- Preserve correct exception propagation.

### Code Quality

- Follow SOLID principles where they provide real value.
- Avoid unnecessary abstractions.
- Avoid duplicate business logic.
- Do not introduce design patterns without a clear justification.
- Do not create generic implementations when a simpler solution is sufficient.
- Avoid premature optimization.
- Do not swallow exceptions silently.
- Do not use empty catch blocks.

### Incomplete Implementations

- Do not leave incomplete functionality in a completed feature.
- Do not replace required functionality with placeholder implementations.
- TODO comments must not be used to hide unfinished requirements.
- Report any functionality that remains incomplete.

---

## 4. API Design Rules

- Follow REST conventions.
- Use appropriate HTTP methods and status codes.
- Use request and response DTOs.
- Never expose Domain entities directly as API contracts.
- Keep controllers thin.
- Controllers must not contain business logic or database operations.
- Validate incoming requests appropriately.
- Enforce authentication and authorization at the API boundary.
- Do not expose internal exceptions, stack traces, or sensitive implementation details to clients.
- Error responses must follow RFC 9457 (Problem Details for HTTP APIs).
- Follow the existing global exception handling and error response implementation.

Do not introduce a new API response pattern without discussing it with the human.

---

## 5. Database and EF Core Rules

The project uses SQL Server with Entity Framework Core.

- DbContext belongs to Infrastructure.
- Entity configurations belong to Infrastructure.
- Database migrations belong to Infrastructure.
- Follow the existing EF Core configuration and migration patterns.
- Use asynchronous database operations.
- Propagate CancellationToken to EF Core operations.
- Avoid N+1 query problems.
- Use AsNoTracking for appropriate read-only queries.
- Avoid unnecessary database queries and repeated materialization.
- Use transactions when multiple database operations must succeed or fail atomically.
- Consider concurrency when implementing operations that modify shared resources.
- Never assume that application-level validation alone guarantees database consistency.
- Use appropriate database constraints when required by business rules.

### Migrations

- Inspect generated migrations before considering them complete.
- Never silently delete or rewrite existing migrations.
- Never run destructive database operations without explicit authorization.
- Do not execute database updates against production or shared environments without explicit human approval.
- Report schema changes and their consequences.

---

## 6. Security Rules

Security is a priority, especially for authentication, authorization, customers, and orders.

- Never hardcode secrets, passwords, tokens, or API keys.
- Never commit secrets to Git.
- Never expose sensitive customer information.
- Every customer-specific operation must enforce resource ownership.
- Admin-only operations must require appropriate authorization.
- Do not trust user-supplied identifiers or ownership claims.
- Validate external input.
- Avoid leaking sensitive information through error responses or logs.
- Follow the existing JWT and refresh token implementation.
- Never weaken authentication or authorization to make tests pass.

### Sensitive Business Operations

Changes involving the following require careful analysis and explicit human approval when requirements are unclear:

- Product pricing.
- Order totals.
- Discounts.
- Inventory and stock availability.
- Checkout behavior.
- Order state transitions.
- Authentication and authorization rules.
- Customer data ownership.

Never invent business rules for these operations.

---

## 7. Configuration and Dependencies

### Configuration

- Use environment variables for environment-specific configuration and secrets.
- Local secrets may be stored in the existing gitignored .env file.
- Keep .env.example updated with required variable names and safe example values.
- Never put real credentials in .env.example.
- Non-sensitive default configuration may remain in appsettings.json.
- Never commit real production credentials.

### NuGet Packages

Before introducing a new NuGet package:

1. Explain why the existing .NET BCL or EF Core capabilities are insufficient.
2. Identify the functionality required.
3. Consider existing project dependencies.
4. Explain the package's benefits and trade-offs.
5. Present the proposal to the human and wait for approval.

Prefer built-in .NET functionality and existing dependencies when appropriate.

Never add dependencies simply because they make implementation easier.

---

## 8. Testing Strategy

The project uses xUnit for unit and integration testing.

### Unit Tests

Test:
- Domain invariants.
- Business rules.
- Domain behavior.
- Application logic where appropriate.
- Edge cases and invalid operations.

### Integration Tests

Test:
- Database interactions.
- Repository implementations.
- API endpoints.
- Authentication and authorization.
- Important application workflows.

### Testing Rules

- Follow existing testing patterns and fixtures.
- Every important new business rule must have appropriate tests.
- Test both successful and unsuccessful scenarios.
- Include relevant edge cases.
- Tests must be deterministic and independent.
- Do not delete existing tests to make a build pass.
- Do not weaken assertions simply to make tests pass.
- Do not skip failing tests without a justified and approved reason.
- Do not claim tests passed without actually executing them.

Before completing a feature, run:

```bash
dotnet build SyriaShop.slnx
dotnet test SyriaShop.slnx
```

Report the actual results.

---

## 9. Definition of Done

A feature is considered implemented only when:

1. The solution builds successfully with zero errors.
2. No new compiler warnings are introduced.
3. All relevant existing tests pass.
4. New business behavior has appropriate test coverage.
5. Architecture rules are respected.
6. No unnecessary dependencies or unrelated changes are introduced.
7. PLAN.md accurately reflects the implementation status.
8. A detailed implementation report is provided.

Zero compiler warnings is the target. If existing warnings are present, report them rather than suppressing unrelated warnings.

**Implementation completion and human approval are two separate states.**

The feature is not considered fully accepted until the human has reviewed and approved the changes.

Never mark a PLAN.md checklist item as completed ([x]) without human confirmation.

---

## 10. Git Workflow

GitHub Flow is the project's development workflow.

The human is the gatekeeper for branch creation, pushing, and merging.

### Before Development

1. Inspect the current branch.
2. Run git status.
3. Inspect existing uncommitted changes.
4. Never overwrite or discard pre-existing human changes.
5. Before creating a new branch, ask the human and wait for approval.

### Branch Naming

Use descriptive branch names:

```text
feat/authentication
feat/shopping-cart
feat/order-management
fix/order-validation
docs/update-project-rules
```

One logical feature per branch.

### Commits

Follow Conventional Commits:

```text
feat(auth): implement refresh token rotation
feat(catalog): add product filtering
fix(orders): prevent invalid state transitions
test(cart): cover quantity validation
docs: update development rules
chore: update project dependencies
```

Keep commits focused and meaningful.

### Push and Merge

- Before pushing, ask the human and wait for approval.
- The human reviews the changes before pushing.
- The human creates and merges Pull Requests on GitHub.
- The agent must never merge branches.
- Never force-push or rewrite published Git history without explicit authorization.

### After Merge

After the human confirms that a feature has been merged:

```bash
git switch main
git pull
```

Refresh the local main branch before beginning the next feature.

### Main Branch Exceptions

Documentation and chore changes may be committed locally on main when appropriate.

Pushing still requires human approval.

---

## 11. AI Agent Behavior and Collaboration

The project may use multiple specialized agents.

### Planner Agent

Responsible for:

- Requirements analysis.
- Existing codebase inspection.
- Architecture analysis.
- Implementation planning.
- Identifying risks and edge cases.
- Defining the testing strategy.

The Planner must not implement features or modify project files.

### Build Agent

Responsible for:

- Implementing approved requirements.
- Following the approved implementation plan.
- Writing and running tests.
- Integrating review feedback.
- Reporting implementation results.

The Build Agent is the primary implementation agent.

### Reviewer Agent

Responsible for:

- Independent code review.
- Identifying bugs and architectural violations.
- Security review.
- Database and performance review.
- Identifying missing test coverage.

The Reviewer must not modify files or automatically fix reported problems.

### Collaboration Rules

- Use the Planner for significant features and tasks requiring architectural analysis.
- Use the Reviewer for independent reviews of significant implementations.
- The Build Agent is responsible for implementation and integrating confirmed findings.
- A review report is a recommendation, not automatic approval.
- Do not create multiple agents for the same responsibility without a clear need.
- Agents must respect the responsibilities and restrictions defined in their own configurations.

---

## 12. General Agent Rules

- Read and follow this file before implementing code.
- Read README.md to understand the project.
- Read PLAN.md before planning or modifying significant features.
- Inspect existing implementations before creating new ones.
- Never assume a feature or abstraction exists without checking.
- Ask the human when important requirements or business decisions are unclear.
- Make assumptions explicit when proceeding is reasonable.
- Prefer small, reviewable changes.
- Never modify unrelated files.
- Never delete or rewrite human-owned changes.
- Never silently change existing behavior.
- Never report unverified work as completed.
- Never push or merge without following the Git rules.

### Scope Control

Implement only what the task requires.

Do not use a feature request as an excuse to:

- Rewrite unrelated components.
- Introduce new architectural patterns.
- Refactor the entire solution.
- Change existing business behavior.
- Add unrelated dependencies.
- Generate unnecessary documentation.

If an improvement is useful but outside the requested scope, report it separately.

---

## 13. Documentation Rules

Documentation in this repository is written in English.

- README.md describes the project and how to run it.
- PLAN.md tracks the roadmap and implementation status.
- Keep documentation accurate and concise.
- Update relevant documentation when implementation or project configuration changes.
- Do not document planned functionality as implemented.
- Do not duplicate information unnecessarily.
- Do not invent technical decisions or business requirements.
- Do not create additional documentation files without a clear purpose.

The human confirms completed checklist items in PLAN.md.

Agents may update progress and implementation status but must not falsely mark work as approved.

---

## 14. Required Implementation Report

After every significant implementation task or feature, provide a detailed report.

The report MUST contain:

### 1. Summary
What was implemented and why.

### 2. Files Changed
Every created, modified, or deleted file and the reason for the change.

### 3. Implementation Details
Important logic, responsibilities, and execution flow.

### 4. Design Decisions
Important decisions, alternatives considered, and trade-offs.

### 5. Tests
Tests added or modified.

### 6. Verification Results
Actual build and test results, including failures and warnings.

### 7. Security and Performance
Relevant considerations and potential risks.

### 8. Human Review Checklist
Anything the human should manually inspect or verify.

### 9. Remaining Work
Incomplete tasks, known limitations, and required decisions.

Never provide a vague one-line completion message.

---

**Last reviewed:** 2026-11-09

**Review policy:** Update these rules when the project architecture or development workflow changes. Avoid unnecessary rule additions.