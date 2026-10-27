# Tuxboard Roadmap

> **Draft.** This roadmap was inferred from the code, not from maintainer plans. Treat every item as a proposal until it is confirmed or dropped.

Current release: **v10.0.14** (.NET 10, EF Core, nullable reference types enabled).

## Recently completed

- Nullable reference types across entities, DTOs, services and EF configuration (v10.0.7)
- Performance and stability pass: fewer N+1 queries, batched saves, `CancellationToken` propagation (v10.0.10)
- Idempotent SQL Server setup script, plus AI prompts for the architecture and database setup

## Near term: finish and align what exists

- **Plan-based widget access.** `Plan` is documented as optional, and `GetWidgetsFor(planId)` exists. The `WidgetPlan` relationship is left for the consumer to add. Ship an opt-in, tested implementation, or document the pattern properly.
- **Review `TuxboardConfigDto`.** `Moveable`, `DeleteWidgets`, `UseSettings` and `Personalized` are client-facing flags. Confirm they are bound from `TuxboardConfig` and documented.
- **Add `ROADMAP.md` and a changelog** to the README and NuGet package docs.

## Test coverage

Tests currently cover `WidgetPlacement`, `Dashboard`, `DashboardService` and `ServiceCollectionExtensions`.

- Add tests for the `TuxDbContextExtensions` query helpers.
- Add tests for the EF configuration classes (schema override, seed data).
- Test the `DashboardService` invariants added in v10.0.7, such as "at least one row per layout" and the null-guard paths.
- Run tests against SQL Server (or a container) in CI as well as SQLite, since the provider-specific behavior can differ.

## API gaps

The `IDashboardService<T>` interface has no tab management or dashboard deletion.

- Tab operations: add, rename, reorder, remove and set the current tab.
- Dashboard operations: delete, reset to template.
- Widget catalog management: add, update and retire `Widget` entries.
- Consider an `IAsyncEnumerable` or paged variant of `GetWidgets` for large catalogs.

## Developer experience

- Cut the sync and async duplication in the service (every mutating method has both). Options are source generation or making async the primary API.
- Support multiple database providers (PostgreSQL, SQLite) with provider-agnostic migrations. Today the schema is set up by a SQL Server script.
- Ship EF Core migrations, or a migration bundle, as an alternative to the SQL script.
- Add a `Tuxboard.Examples` quick-start link and a minimal-API sample to the README.
- Expand XML docs and publish an API reference.

## CI/CD

- `azure-pipelines.yml` is commented as ".NET Framework" and uses `VSTest@2`. Update it to `DotNetCoreCLI@2` with `dotnet test` for .NET 10.
- Automate version bumps and release notes from tags.
- Add dependency and vulnerability scanning (`dotnet list package --vulnerable`).

## Ideas, not committed

- Role-based and plan-based default dashboards beyond the `CreateFromTemplate` flow
- Widget settings validation and typed setting values
- Dashboard import and export (JSON)
- Analytics hooks for widget usage
- Front-end packaging: publish the TypeScript/JavaScript client as an npm package alongside the NuGet package
