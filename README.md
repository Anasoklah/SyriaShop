# SyriaShop

A professional **e-commerce REST API** built in **.NET 10** with Clean Architecture.
Scoped like a real shop (catalog → cart → orders), not a marketplace.
Developed AI-first: the human drives, the AI implements — rules in [`AGENTS.md`](AGENTS.md).

| Tech | Choice |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| Database | SQL Server (EF Core) |
| Auth | JWT + roles (Admin, Customer) |
| Payments | Cash on delivery |
| Localization | Arabic + English |
| Tests | xUnit (unit + integration) |

## Project structure

```
src/
  SyriaShop.Domain          Entities, domain logic — no framework dependencies
  SyriaShop.Application     Use cases, interfaces, DTOs
  SyriaShop.Infrastructure  EF Core, Migrations, repository implementations
  SyriaShop.Api             Controllers, dependency injection wiring
tests/
  SyriaShop.UnitTests
  SyriaShop.IntegrationTests
```

## Run locally

```bash
dotnet build SyriaShop.slnx
dotnet test SyriaShop.slnx
dotnet run --project src/SyriaShop.Api
```

## Documentation

- [`PLAN.md`](PLAN.md) — roadmap and honest status of every phase
- [`AGENTS.md`](AGENTS.md) — the rules any AI assistant must follow in this repo

## Status

Work in progress — see [`PLAN.md`](PLAN.md) for the live checklist.
