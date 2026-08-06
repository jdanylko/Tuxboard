---
arguments: userId_type
description: >
  Use when the user needs to create or recreate the Tuxboard SQL Server database from scratch.
  Trigger phrases: "set up the database", "create the Tuxboard DB", "spin up a fresh database",
  "initialize the schema", "I need the SQL script", "scaffold the database for a new project".
  Serves: developers integrating Tuxboard.Core into a new ASP.NET Core application who do not
  yet have a database, or who need to reset to a clean known state.
---

# Create Tuxboard Database

Generate a complete T-SQL setup script for the Tuxboard database, save it to `create-tuxboard-database.sql` in the workspace root, execute it, and validate the result.

## Step 0 — Collect inputs

If `userId_type` was passed as an argument, use it and skip asking. Otherwise ask:

**What type is `Dashboard.UserId`?**
- `int` — integer identity (maps to SQL `int`)
- `guid` — uniqueidentifier (maps to SQL `uniqueidentifier`)

Also ask if not already known:
- **Server name** (default: `localhost`)
- **Database name** (default: `TuxboardDb`)
- **Schema** (default: `dbo` — if blank or `dbo`, skip custom schema creation)

Resolve `<UserIdSqlType>` from the answer:
- `int` → `int`
- `guid` or `uniqueidentifier` → `uniqueidentifier`

## Step 1 — Generate the SQL script

Create `create-tuxboard-database.sql` in the workspace root. Substitute `<DatabaseName>`, `<Schema>`, and `<UserIdSqlType>` throughout. Verify after writing that the file contains no unresolved `<...>` placeholders before proceeding to Step 2.

```sql
-- =============================================================
-- Tuxboard Database Setup Script
-- =============================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'<DatabaseName>')
BEGIN
    CREATE DATABASE [<DatabaseName>];
END
GO

USE [<DatabaseName>];
GO

-- Skip this block when schema is "dbo"
IF N'<Schema>' <> N'dbo'
    AND NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'<Schema>')
BEGIN
    EXEC('CREATE SCHEMA [<Schema>]');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[LayoutType]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[LayoutType] (
        [LayoutTypeId] int          NOT NULL,
        [Title]        varchar(30)  NOT NULL,
        [Layout]       varchar(MAX) NOT NULL,
        CONSTRAINT [PK_LayoutType] PRIMARY KEY ([LayoutTypeId])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[Plan]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[Plan] (
        [PlanId] int         NOT NULL IDENTITY(1,1),
        [Title]  varchar(50) NOT NULL,
        CONSTRAINT [PK_Plan] PRIMARY KEY ([PlanId])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[Widget]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[Widget] (
        [WidgetId]    varchar(36)  NOT NULL DEFAULT (newid()),
        [Name]        varchar(50)  NOT NULL,
        [Title]       varchar(30)  NOT NULL,
        [Description] text         NOT NULL,
        [ImageUrl]    varchar(200) NOT NULL,
        [GroupName]   varchar(15)  NOT NULL,
        [Permission]  int          NOT NULL DEFAULT 0,
        [Moveable]    bit          NOT NULL DEFAULT 0,
        [CanDelete]   bit          NOT NULL DEFAULT 0,
        [UseSettings] bit          NOT NULL DEFAULT 0,
        [UseTemplate] bit          NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Widget] PRIMARY KEY ([WidgetId])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[WidgetPlan]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[WidgetPlan] (
        [WidgetId] varchar(36) NOT NULL,
        [PlanId]   int         NOT NULL,
        CONSTRAINT [PK_WidgetPlan]       PRIMARY KEY ([WidgetId], [PlanId]),
        CONSTRAINT [FK_WidgetPlan_Widget] FOREIGN KEY ([WidgetId]) REFERENCES [<Schema>].[Widget]([WidgetId]),
        CONSTRAINT [FK_WidgetPlan_Plan]   FOREIGN KEY ([PlanId])   REFERENCES [<Schema>].[Plan]([PlanId])
    );
    CREATE INDEX [IX_WidgetPlan_PlanId] ON [<Schema>].[WidgetPlan]([PlanId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[WidgetDefault]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[WidgetDefault] (
        [WidgetDefaultId] varchar(36)  NOT NULL DEFAULT (newid()),
        [WidgetId]        varchar(36)  NOT NULL,
        [SettingName]     varchar(20)  NOT NULL,
        [SettingTitle]    varchar(100) NOT NULL,
        [DefaultValue]    varchar(MAX) NOT NULL,
        [SettingIndex]    int          NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WidgetDefault]       PRIMARY KEY ([WidgetDefaultId]),
        CONSTRAINT [FK_WidgetDefault_Widget] FOREIGN KEY ([WidgetId]) REFERENCES [<Schema>].[Widget]([WidgetId])
    );
    CREATE INDEX [IX_WidgetDefault_WidgetId] ON [<Schema>].[WidgetDefault]([WidgetId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[WidgetDefaultOption]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[WidgetDefaultOption] (
        [WidgetOptionId]  varchar(36) NOT NULL DEFAULT (newid()),
        [WidgetDefaultId] varchar(36) NOT NULL,
        [SettingLabel]    varchar(30) NOT NULL,
        [SettingValue]    varchar(30) NOT NULL,
        CONSTRAINT [PK_WidgetSettingOption]               PRIMARY KEY ([WidgetOptionId]),
        CONSTRAINT [FK_WidgetDefaultOption_WidgetDefault]  FOREIGN KEY ([WidgetDefaultId]) REFERENCES [<Schema>].[WidgetDefault]([WidgetDefaultId])
    );
    CREATE INDEX [IX_WidgetDefaultOption_WidgetDefaultId] ON [<Schema>].[WidgetDefaultOption]([WidgetDefaultId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[Dashboard]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[Dashboard] (
        [DashboardId]  uniqueidentifier NOT NULL DEFAULT (newid()),
        [SelectedTab]  int              NOT NULL DEFAULT 1,
        [UserId]       <UserIdSqlType>  NULL,
        CONSTRAINT [PK_Dashboard] PRIMARY KEY ([DashboardId])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[DashboardTab]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[DashboardTab] (
        [TabId]       varchar(36) NOT NULL DEFAULT (newid()),
        [DashboardId] varchar(36) NOT NULL,
        [TabTitle]    varchar(30) NOT NULL,
        [TabIndex]    int         NOT NULL DEFAULT 1,
        CONSTRAINT [PK_DashboardTab] PRIMARY KEY ([TabId])
    );
    CREATE INDEX [IX_DashboardTab_DashboardId] ON [<Schema>].[DashboardTab]([DashboardId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[Layout]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[Layout] (
        [LayoutId]    varchar(36) NOT NULL DEFAULT (newid()),
        [TabId]       varchar(36) NULL,
        [LayoutIndex] int         NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Layout]                       PRIMARY KEY ([LayoutId]),
        CONSTRAINT [FK_DashboardLayout_DashboardTab]  FOREIGN KEY ([TabId]) REFERENCES [<Schema>].[DashboardTab]([TabId])
    );
    CREATE INDEX [IX_Layout_TabId] ON [<Schema>].[Layout]([TabId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[LayoutRow]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[LayoutRow] (
        [LayoutRowId]  varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutId]     varchar(36) NULL,
        [LayoutTypeId] int         NOT NULL,
        [RowIndex]     int         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_LayoutRow]           PRIMARY KEY ([LayoutRowId]),
        CONSTRAINT [FK_LayoutRow_LayoutType] FOREIGN KEY ([LayoutTypeId]) REFERENCES [<Schema>].[LayoutType]([LayoutTypeId])
    );
    CREATE INDEX [IX_LayoutRow_LayoutId]     ON [<Schema>].[LayoutRow]([LayoutId]);
    CREATE INDEX [IX_LayoutRow_LayoutTypeId] ON [<Schema>].[LayoutRow]([LayoutTypeId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[DashboardDefault]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[DashboardDefault] (
        [DefaultId] varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutId]  varchar(36) NOT NULL,
        [PlanId]    int         NULL,
        CONSTRAINT [PK_DashboardDefault]       PRIMARY KEY ([DefaultId]),
        CONSTRAINT [FK_DashboardDefault_Layout] FOREIGN KEY ([LayoutId]) REFERENCES [<Schema>].[Layout]([LayoutId]),
        CONSTRAINT [FK_DashboardDefault_Plan]   FOREIGN KEY ([PlanId])   REFERENCES [<Schema>].[Plan]([PlanId])
    );
    CREATE INDEX [IX_DashboardDefault_LayoutId] ON [<Schema>].[DashboardDefault]([LayoutId]);
    CREATE INDEX [IX_DashboardDefault_PlanId]   ON [<Schema>].[DashboardDefault]([PlanId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[DashboardDefaultWidget]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[DashboardDefaultWidget] (
        [DefaultWidgetId]    varchar(36) NOT NULL DEFAULT (newid()),
        [DashboardDefaultId] varchar(36) NOT NULL,
        [LayoutRowId]        varchar(36) NOT NULL,
        [WidgetId]           varchar(36) NOT NULL,
        [ColumnIndex]        int         NOT NULL DEFAULT 0,
        [WidgetIndex]        int         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_DashboardDefaultWidget]                  PRIMARY KEY ([DefaultWidgetId]),
        CONSTRAINT [FK_DashboardDefaultWidget_DashboardDefault]  FOREIGN KEY ([DashboardDefaultId]) REFERENCES [<Schema>].[DashboardDefault]([DefaultId]),
        CONSTRAINT [FK_DashboardDefaultWidget_LayoutRow]         FOREIGN KEY ([LayoutRowId])        REFERENCES [<Schema>].[LayoutRow]([LayoutRowId]),
        CONSTRAINT [FK_DashboardDefaultWidget_Widget]            FOREIGN KEY ([WidgetId])           REFERENCES [<Schema>].[Widget]([WidgetId])
    );
    CREATE INDEX [IX_DashboardDefaultWidget_DashboardDefaultId] ON [<Schema>].[DashboardDefaultWidget]([DashboardDefaultId]);
    CREATE INDEX [IX_DashboardDefaultWidget_LayoutRowId]        ON [<Schema>].[DashboardDefaultWidget]([LayoutRowId]);
    CREATE INDEX [IX_DashboardDefaultWidget_WidgetId]           ON [<Schema>].[DashboardDefaultWidget]([WidgetId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[WidgetPlacement]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[WidgetPlacement] (
        [WidgetPlacementId] varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutRowId]       varchar(36) NOT NULL,
        [WidgetId]          varchar(36) NOT NULL,
        [ColumnIndex]       int         NOT NULL DEFAULT 0,
        [WidgetIndex]       int         NOT NULL DEFAULT 0,
        [Collapsed]         bit         NOT NULL DEFAULT 0,
        [UseSettings]       bit         NOT NULL DEFAULT 0,
        [UseTemplate]       bit         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WidgetPlacement]            PRIMARY KEY ([WidgetPlacementId]),
        CONSTRAINT [FK_WidgetPlacement_LayoutRow1]  FOREIGN KEY ([LayoutRowId]) REFERENCES [<Schema>].[LayoutRow]([LayoutRowId]),
        CONSTRAINT [FK_WidgetPlacement_Widget1]     FOREIGN KEY ([WidgetId])    REFERENCES [<Schema>].[Widget]([WidgetId])
    );
    CREATE INDEX [IX_WidgetPlacement_LayoutRowId] ON [<Schema>].[WidgetPlacement]([LayoutRowId]);
    CREATE INDEX [IX_WidgetPlacement_WidgetId]    ON [<Schema>].[WidgetPlacement]([WidgetId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[<Schema>].[WidgetSetting]') AND type = 'U')
BEGIN
    CREATE TABLE [<Schema>].[WidgetSetting] (
        [WidgetSettingId]   varchar(36)  NOT NULL DEFAULT (newid()),
        [WidgetPlacementId] varchar(36)  NOT NULL,
        [WidgetDefaultId]   varchar(36)  NOT NULL,
        [Value]             varchar(MAX) NOT NULL,
        CONSTRAINT [PK_WidgetSetting]                PRIMARY KEY ([WidgetSettingId]),
        CONSTRAINT [FK_WidgetSetting_WidgetPlacement]  FOREIGN KEY ([WidgetPlacementId]) REFERENCES [<Schema>].[WidgetPlacement]([WidgetPlacementId]),
        CONSTRAINT [FK_WidgetSetting_WidgetDefault]    FOREIGN KEY ([WidgetDefaultId])   REFERENCES [<Schema>].[WidgetDefault]([WidgetDefaultId])
    );
    CREATE INDEX [IX_WidgetSetting_WidgetPlacementId] ON [<Schema>].[WidgetSetting]([WidgetPlacementId]);
    CREATE INDEX [IX_WidgetSetting_WidgetDefaultId]   ON [<Schema>].[WidgetSetting]([WidgetDefaultId]);
END
GO

-- Seed: LayoutType
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[LayoutType] WHERE [LayoutTypeId] = 1)
BEGIN
    INSERT INTO [<Schema>].[LayoutType] ([LayoutTypeId], [Title], [Layout]) VALUES
        (1, 'Three Columns, Equal',      'col-4/col-4/col-4'),
        (2, 'Three Columns, 50% Middle', 'col-3/col-6/col-3'),
        (3, 'Four Columns, 25%',         'col-3/col-3/col-3/col-3'),
        (4, 'Two Columns, 50%',          'col-6/col-6');
END
GO

-- Seed: Widget
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[Widget] WHERE [WidgetId] = '1885170C-7C48-4557-ABC7-BC06D3FC51EE')
BEGIN
    INSERT INTO [<Schema>].[Widget]
        ([WidgetId], [Name], [Title], [Description], [ImageUrl], [GroupName], [Permission], [Moveable], [CanDelete], [UseSettings], [UseTemplate])
    VALUES
        ('1885170C-7C48-4557-ABC7-BC06D3FC51EE', 'generalinfo', 'General Info', 'Display General Information', '', 'General', 0, 0, 0, 0, 0),
        ('C9A9DB53-14CA-4551-87E7-F9656F39A396', 'helloworld',  'Hello World',  'A Simple Hello World Widget', '', 'Example', 0, 1, 1, 1, 1),
        ('EE84443B-7EE7-4754-BB3C-313CC0DA6039', 'table',       'Sample Table', 'Demonstration of data table', '', 'General', 0, 1, 1, 1, 1);
END
GO

-- Seed: WidgetDefault
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[WidgetDefault] WHERE [WidgetDefaultId] = '046F4AA8-5E45-4C86-B2F8-CBF3E42647E7')
BEGIN
    INSERT INTO [<Schema>].[WidgetDefault]
        ([WidgetDefaultId], [WidgetId], [SettingName], [SettingTitle], [DefaultValue], [SettingIndex])
    VALUES
        ('046F4AA8-5E45-4C86-B2F8-CBF3E42647E7', 'EE84443B-7EE7-4754-BB3C-313CC0DA6039', 'widgettitle', 'Title', 'Sample Table', 1),
        ('5C85537A-1319-48ED-A475-83D3DC3E7A8D', 'C9A9DB53-14CA-4551-87E7-F9656F39A396', 'widgettitle', 'Title', 'Projects',     1);
END
GO

-- Seed: Layout (template, not linked to any tab)
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[Layout] WHERE [LayoutId] = '5267DA05-AFE4-4753-9CEE-D5D32C2B068E')
BEGIN
    INSERT INTO [<Schema>].[Layout] ([LayoutId], [TabId], [LayoutIndex])
    VALUES ('5267DA05-AFE4-4753-9CEE-D5D32C2B068E', NULL, 1);
END
GO

-- Seed: LayoutRow (Two Columns, 50% row for the template)
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[LayoutRow] WHERE [LayoutRowId] = 'D58AFCD2-2007-4FD0-87A9-93C85C667F3F')
BEGIN
    INSERT INTO [<Schema>].[LayoutRow] ([LayoutRowId], [LayoutId], [LayoutTypeId], [RowIndex])
    VALUES ('D58AFCD2-2007-4FD0-87A9-93C85C667F3F', '5267DA05-AFE4-4753-9CEE-D5D32C2B068E', 4, 0);
END
GO

-- Seed: DashboardDefault
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[DashboardDefault] WHERE [DefaultId] = '0D96A18E-90B8-4A9F-9DF1-126653D68FE6')
BEGIN
    INSERT INTO [<Schema>].[DashboardDefault] ([DefaultId], [LayoutId], [PlanId])
    VALUES ('0D96A18E-90B8-4A9F-9DF1-126653D68FE6', '5267DA05-AFE4-4753-9CEE-D5D32C2B068E', NULL);
END
GO

-- Seed: DashboardDefaultWidget (Hello World placed in column 0)
IF NOT EXISTS (SELECT 1 FROM [<Schema>].[DashboardDefaultWidget] WHERE [DefaultWidgetId] = 'D21E94CF-86A9-4058-BB72-F269728AC8AD')
BEGIN
    INSERT INTO [<Schema>].[DashboardDefaultWidget]
        ([DefaultWidgetId], [DashboardDefaultId], [LayoutRowId], [WidgetId], [ColumnIndex], [WidgetIndex])
    VALUES
        ('D21E94CF-86A9-4058-BB72-F269728AC8AD',
         '0D96A18E-90B8-4A9F-9DF1-126653D68FE6',
         'D58AFCD2-2007-4FD0-87A9-93C85C667F3F',
         'C9A9DB53-14CA-4551-87E7-F9656F39A396',
         0, 0);
END
GO

PRINT 'Tuxboard database setup complete.';
GO
```

## Step 2 — Execute the script

Run via `sqlcmd` with Windows Authentication:

```powershell
sqlcmd -S "<ServerName>" -E -i "create-tuxboard-database.sql"
```

If Windows Authentication is unavailable, ask the user for SQL login credentials and use:

```powershell
sqlcmd -S "<ServerName>" -U "<login>" -P "<password>" -i "create-tuxboard-database.sql"
```

If `sqlcmd` is not found, try:

```powershell
Invoke-Sqlcmd -ServerInstance "<ServerName>" -InputFile "create-tuxboard-database.sql"
```

If neither is available, tell the user to run the `.sql` file manually in SSMS or Azure Data Studio.

## Step 3 — Validate

Run each check in order. Stop and report on the first failure rather than continuing past a broken state.

**Check 1 — Table count (expected: 14)**
```powershell
sqlcmd -S "<ServerName>" -E -d "<DatabaseName>" -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = '<Schema>';"
```
Pass: returns `14`. Fail: report actual count and list which tables from the expected set are missing:
`Dashboard`, `DashboardDefault`, `DashboardDefaultWidget`, `DashboardTab`,
`Layout`, `LayoutRow`, `LayoutType`, `Plan`, `Widget`, `WidgetDefault`,
`WidgetDefaultOption`, `WidgetPlan`, `WidgetPlacement`, `WidgetSetting`.

**Check 2 — Dashboard.UserId column type**
```powershell
sqlcmd -S "<ServerName>" -E -d "<DatabaseName>" -Q "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '<Schema>' AND TABLE_NAME = 'Dashboard' AND COLUMN_NAME = 'UserId';"
```
Pass: returns `<UserIdSqlType>` (either `int` or `uniqueidentifier` matching the requested type). Fail: report what was found vs. what was expected.

**Check 3 — Seed data row counts**
```powershell
sqlcmd -S "<ServerName>" -E -d "<DatabaseName>" -Q "
SELECT 'LayoutType'  AS [Table], COUNT(*) AS [Rows] FROM [<Schema>].[LayoutType]
UNION ALL
SELECT 'Widget',       COUNT(*) FROM [<Schema>].[Widget]
UNION ALL
SELECT 'WidgetDefault', COUNT(*) FROM [<Schema>].[WidgetDefault]
UNION ALL
SELECT 'Layout',       COUNT(*) FROM [<Schema>].[Layout]
UNION ALL
SELECT 'LayoutRow',    COUNT(*) FROM [<Schema>].[LayoutRow]
UNION ALL
SELECT 'DashboardDefault', COUNT(*) FROM [<Schema>].[DashboardDefault]
UNION ALL
SELECT 'DashboardDefaultWidget', COUNT(*) FROM [<Schema>].[DashboardDefaultWidget];
"
```
Pass: LayoutType=4, Widget=3, WidgetDefault=2, Layout=1, LayoutRow=1, DashboardDefault=1, DashboardDefaultWidget=1. Fail: report which table has the wrong count.

**Check 4 — Foreign key constraint count (expected: 13)**
```powershell
sqlcmd -S "<ServerName>" -E -d "<DatabaseName>" -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON RC.CONSTRAINT_NAME = TC.CONSTRAINT_NAME WHERE TC.TABLE_SCHEMA = '<Schema>';"
```
Pass: returns `13`. Fail: report actual count.

**Final verdict**

```
Validation: PASS / FAIL
- Check 1 (table count):   PASS / FAIL
- Check 2 (UserId type):   PASS / FAIL
- Check 3 (seed data):     PASS / FAIL
- Check 4 (FK count):      PASS / FAIL
```

All four must pass for an overall PASS.

## Error handling

- **Database/table already exists**: all `CREATE` statements are wrapped in `IF NOT EXISTS` — safe to re-run.
- **Schema not found**: schema block only runs when the schema doesn't exist.
- **sqlcmd not found**: suggest installing [SQL Server command-line tools](https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility) or running the file manually in SSMS.
- **Login failure**: verify server name and credentials; retry with `-U`/`-P` flags.
