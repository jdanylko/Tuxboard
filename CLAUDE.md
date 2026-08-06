# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test
dotnet test --filter "FullyQualifiedName~TestClassName.MethodName"

# Pack NuGet package
dotnet pack Tuxboard.Core/Tuxboard.Core.csproj
```

## Architecture

Tuxboard is a NuGet library (`Tuxboard.Core`) for embedding configurable dashboards into ASP.NET Core apps. It has no runnable host — examples live in a separate repo ([Tuxboard.Examples](https://github.com/jdanylko/Tuxboard.Examples)).

### Domain model hierarchy

```
Dashboard<T>          ← T is the user-id type (struct); nullable = anonymous dashboard
  └─ DashboardTab
       └─ Layout
            └─ LayoutRow   ← typed by LayoutType (column split config)
                 └─ WidgetPlacement
                      └─ WidgetSetting
```

`DashboardDefault` / `DashboardDefaultWidget` are templates used to bootstrap a new user's dashboard via `CreateFromTemplate`.

`Widget` is the catalog entry; `WidgetPlacement` is a live instance on a dashboard row. `WidgetDefault` / `WidgetDefaultOption` define per-widget configurable fields; `WidgetSetting` stores the per-placement values.

Domain entities live in `Domain/Entities/`. Partial classes in `Domain/Partials/` add behavior (e.g., `Dashboard.GetCurrentTab()`, `LayoutRow.RowContainsWidgets()`).

### Generic user-id pattern

`Dashboard<T>`, `TuxDbContext<T>`, `IDashboardService<T>`, and `DashboardService<T>` are all generic on `T : struct`. This lets the library work with `Guid`, `int`, or any value-type user identifier without coupling to a specific identity system.

### Data layer

`TuxDbContext<T>` extends `DbContext`. All EF table/column mappings are in `Data/Configuration/` using `IEntityTypeConfiguration<T>`. The schema defaults to `dbo` but is overridable via `TuxboardConfig.Schema`.

`Data/Extensions/TuxDbContextExtensions.cs` contains query helpers used directly from `DashboardService` (e.g., `GetPlacementsByLayoutRow`, `GetLayoutForTab`).

### Service layer

`IDashboardService<T>` / `DashboardService<T>` in `Infrastructure/Services/` is the primary API surface. Every mutating method has both sync and async overloads (async takes a `CancellationToken`).

### Registration

Call `AddTuxboardDashboard<T>()` in `Program.cs`. Two overloads: one binds from `IConfiguration` (reads `TuxboardConfig` section), one accepts an `Action<TuxboardConfig>` lambda. Both register `TuxDbContext<T>`, `IDashboardService<T>`, and `ITuxboardConfig` as a singleton.

`TuxboardConfig` carries `ConnectionString`, `Schema`, and feature flags (`UseAnalytics`, `DefaultTab`, `DashboardIndex`).

### Tests

`Tuxboard.Core.Tests` targets `net10.0`, uses xUnit, and substitutes SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) for SQL Server via `TestTuxDbContext`. Tests are organized by namespace mirroring the source (`Infrastructure/Services/`, `Infrastructure/Extensions/`, `Domain/`).
