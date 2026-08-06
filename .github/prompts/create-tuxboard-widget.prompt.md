---
description: "Create a new Tuxboard widget: scaffolds the ViewComponent folder/files, inserts the Widget record, and optionally seeds WidgetDefault settings and WidgetDefaultOption dropdown items."
mode: "agent"
tools: ["codebase", "editFiles", "runCommands", "problems"]
---

# Create a Tuxboard Widget

You are a senior .NET architect with deep knowledge of the Tuxboard dashboard framework, ASP.NET Core Razor Pages, View Components, and SQL Server.

## Inputs

- **Widget name** (PascalCase, no spaces): `${input:widgetName:MyWidget}` — used as the folder name, class name prefix, and `Widget.Name` (lowercased).
- **Widget title** (display name): `${input:widgetTitle:My Widget}`
- **Description**: `${input:widgetDescription:A custom Tuxboard widget.}`
- **Group / Category** (`GroupName`): `${input:groupName:General}`
- **Moveable** (can users drag it?): `${input:moveable:true}` — `true` or `false`
- **CanDelete** (can users remove it?): `${input:canDelete:true}` — `true` or `false`
- **UseSettings** (does it have user-editable settings?): `${input:useSettings:false}` — `true` or `false`
- **UseTemplate** (render inside a card wrapper with a draggable header?): `${input:useTemplate:true}` — `true` or `false`; `false` = static widget rendered without header/wrapper.

## Background: How Tuxboard Widgets Work

- A **`Widget`** is a template/definition record in the database. It is never placed directly on a dashboard.
- A **`WidgetPlacement`** is the live instance created from a `Widget` when a user adds it to their dashboard.
- A **View Component** named exactly `Widget.Name` (lowercase) is the ASP.NET Core component that renders the widget's body.
- **`UseTemplate = true`**: the `widgettemplate` View Component wraps the widget in a `<div class="card">` with a draggable header showing the widget title. The widget's View Component renders inside `card-body`.
- **`UseTemplate = false`**: the widget is rendered directly with no card wrapper — suitable for static/full-width widgets.
- **`UseSettings = true`**: the widget supports user-configurable settings stored in `WidgetSetting` records. Each setting is defined in `WidgetDefault`.

## Instructions

### Step 1 — Discover the project

Use `codebase` to find:
- The root namespace of the project (check `Program.cs` or any existing View Component namespace).
- The path to `Pages/Shared/Components/` — this is where all widget folders live.
- The SQL Server connection string or confirm `sqlcmd` / `Invoke-Sqlcmd` is available for database insertion.
- Whether the project uses Plans (`WidgetPlan` table) — check if any `WidgetPlan` rows exist in the database or if `Plan` is referenced in the codebase.
- Whether a widget named `${input:widgetName}` already exists (folder or DB record); stop and report if it does.

### Step 2 — Create the widget folder and files

**Before creating files**, verify `Pages/_ViewImports.cshtml` contains `@addTagHelper *, <AssemblyName>` for the project's own assembly (e.g., `@addTagHelper *, MyProject`). If it is absent and the project uses custom tag helpers (such as `ConditionTagHelper`), add the line. If it already exists, proceed without changes.

Create the following three files under `Pages/Shared/Components/${input:widgetName}/`:

#### 2a. `${input:widgetName}ViewComponent.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;

namespace <RootNamespace>.Pages.Shared.Components.${input:widgetName};

[ViewComponent(Name = "${input:widgetName:MyWidget}".ToLower())]
public class ${input:widgetName}ViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(WidgetPlacement placement)
    {
        var model = new ${input:widgetName}Model { Placement = placement };
        return View("Default", model);
    }
}
```

> **Important:** The `[ViewComponent(Name = "...")]` attribute value must be the **lowercase** version of `${input:widgetName}` — this must exactly match the `Widget.Name` value inserted in Step 3.

> **If the widget needs injected services** (e.g., `DbContext`, `IHttpClientFactory`), use constructor injection and change `Invoke` to `InvokeAsync`:
> ```csharp
> [ViewComponent(Name = "<widgetname-lowercase>")]
> public class ${input:widgetName}ViewComponent : ViewComponent
> {
>     private readonly IMyService _service;
>     public ${input:widgetName}ViewComponent(IMyService service) => _service = service;
> 
>     public async Task<IViewComponentResult> InvokeAsync(WidgetPlacement placement)
>     {
>         var data = await _service.GetDataAsync();
>         var model = new ${input:widgetName}Model { Placement = placement, Data = data };
>         return View("Default", model);
>     }
> }
> ```

#### 2b. `${input:widgetName}Model.cs`

```csharp
using Tuxboard.Core.Infrastructure.Models;

namespace <RootNamespace>.Pages.Shared.Components.${input:widgetName};

/// <summary>
/// View model for the ${input:widgetTitle} widget.
/// </summary>
public class ${input:widgetName}Model : WidgetModel
{
    // Add widget-specific properties here.
}
```

#### 2c. `Default.cshtml`

If `${input:useTemplate}` is `true`, the template wrapper handles the card/header — render only the **body content**:

```cshtml
@model <RootNamespace>.Pages.Shared.Components.${input:widgetName}.${input:widgetName}Model

<p>${input:widgetTitle} widget content goes here.</p>
```

If `${input:useTemplate}` is `false`, render a **complete self-contained** HTML block (no outer card wrapper is provided):

```cshtml
@model <RootNamespace>.Pages.Shared.Components.${input:widgetName}.${input:widgetName}Model

<div class="card mb-3" data-id="@Model.Placement.WidgetPlacementId" data-static="true">
    <div class="card-body">
        <h5 class="card-title">${input:widgetTitle}</h5>
        <p>${input:widgetDescription}</p>
    </div>
</div>
```

### Step 3 — Insert the Widget database record

Determine the connection string from `appsettings.json` (key `ConnectionStrings:DefaultConnection` or similar).

Generate and run the following SQL, substituting all values:

```sql
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Widget]
    WHERE [Name] = '${input:widgetName:MyWidget}'.ToLower()
)
BEGIN
    INSERT INTO [dbo].[Widget]
        ([WidgetId], [Name], [Title], [Description], [ImageUrl],
         [GroupName], [Permission], [Moveable], [CanDelete], [UseSettings], [UseTemplate])
    VALUES
        (NEWID(),
         '<widgetname-lowercase>',
         '${input:widgetTitle}',
         '${input:widgetDescription}',
         '',
         '${input:groupName}',
         0,
         <1 if moveable=true, else 0>,
         <1 if canDelete=true, else 0>,
         <1 if useSettings=true, else 0>,
         <1 if useTemplate=true, else 0>);
END
```

Rules for substitution:
- `<widgetname-lowercase>` = `${input:widgetName}` converted to all lowercase (e.g., `MyWidget` → `mywidget`). This **must** match the `[ViewComponent(Name = "...")]` attribute exactly.
- Boolean flags: `true` → `1`, `false` → `0`.
- `Permission` is always `0` (accessible to everyone).
- `ImageUrl` defaults to `''` (empty string).

Execute using `runCommands`:

```powershell
sqlcmd -S "<ServerName>" -d "<DatabaseName>" -E -Q "<SQL statement above>"
```

Extract `<ServerName>` and `<DatabaseName>` from the connection string found in `appsettings.json`. If Windows Authentication is not available, use `-U <login> -P <password>` instead of `-E`.

After inserting, run this verification query and confirm one row is returned:

```sql
SELECT [WidgetId], [Name], [Title], [GroupName], [UseSettings], [UseTemplate]
FROM [dbo].[Widget]
WHERE [Name] = '<widgetname-lowercase>';
```

If no row is returned, check for errors in the INSERT and retry.

### Step 4 — Link the widget to Plans (conditional)

> **Only perform this step if the project uses Plans** (i.e., a `WidgetPlan` table exists with data, or `Plan` is referenced in the codebase).

A widget that is not linked to any Plan will be invisible to users restricted to a Plan. Insert one row into `WidgetPlan` for each Plan that should have access to this widget:

```sql
DECLARE @WidgetId UNIQUEIDENTIFIER = (
    SELECT [WidgetId] FROM [dbo].[Widget] WHERE [Name] = '<widgetname-lowercase>'
);

-- Repeat for each Plan that should include this widget
INSERT INTO [dbo].[WidgetPlan] ([WidgetId], [PlanId])
SELECT @WidgetId, [PlanId]
FROM [dbo].[Plan]
WHERE [PlanName] = '<PlanName>';   -- e.g. 'Standard', 'Premium'
```

If you are unsure which Plans to assign, ask the user before inserting.

### Step 5 — Insert WidgetDefault settings records

> **Only perform this step if `${input:useSettings}` is `true`.**

For each setting the widget needs, insert one row into `WidgetDefault`. The most common setting is `widgettitle` — always add it first if the widget uses a title.

**SettingType values:**

| Value | Type |
|---|---|
| `0` | String (text input) |
| `1` | Number |
| `2` | Boolean (checkbox) |
| `3` | Date/Time |

**SQL template — repeat once per setting:**

```sql
DECLARE @WidgetId UNIQUEIDENTIFIER = (
    SELECT [WidgetId] FROM [dbo].[Widget] WHERE [Name] = '<widgetname-lowercase>'
);

INSERT INTO [dbo].[WidgetDefault]
    ([WidgetDefaultId], [WidgetId], [SettingName], [SettingTitle],
     [SettingType], [DefaultValue], [SettingIndex])
VALUES
    (NEWID(),
     @WidgetId,
     '<settingname-lowercase>',   -- programmatic name, e.g. 'widgettitle'
     '<SettingTitle>',            -- human-readable label, e.g. 'Title'
     <SettingType>,               -- 0=string, 1=number, 2=boolean, 3=date/time
     '<DefaultValue>',            -- default value as a string, e.g. '${input:widgetTitle}'
     <SettingIndex>);             -- 1-based order index
```

**Rules:**
- `SettingName` must be **lowercase** with no spaces (e.g., `widgettitle`, `refreshinterval`).
- `SettingIndex` starts at `1` and increments by 1 for each additional setting.
- Ask the user to confirm each setting name, title, type, default value, and index before inserting — or proceed with the `widgettitle` setting as a sensible default if `UseSettings=true` and no settings were specified.

**Default `widgettitle` setting (add automatically when `UseSettings=true`):**

```sql
DECLARE @WidgetId UNIQUEIDENTIFIER = (
    SELECT [WidgetId] FROM [dbo].[Widget] WHERE [Name] = '<widgetname-lowercase>'
);

INSERT INTO [dbo].[WidgetDefault]
    ([WidgetDefaultId], [WidgetId], [SettingName], [SettingTitle],
     [SettingType], [DefaultValue], [SettingIndex])
VALUES
    (NEWID(), @WidgetId, 'widgettitle', 'Title', 0, '${input:widgetTitle}', 1);
```

### Step 6 — Insert WidgetDefaultOption dropdown items

> **Only perform this step if a setting from Step 5 is a dropdown** (i.e., the user wants a fixed set of choices for a setting rather than free-text input).

For each dropdown option, insert one row into `WidgetDefaultOption`, linked by `WidgetDefaultId`.

**SQL template — repeat once per option:**

```sql
DECLARE @WidgetDefaultId UNIQUEIDENTIFIER = (
    SELECT [WidgetDefaultId] FROM [dbo].[WidgetDefault]
    WHERE [WidgetId] = (SELECT [WidgetId] FROM [dbo].[Widget] WHERE [Name] = '<widgetname-lowercase>')
      AND [SettingName] = '<settingname-lowercase>'
);

INSERT INTO [dbo].[WidgetDefaultOption]
    ([WidgetOptionId], [WidgetDefaultId], [SettingLabel], [SettingValue], [SettingIndex])
VALUES
    (NEWID(),
     @WidgetDefaultId,
     '<SettingLabel>',   -- visible display text in the dropdown, e.g. 'Small'
     '<SettingValue>',   -- stored value, e.g. 'sm'
     <SettingIndex>);    -- 1-based display order
```

**Rules:**
- `SettingLabel` is the **human-readable** option shown in the dropdown (e.g., `"Small"`, `"Medium"`, `"Large"`).
- `SettingValue` is the **stored/programmatic** value (e.g., `"sm"`, `"md"`, `"lg"`).
- `SettingIndex` starts at `1` and increments by 1 for each option in the same setting.
- Only insert `WidgetDefaultOption` rows for settings that are dropdowns. String/number/boolean/date settings do not use this table.

### Step 7 — Check for build errors

Use the `problems` tool to check for errors in the newly created files. Fix any namespace mismatches or missing `using` directives before completing.

### Step 8 — Report results

After all steps complete, report:

```
✅ Widget created: ${input:widgetName}

Files created:
  Pages/Shared/Components/${input:widgetName}/${input:widgetName}ViewComponent.cs
  Pages/Shared/Components/${input:widgetName}/${input:widgetName}Model.cs
  Pages/Shared/Components/${input:widgetName}/Default.cshtml

Database records inserted:
  Widget:
    Name:        <widgetname-lowercase>
    Title:       ${input:widgetTitle}
    Group:       ${input:groupName}
    Moveable:    ${input:moveable}
    CanDelete:   ${input:canDelete}
    UseSettings: ${input:useSettings}
    UseTemplate: ${input:useTemplate}

  WidgetDefault settings: <list each SettingName + SettingTitle inserted, or "none">
  WidgetDefaultOption items: <list each setting + its options, or "none">
  WidgetPlan records: <list Plans linked, or "not applicable">

Next steps:
  - Add domain logic and properties to ${input:widgetName}Model.cs
  - Update Default.cshtml with your widget's actual content
  - Read settings in the View Component: placement.GetSettingOrDefault("settingname")
  - Read settings in Default.cshtml: @Model.Placement.GetSettingOrDefault("settingname")
```

## Constraints

- Do **not** modify any Tuxboard.Core library files.
- The `[ViewComponent(Name = "...")]` value and `Widget.Name` in the database **must be identical** (both lowercase). A mismatch will cause the widget to silently fail to render.
- Always inherit `${input:widgetName}Model` from `WidgetModel` (`Tuxboard.Core.Infrastructure.Models`).
- The `Default.cshtml` filename is fixed — ASP.NET Core View Components look for `Default.cshtml` by convention.
- Do not use `.Result` or `.Wait()` — use `async`/`await` if the View Component needs async data.
- For async data loading, change `Invoke` to `InvokeAsync` and return `Task<IViewComponentResult>`.
- `WidgetDefault.SettingName` must be **lowercase** with no spaces — it is used as a key in `placement.GetSettingOrDefault("settingname")`.
- Only insert `WidgetDefault` rows when `UseSettings = true`.
- Only insert `WidgetDefaultOption` rows for settings that represent a dropdown — not for string, number, boolean, or date/time settings.
- `WidgetDefaultOption` rows are linked to `WidgetDefault` via `WidgetDefaultId` — never to `Widget` directly.
- Field length limits enforced by the database schema:
  - `Widget.Name` — max **50** characters (must be lowercase)
  - `Widget.Title` — max **30** characters
  - `WidgetDefault.SettingName` — max **20** characters (lowercase, no spaces)
  - `WidgetDefault.SettingTitle` — max **100** characters

## Reference: Widget Flag Meanings

| Flag | `true` / `1` | `false` / `0` |
|---|---|---|
| `Moveable` | User can drag and reposition | Widget is fixed in place |
| `CanDelete` | User can remove from dashboard | Widget is permanent |
| `UseSettings` | Settings dialog available; `WidgetDefault` records define the fields | No settings |
| `UseTemplate` | Rendered inside a card with draggable header via `widgettemplate` | Rendered directly; widget owns its full HTML |

## Reference: SettingType Values

| Value | Type | Input rendered |
|---|---|---|
| `0` | String | Text input |
| `1` | Number | Numeric input |
| `2` | Boolean | Checkbox |
| `3` | Date/Time | Date picker |

## Reference: Reading Settings in a View Component

Use `placement.GetSettingOrDefault("settingname")` to retrieve a setting value at render time. Falls back to `WidgetDefault.DefaultValue` when no user-specific `WidgetSetting` record exists:

```csharp
var title = placement.GetSettingOrDefault("widgettitle");
```
