# PLAN.md — SyriaShop Roadmap & Development Status

> Living project roadmap and progress tracker.
> Updated after significant development tasks.
> Completed work must reflect actual implementation and human verification.
>
> This file tracks project progress. It is not a replacement for detailed feature requirements or implementation plans.

---

## Project Status

**Project:** SyriaShop

**Current Phase:** Phase 3 — Identity & Auth

**Current Focus:** Human review of Phase 3 on `feat/auth-jwt`.

**Next Step:** Complete the human review, approve and merge Phase 3, then start planning Phase 4 — Cart & Orders.

**Development Status:** In Progress

---

## Status Definitions

| Status | Meaning |
|---|---|
| Not Started | Work has not started. |
| In Progress | Implementation is currently underway. |
| Implemented | Implementation and relevant tests are complete. |
| Awaiting Human Review | Implementation is ready but human approval is pending. |
| Accepted | Human review is complete and the work is approved. |

### Checklist Rules

- `[ ]` means the task is not yet confirmed as complete.
- `[x]` means the human has confirmed the task is complete.
- AI agents may report implementation progress but must not mark tasks complete without human confirmation.
- Implementation completion and human approval are separate states.
- Never remove completed historical tasks.
- Update this file after significant work, not after every minor code change.

---

# Phase 0 — Foundation & Rules

**Status:** Completed

- [x] `.gitignore` configured.
- [x] Git repository initialized.
- [x] `main` branch established.
- [x] Solution structured using Clean Architecture (`src/` and `tests/`).
- [x] Initial solution builds with zero errors and warnings.
- [x] `AGENTS.md` created with AI development rules.
- [x] `README.md` created.
- [x] Repository published to GitHub: https://github.com/Anasoklah/SyriaShop
- [x] SSH deploy key configured for GitHub access.

---

# Phase 1 — Core Infrastructure & Stack

**Status:** Completed

### Database

- [x] EF Core `AppDbContext` configured.
- [x] SQL Server configured.
- [x] `ConnectionStrings__Default` configured through local environment variables.
- [x] `dotnet-ef` tooling configured.
- [x] Initial migration created and applied to `SyriaShopDb`.
- [x] Design-time DbContext factory implemented.

### API Infrastructure

- [x] Global exception handling.
- [x] Standardized HTTP error responses using ASP.NET Core ProblemDetails.
- [x] Health check endpoint (`/health`).
- [x] Database connectivity health probe.
- [x] OpenAPI document information configured through a document transformer.
- [x] Scalar API reference UI configured for Development (`/scalar/v1`).

### Logging

- [x] Structured logging configured.
- [x] JSON logging formatter in production.
- [x] Human-readable logging in development.
- [x] Logging implemented using BCL functionality without additional packages.

### Initial Testing

- [x] Unit tests for `EnvFileLoader`.
- [x] Six initial tests passing.

---

# Phase 2 — Product Catalog

**Status:** Completed

### Categories

- [x] Category domain entity.
- [x] Category CRUD operations.
- [x] Admin authorization for category management.

### Products

- [x] Product domain entity.
- [x] Product CRUD operations.
- [x] Admin authorization for product management.

### Product Listing

- [x] Product search.
- [x] Filtering.
- [x] Sorting.
- [x] Pagination.

### Development Data

- [x] Sample data seeding for local development.

### Testing

- [x] Domain unit tests.
- [x] API integration tests.
- [x] 29 unit tests passing.
- [x] 14 integration tests passing.

---

# Phase 3 — Identity & Authentication

**Status:** Implemented — Awaiting Human Review

**Branch:** `feat/auth-jwt`

### Authentication

- [x] User registration.
- [x] User login.
- [x] JWT access token generation.
- [x] Access token lifetime configured to approximately 15 minutes.
- [x] Refresh token lifetime configured to 7 days.
- [x] Refresh token rotation.
- [x] Refresh tokens stored in the database.

### Authorization

- [x] `Admin` role.
- [x] `Customer` role.
- [x] Environment-driven initial account seeding.
- [x] Admin-only protection for catalog write operations.
- [x] Generic authentication failure responses.
- [x] Customer-specific authorization to be implemented with Phase 4.

### API Endpoints

- [x] `POST /api/auth/register`
- [x] `POST /api/auth/login`
- [x] `POST /api/auth/refresh`

### Testing

- [x] Access token generator unit tests.
- [x] Authentication integration tests.
- [x] Authorization role-gate matrix tests.
- [x] 32 unit tests passing.
- [x] 24 integration tests passing.
- [x] 56 tests passing in total.

### Human Review

- [ ] Review authentication implementation.
- [ ] Review refresh token lifecycle and security.
- [ ] Review authorization implementation.
- [ ] Inspect Git diff.
- [ ] Approve Phase 3.
- [ ] Merge `feat/auth-jwt` through GitHub.

---

# Phase 4 — Cart & Orders

**Status:** Not Started

**Goal:** Implement the complete shopping and order lifecycle for authenticated customers.

## 4.1 — Shopping Cart

### Domain

- [ ] Define cart business rules.
- [ ] Design Cart domain entity.
- [ ] Design CartItem domain entity.
- [ ] Define cart-product relationships.
- [ ] Define cart quantity validation rules.

### Application

- [ ] Add item to cart.
- [ ] Remove item from cart.
- [ ] Update item quantity.
- [ ] Get current customer's cart.
- [ ] Clear cart when appropriate.

### Infrastructure

- [ ] Configure EF Core cart mappings.
- [ ] Configure database relationships and constraints.
- [ ] Create required migration.

### API

- [ ] Add item endpoint.
- [ ] Remove item endpoint.
- [ ] Update quantity endpoint.
- [ ] Get current cart endpoint.
- [ ] Clear cart endpoint.

### Authorization

- [ ] Customers can access only their own carts.
- [ ] Customers cannot manipulate another customer's cart.

### Testing

- [ ] Cart domain unit tests.
- [ ] Cart application tests.
- [ ] Cart API integration tests.
- [ ] Authorization tests.
- [ ] Invalid quantity and unavailable product tests.

## 4.2 — Checkout & Order Creation

### Domain

- [ ] Design Order domain entity.
- [ ] Design OrderItem domain entity.
- [ ] Define order business invariants.
- [ ] Define order item price snapshot behavior.
- [ ] Define order total calculation rules.

### Application

- [ ] Implement checkout use case.
- [ ] Validate cart contents before checkout.
- [ ] Calculate order totals using server-side product prices.
- [ ] Create order items with price snapshots.
- [ ] Handle cart-to-order conversion.
- [ ] Ensure checkout operations are transactionally consistent.

### Infrastructure

- [ ] Configure Order EF Core mappings.
- [ ] Configure OrderItem EF Core mappings.
- [ ] Configure required database constraints.
- [ ] Create required migration.

### API

- [ ] Implement checkout endpoint.
- [ ] Implement customer's order creation flow.

### Testing

- [ ] Successful checkout.
- [ ] Empty cart checkout.
- [ ] Invalid product checkout.
- [ ] Invalid or unavailable product handling.
- [ ] Price snapshot verification.
- [ ] Order total calculation tests.
- [ ] Transaction failure tests.
- [ ] Relevant concurrency tests.

## 4.3 — Order Management

### Order Lifecycle

- [ ] Implement order status model.
- [ ] Implement allowed state transitions.
- [ ] Implement invalid transition protection.
- [ ] Support cancellation according to approved business rules.

Proposed lifecycle:

```text
Pending → Confirmed → Shipped → Delivered
    |
    +----------------→ Cancelled
```

The allowed cancellation transitions must be explicitly defined before implementation.

### Order Operations

- [ ] Customer can view their own orders.
- [ ] Customer can view individual order details.
- [ ] Admin can view all orders.
- [ ] Admin can manage order statuses.
- [ ] Admin can access individual order details.

### Cash on Delivery

- [ ] Implement Cash on Delivery as the initial payment method.
- [ ] Store the selected payment method in the order.
- [ ] Define payment status independently from order status where required.
- [ ] No external payment gateway in this phase.

### Testing

- [ ] Order domain unit tests.
- [ ] Order state transition tests.
- [ ] Order application tests.
- [ ] Order API integration tests.
- [ ] Customer ownership authorization tests.
- [ ] Admin authorization tests.
- [ ] Cancellation tests.

---

# Phase 5 — Localization (Arabic & English)

**Status:** Not Started

### Culture Management

- [ ] Configure request culture selection using `Accept-Language`.
- [ ] Support Arabic (`ar`).
- [ ] Support English (`en`).
- [ ] Define default culture and fallback behavior.

### Localized Content

- [ ] Define bilingual product content.
- [ ] Define bilingual category content.
- [ ] Update database model where required.
- [ ] Implement localized product responses.
- [ ] Implement localized category responses.

### Error Messages

- [ ] Add resource files for Arabic and English.
- [ ] Localize validation messages.
- [ ] Localize application error messages.
- [ ] Localize API error responses.

### Testing

- [ ] Arabic culture tests.
- [ ] English culture tests.
- [ ] Culture fallback tests.
- [ ] Localized API integration tests.

---

# Phase 6 — Quality Hardening

**Status:** Not Started

### Validation

- [ ] Review existing input validation implementation.
- [ ] Evaluate whether centralized validation is needed.
- [ ] Introduce FluentValidation only if justified and approved.
- [ ] Ensure validation coverage across all relevant inputs.

### Testing

- [ ] Verify integration test coverage for every public endpoint.
- [ ] Add missing integration tests.
- [ ] Complete order state machine unit test coverage.
- [ ] Review authentication and authorization test coverage.
- [ ] Add architecture dependency tests.
- [ ] Verify database constraints and failure scenarios.

### CI

- [ ] Configure GitHub Actions.
- [ ] Restore dependencies.
- [ ] Build the solution.
- [ ] Run unit tests.
- [ ] Run integration tests.
- [ ] Run the workflow on pushes and pull requests.
- [ ] Verify CI results before merging changes.

### Code Quality

- [ ] Review logging and exception handling.
- [ ] Review database query performance.
- [ ] Review asynchronous operations and CancellationToken propagation.
- [ ] Review security-sensitive operations.
- [ ] Remove unnecessary complexity and duplication.

---

# Phase 7 — Packaging & Release

**Status:** Not Started

### Docker

- [ ] Create production-ready Dockerfile.
- [ ] Configure production environment variables.
- [ ] Verify container startup.
- [ ] Verify database connectivity from the container.
- [ ] Validate health checks.

### Documentation

- [ ] Update README with complete setup instructions.
- [ ] Document required environment variables.
- [ ] Document database setup and migrations.
- [ ] Add Scalar API reference screenshots.
- [ ] Document supported API functionality.

### Release Verification

- [ ] Verify clean build.
- [ ] Verify all tests pass.
- [ ] Verify production configuration.
- [ ] Verify no secrets are committed.
- [ ] Review database migration strategy.
- [ ] Perform final security review.
- [ ] Perform final human review.

### Deployment

- [ ] Evaluate hosting options.
- [ ] Select a suitable deployment environment.
- [ ] Deploy the API.
- [ ] Verify production health and API availability.

**Optional:**
- [ ] Deploy to Azure or another suitable hosting provider.

---

# Phase 8 — Future Improvements

**Status:** Not Started

Potential improvements to evaluate after the core shop is complete:

- [ ] Product image upload and storage.
- [ ] Product stock management.
- [ ] Order notifications.
- [ ] Email notifications.
- [ ] Product reviews and ratings.
- [ ] Advanced product filtering.
- [ ] Audit logging for important administrative operations.

These items are not part of the initial delivery scope. They require separate requirements and planning before implementation.

---

## Important Development Rules

1. Complete and review the current phase before starting work on the next phase.
2. Use the Planner agent for significant features and architectural changes.
3. Get human approval for the implementation plan before development.
4. Use the Build agent to implement approved tasks.
5. Use the Reviewer agent for independent code reviews.
6. Fix confirmed findings and rerun relevant tests.
7. Human approval is required before pushing or merging.
8. Never mark work complete based only on an AI agent's statement.
9. Keep this roadmap consistent with the actual implementation.

---

*Last updated: 2026-11-09*