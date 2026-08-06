---
arguments: topic
description: >
  Use when the user asks how Tuxboard works, needs to onboard someone to the codebase, or is
  trying to understand any part of the library before building on top of it.
  Trigger phrases: "how does Tuxboard work", "explain the architecture", "what's the domain model",
  "how do I wire up the service layer", "what does DashboardService do", "how are widgets placed",
  "walk me through the codebase", "I'm new to this repo".
  Scoped topics via argument: domain, services, data, config, templates, plans, design.
  Serves: developers integrating Tuxboard.Core, contributors, or anyone doing a first-pass
  orientation of the library before writing code.
---

# Tuxboard Architecture Guide

Explain the architecture of the Tuxboard dashboard library. If a `topic` argument was provided, scope the answer to that section only. Valid topics: `domain`, `services`, `data`, `config`, `templates`, `plans`, `design`.

## Step 0 — Read first (mandatory)

Before answering, read these files to get the current state of the codebase. Do not answer from the embedded prose alone — treat this document as structure guidance only.

- `Tuxboard.Core/Infrastructure/Services/IDashboardService.cs` — authoritative method list
- `Tuxboard.Core/Data/Context/ITuxDbContext.cs` — DbSet inventory
- `Tuxboard.Core/Configuration/TuxboardConfig.cs` — config properties
- `Tuxboard.Core/Domain/Entities/` — scan for all entity files

If any entity, method, or file path mentioned below does not exist in the current codebase, note the discrepancy explicitly and answer from the code, not from this document.

## 1 — Big picture

Tuxboard is a .NET library (`Tuxboard.Core`) for embedding configurable, database-driven dashboards into ASP.NET Core applications. Users arrange widgets across a grid-based layout. The library has no runnable host — examples live in the separate [Tuxboard.Examples](https://github.com/jdanylko/Tuxboard.Examples) repository.

Stack: ASP.NET Core 10+, Entity Framework Core, SQL Server.

## 2 — Domain model hierarchy (`topic: domain`)

Walk through the containment hierarchy top-down:

```
Dashboard<T>           (generic on user ID type: Guid, int, etc.)
  └── DashboardTab     (one or more tabs per dashboard)
        └── Layout     (one layout per tab)
              └── LayoutRow             (one or more rows, each typed by a LayoutType)
                    └── WidgetPlacement (one widget instance per cell)
                          └── WidgetSetting  (per-placement key/value settings)
```

Key entity notes (verify each against `Domain/Entities/` before stating):

- **`Dashboard<T>`** — `T : struct` is the user ID type (e.g., `Guid`, `int`). `UserId == null` means a static/shared dashboard with no owner.
- **`LayoutType`** — defines column structure as a slash-delimited CSS class string, e.g. `"col-4/col-4/col-4"` for three equal Bootstrap columns. Any 12-column grid system works.
- **`Widget`** — the catalog definition (name, title, flags like `Moveable`, `CanDelete`, `UseSettings`, `UseTemplate`). Not a live instance.
- **`WidgetPlacement`** — a live instance of a widget on a specific `LayoutRow`, tracking `ColumnIndex`, `WidgetIndex`, `Collapsed`.
- **`WidgetDefault`** / **`WidgetDefaultOption`** — define the *schema* for a widget's configurable settings.
- **`WidgetSetting`** — per-placement instance values for those settings.

## 3 — Dashboard template system (`topic: templates`)

- **`DashboardDefault`** — a reusable layout template, optionally scoped to a `Plan`.
- **`DashboardDefaultWidget`** — pre-placed widgets within a template's `LayoutRow`.
- When `GetDashboardFor(config, userId)` is called and no dashboard exists for that user, `DashboardService` auto-creates one from the matching `DashboardDefault` template.

## 4 — Plan / subscription system (`topic: plans`)

- **`Plan`** — optional subscription tier (e.g., Bronze, Gold, Platinum).
- `Widget` entities can be associated with `Plan`s to gate access.
- `DashboardDefault` can be scoped to a `Plan` for tier-specific default layouts.
- `GetWidgetsFor(planId)` returns only widgets available for that plan; pass `0` for all widgets.

## 5 — Service layer (`topic: services`)

Read `IDashboardService.cs` and enumerate all public methods, grouped into:
- Dashboard retrieval / creation
- Layout management
- Widget management
- Sync vs. async overloads

Do not reproduce a static table here — read the interface and report what is actually there. Note any method that exists in sync form but lacks an async counterpart, or vice versa.

## 6 — Data layer (`topic: data`)

Read `ITuxDbContext.cs` and enumerate all `DbSet<>` properties currently defined.

- **`TuxDbContext<T>`** (`Data/Context/`) — EF Core `DbContext`, generic on user ID type. SQL Server via `UseSqlServer()`; migrations assembly resolves to the calling project.
- **`ITuxDbContext<T>`** — interface used in tests (SQLite-backed `TestTuxDbContext` in the test project).
- Table/column mappings are in `Data/Configuration/` via `IEntityTypeConfiguration<T>` classes, one per entity.
- `Data/Extensions/TuxDbContextExtensions.cs` contains query helpers used internally by `DashboardService`.

## 7 — Configuration and DI registration (`topic: config`)

Read `TuxboardConfig.cs` and enumerate all properties currently defined. Then describe registration:

Register via `AddTuxboardDashboard<T>()` in `Program.cs`:

```csharp
// From appsettings.json TuxboardConfig section
builder.Services.AddTuxboardDashboard<Guid>(builder.Configuration);

// Or via lambda
builder.Services.AddTuxboardDashboard<Guid>(config =>
{
    config.ConnectionString = "...";
    config.Schema = "dbo";
});
```

This registers `ITuxboardConfig` (singleton), `TuxDbContext<T>` (scoped), `IDashboardService<T>` (transient), and `ITuxDbContext<T>` (transient).

## 8 — Key design decisions (`topic: design`)

1. **Generic user ID** — `Dashboard<T>` avoids coupling to any specific identity system. Use `Guid` or `int`; string-based IDs should be mapped to a surrogate.
2. **Static vs. user dashboards** — `UserId == null` means a shared/anonymous dashboard.
3. **Template-driven creation** — `DashboardDefault` enables consistent onboarding layouts without manual setup per user.
4. **CSS-agnostic layouts** — `LayoutType.Layout` is a plain slash-delimited string; Bootstrap is the default but any grid system works.
5. **Per-placement settings** — `WidgetSetting` stores instance-level config, schema-driven by `WidgetDefault`.
6. **Plan-gated widgets** — optional `Plan` associations enable subscription-tier feature control.

## Step 9 — Staleness verification (always run)

After completing the explanation, verify claims against the current source using `/code-review` on the two most change-prone files:

```
/code-review Tuxboard.Core/Infrastructure/Services/IDashboardService.cs
/code-review Tuxboard.Core/Data/Context/ITuxDbContext.cs
```

Use the review output to cross-check:
1. Every method mentioned in the services section exists in `IDashboardService.cs`
2. Every `DbSet<>` mentioned in the data section exists in `ITuxDbContext.cs`
3. Every entity class mentioned exists as a file in `Domain/Entities/`
4. Every config property mentioned exists in `TuxboardConfig.cs`

Report as:

```
Verification: PASS / FAIL
- Claims checked: N
- Discrepancies: [list any, or "none"]
```

A discrepancy is: a named type, method, or property that does not exist in the current source. If any discrepancy exists, state what the code actually shows instead.
