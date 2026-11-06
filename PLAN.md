# PLAN.md — Roadmap & Status

> Living document. Updated after every completed task — never deleted.
> The build-on-every-push CI workflow proves the whole file's history is honest.

**Current focus:** Phase 1 — Core stack (EF Core migrations, ProblemDetails, Swagger polish)

---

## Phase 0 — Foundation & Rules

- [x] `.gitignore`, git repo initialized, `main` branch
- [x] Solution restructured into Clean Architecture (`src/` + `tests/`)
- [x] Build succeeds with 0 errors / 0 warnings
- [x] `AGENTS.md` written (AI contract)
- [x] `README.md` written
- [x] Pushed to GitHub

## Phase 1 — Core stack

- [ ] EF Core `DbContext` + SQL Server (LocalDB) + first migration
- [ ] Serilog or structured logging setup
- [ ] Global error handling middleware returning RFC 7807 `ProblemDetails`
- [ ] Health check endpoint (`/health`)
- [ ] Swagger/OpenAPI notes (tags, descriptions, machine-readable docs)

## Phase 2 — Catalog

- [ ] Category entity + CRUD (admin)
- [ ] Product entity + CRUD (admin)
- [ ] List endpoint with search / filter / sort / pagination
- [ ] Seed sample data for local development
- [ ] Unit tests for domain rules; integration tests for endpoints

## Phase 3 — Identity & Auth

- [ ] User registration / login (JWT)
- [ ] Roles: `Admin`, `Customer`
- [ ] Protect admin endpoints; customers see only their own data

## Phase 4 — Cart & Orders

- [ ] Cart per customer (add/remove/update quantity)
- [ ] Checkout → order with item price snapshot
- [ ] Order state machine: `Pending → Confirmed → Shipped → Delivered` / `Cancelled`
- [ ] Cash on delivery (no payment gateway)
- [ ] Customer sees own orders; admin manages all orders

## Phase 5 — Localization (Arabic + English)

- [ ] `Accept-Language` header drives culture
- [ ] Arabic + English content for products/categories
- [ ] Localized error messages via resource files

## Phase 6 — Quality hardening

- [ ] FluentValidation on inputs
- [ ] Integration tests on every endpoint
- [ ] Load the order state machine with unit tests

## Phase 7 — Ship

- [ ] GitHub Actions CI: build + test on every push
- [ ] Dockerfile
- [ ] README screenshots of Swagger
- [ ] (Optional) Deploy to Azure free tier

---

*Each checkbox flips when the human confirms work is done. Honesty is the point.*
