# AGENTS.md

WhatsBridge receives WhatsApp order webhooks and surfaces them in a Fluent Blazor admin app. Projects and namespaces are named `Clone.*` (template origin), not `WhatsBridge.*`.

## Request flow

`WhatsApp -> Clone.WebApi POST /api/Order/create -> RabbitMQ exchange "orders" -> Clone.Worker -> MongoDB WhatsAppWebhookDb.Orders -> Clone.Web /waorders`

Business/admin data (Identity users, Companies, Documents, OrderHeader/OrderDetail) is stored separately in PostgreSQL via EF Core. Orders are not in EF; don't look for them there.

## Projects (`src/`)

- `Clone.Web` - Blazor Web App (interactive server), Fluent UI, Identity, EF Core/Npgsql, Serilog. Entry point `Program.cs`; pages in `Components/Pages` (`.razor` plus optional `.razor.cs`); page data access uses `IUnitOfWork.Repository<T>()` (see `Companies`).
- `Clone.WebApi` - API host. `Controllers/OrderController.cs` validates the webhook; `Services/*Publisher.cs` publishes RabbitMQ messages.
- `Clone.Worker` - console `BackgroundService`. `RabbitDispatcher` declares exchanges/queues; `Services/*Consumer.cs` handles messages. Note: `RabbitConsumerService .cs` has a space in the filename.
- `Clone.DataAccess` - `ApplicationDbContext`, migrations, generic `RepositoryAsync<T>` / `UnitOfWork`.
- `Clone.Models` - EF entities and `Dtos/WhatsApp*` webhook DTOs.
- `Clone.Utility` - constants, Spanish role names (`RD`), formatting helpers.

## Commands

- Build everything: `dotnet build WhatsBridge.slnx` (uses the `.slnx` format; target is `net10.0`).
- Run web: `dotnet run --project src/Clone.Web` (http://localhost:5007).
- Run API: `dotnet run --project src/Clone.WebApi` (http://localhost:5078).
- Run worker: `dotnet run --project src/Clone.Worker`.
- There are no test projects, CI, lint, or formatter configs. Do not assume `dotnet test` works; use the build as the verification step.

## Local infrastructure

- `docker compose up -d` starts only MongoDB (`27017`) and Mongo Express (`8081`). PostgreSQL and RabbitMQ are not in the compose file.
- RabbitMQ host is hardcoded to `host.docker.internal` in `Clone.WebApi/Program.cs` and `Clone.Worker/Program.cs`; change it to `localhost` if the broker runs directly on the host.
- `Clone.Web` uses only `ConnectionStrings:DefaultConnection` (PostgreSQL); `DockerConnection` in `appsettings.json` is unused. The default password is blank - override with user secrets or `ConnectionStrings__DefaultConnection`.
- Mongo database/collection are hardcoded: `WhatsAppWebhookDb.Orders` (Worker writes, Web reads).
- EF migrations live in `Clone.DataAccess/Migrations` and are not applied at startup. `dotnet-ef` is not installed in this environment; install it, then run `dotnet ef database update --project src/Clone.DataAccess --startup-project src/Clone.Web`.

## Gotchas

- The webhook `POST` requires an `x-api-key` header matching the constant in `OrderController`; anything else returns 401. Sample payload: `src/Clone.WebApi/sample-data/order-sample.json`.
- `OrderUpdatedConsumer` is a TODO that only logs; only `order.created` is persisted to Mongo.
- Identity requires confirmed accounts (`RequireConfirmedAccount = true`) but `IdentityNoOpEmailSender` sends no mail - confirm users manually in the database when testing login. `UseAuthentication()` is commented out in `Clone.Web/Program.cs`; check that line first if auth behaves unexpectedly.
- Roles are seeded in `ApplicationDbContext.OnModelCreating` and named in Spanish. Use the `RD.*` constants (Admin, Cliente, Empleado, Proveedor), not string literals.
- Localization defaults to `es-AR` (`en-US`, `es-AR`, `pt-BR` supported). `LocalizationExtensions` clears request culture providers and sets the culture process-wide; change `Localization:DefaultCulture` in appsettings instead of relying on request headers. Resources are under `Clone.Web/Resources`, with page resources at `Resources/Components/Pages/<Page>/<Page>.resx`.
- Entities derive from `AuditableEntity`; `xmin` is the PostgreSQL concurrency token (configured per entity in `OnModelCreating`). Call `InsertAudit` / `UpdateAudit` before saving.
- New repositories are not auto-registered. Add them in `Clone.Web/Extensions/ServiceCollectionExtensions.cs`.

## Existing docs

- `docs/*.log` and `docs/*.png` are scratch notes/diagrams (Spanish/Portuguese) and are not authoritative.
- `docs/entity.ps1` is a rough scaffolder that appends a `DbSet` and generates a repository/interface; generated code usually needs cleanup. SQL seed/query snippets are in `docs/*.sql`.
