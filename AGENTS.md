# ShuttleVN Backend Agent Guide

This file is the operating guide for coding agents working in `ShuttleVNBackend`.
Read it before changing backend code. The system is a single-facility badminton
court management and booking platform with an AI assistant.

## Product Scope

ShuttleVN serves three roles:

| Role | Main capabilities |
| --- | --- |
| Customer | Register/login, see courts and availability, book/change/cancel a booking, booking history, profile, AI assistance. Guest bookings are supported. |
| Employee | Confirm and manage bookings, customers, court usage, invoices, and payments. |
| Admin | All employee capabilities plus courts, schedules, prices, employees, accounts, roles, audit logs, revenue, and utilization reporting. |

In scope: one badminton facility, REST APIs, PostgreSQL, booking/invoice workflows,
reporting, and an LLM-backed assistant. Out of scope unless explicitly requested:
multi-branch support, loyalty programs, payment-gateway integration, model training,
and integrations such as SMS, email, or Zalo.

## Stack And Solution Layout

- ASP.NET Core Web API, C#, PostgreSQL, EF Core, FluentValidation, Serilog, Swagger/OpenAPI.
- Target authentication is JWT access tokens. Store secrets in configuration/user secrets or
  environment variables, never in source control.
- Keep Clean Architecture dependency direction:

```text
Api -> Application -> Core
Api -> Infrastructure -> Application/Core
Infrastructure -> Application/Core
Core -> (no project dependencies)
```

| Project | Responsibility |
| --- | --- |
| `ShuttleVNBackend.Core` | Entities, enums, domain invariants, and domain-only abstractions. |
| `ShuttleVNBackend.Application` | Use cases, DTOs, validation, authorization requirements, repository contracts, and application exceptions. |
| `ShuttleVNBackend.Infrastructure` | EF Core DbContext/mappings/migrations, repository implementations, auth/token providers, logging, and external clients. |
| `ShuttleVNBackend.Api` | Controllers, HTTP request/response mapping, dependency injection, middleware, authentication, and OpenAPI. |

Do not put controller concerns, EF Core queries, HTTP types, or provider SDK types in Core.
Do not let controllers contain business workflows; delegate to Application services/use cases.

## Repository Tree

Use this layout when deciding where new code belongs. Follow the existing naming
and nesting pattern; do not introduce feature folders in arbitrary layers.

```text
ShuttleVNBackend/
|- ShuttleVNBackend.slnx
|- AGENTS.md
|- ShuttleVNBackend.Api/
|  |- Controllers/                 # HTTP endpoints by resource
|  |- Middlewares/                 # Global exception/auth/request middleware
|  |- Properties/
|  |- Program.cs                   # Composition root and HTTP pipeline
|  |- appsettings.json
|  `- appsettings.Development.json
|- ShuttleVNBackend.Application/
|  |- Common/                      # Shared application models, pagination
|  |- DTOs/
|  |  |- Authentication/
|  |  `- User/
|  |- Exceptions/
|  |- Interfaces/
|  |  `- Repositories/             # Repository and unit-of-work contracts
|  `- UseCases/
|     |- Authentication/
|     `- User/
|- ShuttleVNBackend.Core/
|  |- Docs/                        # Domain/database rules; keep current
|  `- Entities/
|     |- Booking/
|     |- Court/
|     |- System/
|     `- User/
`- ShuttleVNBackend.Infrastructure/
   |- Persistence/
   |  |- Repositories/
   |  |- DependencyInjection.cs
   |  `- ShuttleVnDbContext.cs
   `- (future external/auth/logging implementations)
```

When a module grows, extend it symmetrically: for example, `Booking` DTOs,
validators, services/use cases, repository interface/implementation, controller,
and tests. Keep domain entities under `Core/Entities/<Module>` and their enums in
that module's `Enums` folder.

## Coding Style

- Use English for code, identifiers, comments, API route segments, and log event
  names. Vietnamese is appropriate only for user-facing response messages when
  required by the product.
- Use four spaces, file-scoped namespaces, nullable reference types, `var` only
  when the type is obvious, and braces for every control-flow block.
- Use PascalCase for public types, methods, properties, enum members, and
  constants; camelCase for parameters and locals; `_camelCase` for private fields;
  interfaces start with `I`; async methods end with `Async`.
- Keep one public type per file. Name files after their primary type, such as
  `BookingService.cs`, `CreateBookingRequest.cs`, and `BookingValidator.cs`.
- Keep methods focused and normally under 50 lines. A class should have one
  primary responsibility. Split a class before it becomes difficult to scan;
  treat 300 lines as a strong signal and do not exceed 1,000 lines per file.
- Prefer guard clauses and small private methods to deep nesting. Keep cyclomatic
  complexity below 15 where practical.
- Prefer immutable DTOs (`sealed record` / `record`) for request and response
  models unless the existing binding convention requires mutable properties.
- Use constructor injection. Do not use service locators, static mutable state,
  or `new` for infrastructure dependencies inside Application services.
- Do not use `async void`; return `Task`/`Task<T>`. Propagate
  `CancellationToken` from controller to external/database calls.
- Avoid magic strings and numbers for domain concepts; use enums, constants, or
  clearly named configuration options. Do not hard-code connection strings,
  secrets, timezone assumptions, or pricing values.
- Use comments only to explain a non-obvious invariant, trade-off, or external
  constraint. Do not narrate self-evident code.
- Prefer extending existing files and patterns. Create a new abstraction only
  when it removes meaningful duplication or makes a domain boundary clearer.
- Do not leave TODOs, placeholder implementations, dead code, or commented-out
  code in submitted changes.

## Working Rules

Before editing:

1. Read related entities, DTOs, repository interfaces, services, controllers, mappings, and migrations.
2. Search for an existing local convention before creating a new abstraction.
3. Keep changes focused; do not refactor unrelated modules or overwrite user changes.
4. When persistence changes, update entity mapping and add an EF Core migration in the same change.

Implementation conventions:

- Use nullable reference types, async APIs, `CancellationToken`, and `DateOnly`/`TimeOnly` for booking date/time.
- Use `decimal` for all money. Set PostgreSQL precision explicitly (`numeric(10,2)` or the established equivalent).
- Use `Guid` for account/customer/employee/booking/invoice identifiers and `int` for courts and their schedule/pricing records, matching current entities.
- Persist enum values as strings. Keep enum casing and values consistent across Core, EF mappings, API DTOs, and frontend contracts.
- Validate request DTOs in Application with FluentValidation. Controllers validate transport shape only and should not duplicate business rules.
- Return a consistent API envelope and problem response format established by the API. Do not expose exception messages, stack traces, password hashes, reset codes, or tokens in logs/responses.
- Use UTC for stored instants (`CreatedAt`, `UpdatedAt`, audit times). Treat facility booking times as local business times and document the chosen timezone at the API boundary.
- Prefer soft state transitions over physical deletes for records involved in booking, billing, or customer history.

## HTTP, Authorization, And Errors

- Use RESTful, plural resource routes under the configured API version. Keep controllers thin.
- Apply `[Authorize]` by default to staff/admin operations; explicitly allow only public endpoints such as register, login, available-court lookup, guest booking, and booking-code lookup when intended.
- Enforce role checks server-side. UI visibility is not authorization.
- Required role model: `Customer`, `Employee`, `Admin`; Admin includes employee operational permissions.
- The proposal requires JWT authentication. Do not introduce or extend cookie-based auth for API clients. If the existing bootstrap still uses cookies, migrate it deliberately as a cohesive auth change rather than supporting mixed schemes silently.
- Use appropriate status codes: `400` malformed request, `401` unauthenticated, `403` forbidden, `404` absent resource, `409` state/concurrency/overlap conflict, and `422` business validation failure if that is the established API convention.
- Map known application exceptions in `ExceptionHandlingMiddleware`; log unexpected failures with safe contextual identifiers.

## Domain Rules: Courts And Pricing

- `CourtStatus` controls physical availability. A maintenance/closed court cannot accept new bookings.
- `CourtSchedule` represents weekly opening windows. `DayOfWeek` is ISO-8601: Monday=`1` through Sunday=`7`.
- A booking interval must be fully contained by available schedule windows for its court/day.
- Schedule and `PricingRule` intervals for the same court/day must not overlap. Preserve PostgreSQL exclusion constraints and validate before attempting writes for a useful error response.
- All available schedule time must be covered by pricing rules. Price a booking proportionally by minutes across every applicable price segment.
- Persist the calculated `Booking.TotalCost` as an immutable price snapshot; never recalculate historical bookings after a price change.

## Domain Rules: Booking And Invoice

- Every booking has a valid customer. For guest booking, find or create a `Customer` with `AccountId = null` from supplied contact data.
- A court cannot have overlapping non-cancelled bookings for the same date/time interval. Retain the database exclusion constraint as the final concurrency guard; translate its conflict into a domain/API conflict response.
- Generate unique booking codes as `DS-` plus at least five alphanumeric characters.
- Valid booking states are `PENDING`, `CONFIRMED`, `COMPLETED`, and `CANCELLED`. Support both `PENDING -> CONFIRMED -> COMPLETED` and `PENDING -> COMPLETED` where business conditions permit. Prevent invalid/reversed transitions in Application.
- Every booking creation and state transition creates `BookingStatusHistory`, including old/new state, actor employee when applicable, timestamp, and reason.
- When a booking becomes `CONFIRMED` or `COMPLETED`, automatically create an invoice when there is no valid invoice for it.
- Invoice states are `UNPAID`, `PAID`, `CANCELLED`. Only employees/admins may mark invoices paid or cancelled. Never edit a cancelled invoice into a new one; create a replacement invoice.
- Generate invoice codes as `HD-{yyyyMMdd}-{sequence}`. Preserve the partial unique index allowing at most one unpaid invoice per booking.
- Wrap a state transition, status history write, invoice creation, and audit record in one database transaction.

## Audit, Security, And Reliability

- Audit create/update/delete-style actions for courts, schedules, pricing rules, bookings, and invoices. Include actor account when known, action, entity name/id, and safely serialized old/new values.
- Hash passwords with a modern ASP.NET password hasher; never implement custom hashing. Enforce account status on authentication and protected requests.
- Lock or disable an account after five consecutive failed login attempts according to the account policy and record the event in audit logs. Reset the failure counter after a successful login.
- Never trust an account, employee, customer, or role ID supplied by the client when it can be derived from JWT claims.
- Put connection strings, JWT keys, LLM credentials, and Serilog sinks in configuration. Provide redacted sample values only in committed `appsettings` files.
- Design external calls, especially AI calls, with timeout, cancellation, structured logging, and a graceful unavailable response. Standard booking/management workflows must continue when AI is unavailable.

## AI Assistant Boundary

- The LLM is an advisory service, not a source of truth and not an authorization mechanism.
- Static policy/service FAQs belong in the system prompt or a controlled knowledge source. Dynamic availability, prices, and booking status must be fetched through narrow, server-owned tools/use cases.
- Define explicit tool input/output DTOs. Validate every tool argument and apply the requesting user's authorization before querying data.
- Tools may search availability and explain policies. Do not give the model direct DbContext access, arbitrary SQL, unrestricted internal HTTP access, secret/config access, or implicit authority to mutate bookings/payments.
- Any future AI mutation must require explicit user confirmation and call the same validated Application use case as a normal API request.
- Minimize personal data in prompts and logs; do not expose another customer's details or booking history.

## Testing And Verification

For any touched behavior, add or update focused tests. Prioritize Application unit tests for:

- pricing across multiple pricing rules;
- schedule containment and overlap rejection;
- booking state transitions, status history, and guest-customer creation;
- invoice generation and payment authorization;
- role/ownership authorization and AI tool input validation.

Use integration tests against PostgreSQL-compatible behavior for EF mappings, exclusion constraints, transactions, and migrations. An EF in-memory provider does not prove PostgreSQL range/exclusion behavior.

Before finishing, from `ShuttleVNBackend` run:

```powershell
dotnet build ShuttleVNBackend.slnx
dotnet test ShuttleVNBackend.slnx
dotnet format ShuttleVNBackend.slnx --verify-no-changes
```

Also exercise affected endpoints through Swagger/Postman, including unauthorized and conflict cases. If a command cannot run, state exactly why and do not claim it passed.

## Delivery Checklist

- Clean Architecture direction remains intact.
- DTO validation, authorization, business invariant, database constraint, and error mapping are all covered where relevant.
- Entity/mapping changes include a reviewed migration and no destructive data loss unless explicitly approved.
- OpenAPI and API contracts are updated for public endpoint changes.
- No secrets, PII, passwords, reset codes, or tokens are committed or logged.
- Build, tests, and format verification were run or their limitation is reported.
