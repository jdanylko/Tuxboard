---
description: "Create a complete Tuxboard SQL Server database with all tables, indexes, foreign keys, and seed data."
mode: "agent"
tools: ["runCommands", "editFiles"]
---

# Create Tuxboard Database

You are an expert SQL Server database administrator and .NET architect with deep knowledge of the Tuxboard dashboard framework, T-SQL schema design, and EF Core entity mappings.

## Inputs

- **Server name**: `${input:serverName:localhost}`
- **Database name**: `${input:databaseName:TuxboardDb}`
- **Schema**: `${input:schema:dbo}` (leave blank or enter `dbo` to use the default schema)
- **Dashboard UserId type**: `${input:userIdType:int}` — enter `int` for integer user IDs, or `guid` for `uniqueidentifier` (GUID) user IDs

## Task

Generate a complete T-SQL setup script for the Tuxboard database, save it to a `.sql` file, then execute it against the target SQL Server instance.

## Instructions

### Step 1 — Determine inputs

- Use `${input:serverName}` as the SQL Server instance name.
- Use `${input:databaseName}` as the database name.
- Use `${input:schema}` as the schema. If the value is empty, blank, or `dbo`, use `dbo` and skip creating a custom schema.
- Use `${input:userIdType}` to determine the `Dashboard.UserId` column type:
  - `int` → SQL column: `[UserId] int NULL`
  - `guid` (or `uniqueidentifier`) → SQL column: `[UserId] uniqueidentifier NULL`

### Step 2 — Generate the SQL script

Use `editFiles` to create a file named `create-tuxboard-database.sql` in the workspace root with the following content, substituting all bracketed placeholders:

```sql
-- =============================================================
-- Tuxboard Database Setup Script
-- Server:   ${input:serverName}
-- Database: ${input:databaseName}
-- Schema:   ${input:schema}
-- =============================================================

USE [master];
GO

-- Create database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'${input:databaseName}')
BEGIN
    CREATE DATABASE [${input:databaseName}];
END
GO

USE [${input:databaseName}];
GO

-- Create schema if not dbo
-- (Skip this block entirely when schema is "dbo")
IF N'${input:schema}' <> N'dbo'
    AND NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'${input:schema}')
BEGIN
    EXEC('CREATE SCHEMA [${input:schema}]');
END
GO

-- =======================
-- LayoutType
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[LayoutType]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[LayoutType] (
        [LayoutTypeId] int          NOT NULL,
        [Title]        varchar(30)  NOT NULL,
        [Layout]       varchar(MAX) NOT NULL,
        CONSTRAINT [PK_LayoutType] PRIMARY KEY ([LayoutTypeId])
    );
END
GO

-- =======================
-- Plan
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[Plan]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[Plan] (
        [PlanId] int         NOT NULL IDENTITY(1,1),
        [Title]  varchar(50) NOT NULL,
        CONSTRAINT [PK_Plan] PRIMARY KEY ([PlanId])
    );
END
GO

-- =======================
-- Widget
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[Widget]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[Widget] (
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

-- =======================
-- WidgetPlan  (many-to-many join table)
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[WidgetPlan]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[WidgetPlan] (
        [WidgetId] varchar(36) NOT NULL,
        [PlanId]   int         NOT NULL,
        CONSTRAINT [PK_WidgetPlan]       PRIMARY KEY ([WidgetId], [PlanId]),
        CONSTRAINT [FK_WidgetPlan_Widget] FOREIGN KEY ([WidgetId]) REFERENCES [${input:schema}].[Widget]([WidgetId]),
        CONSTRAINT [FK_WidgetPlan_Plan]   FOREIGN KEY ([PlanId])   REFERENCES [${input:schema}].[Plan]([PlanId])
    );

    CREATE INDEX [IX_WidgetPlan_PlanId] ON [${input:schema}].[WidgetPlan]([PlanId]);
END
GO

-- =======================
-- WidgetDefault
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[WidgetDefault]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[WidgetDefault] (
        [WidgetDefaultId] varchar(36)  NOT NULL DEFAULT (newid()),
        [WidgetId]        varchar(36)  NOT NULL,
        [SettingName]     varchar(20)  NOT NULL,
        [SettingTitle]    varchar(100) NOT NULL,
        [DefaultValue]    varchar(MAX) NOT NULL,
        [SettingIndex]    int          NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WidgetDefault]         PRIMARY KEY ([WidgetDefaultId]),
        CONSTRAINT [FK_WidgetDefault_Widget]   FOREIGN KEY ([WidgetId]) REFERENCES [${input:schema}].[Widget]([WidgetId])
    );

    CREATE INDEX [IX_WidgetDefault_WidgetId] ON [${input:schema}].[WidgetDefault]([WidgetId]);
END
GO

-- =======================
-- WidgetDefaultOption
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[WidgetDefaultOption]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[WidgetDefaultOption] (
        [WidgetOptionId]  varchar(36) NOT NULL DEFAULT (newid()),
        [WidgetDefaultId] varchar(36) NOT NULL,
        [SettingLabel]    varchar(30) NOT NULL,
        [SettingValue]    varchar(30) NOT NULL,
        CONSTRAINT [PK_WidgetSettingOption]                 PRIMARY KEY ([WidgetOptionId]),
        CONSTRAINT [FK_WidgetDefaultOption_WidgetDefault]   FOREIGN KEY ([WidgetDefaultId]) REFERENCES [${input:schema}].[WidgetDefault]([WidgetDefaultId])
    );

    CREATE INDEX [IX_WidgetDefaultOption_WidgetDefaultId] ON [${input:schema}].[WidgetDefaultOption]([WidgetDefaultId]);
END
GO

-- =======================
-- Dashboard
-- NOTE: UserId column type is driven by the userIdType input:
--   int  →  [UserId] int NULL
--   guid →  [UserId] uniqueidentifier NULL
-- Substitute the correct declaration below before executing.
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[Dashboard]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[Dashboard] (
        [DashboardId]  uniqueidentifier NOT NULL DEFAULT (newid()),
        [SelectedTab]  int              NOT NULL DEFAULT 1,
        [UserId]       /* REPLACE_USERID_TYPE */ NULL,
        CONSTRAINT [PK_Dashboard] PRIMARY KEY ([DashboardId])
    );
END
GO

-- =======================
-- DashboardTab
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[DashboardTab]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[DashboardTab] (
        [TabId]       varchar(36) NOT NULL DEFAULT (newid()),
        [DashboardId] varchar(36) NOT NULL,
        [TabTitle]    varchar(30) NOT NULL,
        [TabIndex]    int         NOT NULL DEFAULT 1,
        CONSTRAINT [PK_DashboardTab] PRIMARY KEY ([TabId])
    );

    CREATE INDEX [IX_DashboardTab_DashboardId] ON [${input:schema}].[DashboardTab]([DashboardId]);
END
GO

-- =======================
-- Layout
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[Layout]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[Layout] (
        [LayoutId]    varchar(36) NOT NULL DEFAULT (newid()),
        -- NULL TabId means this is a template layout, not assigned to any DashboardTab.
        -- Template layouts are referenced by the DashboardDefault table via the LayoutId field
        -- and are used as the default layout applied when a new dashboard is provisioned for a user.
        [TabId]       varchar(36) NULL,
        [LayoutIndex] int         NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Layout]                        PRIMARY KEY ([LayoutId]),
        CONSTRAINT [FK_DashboardLayout_DashboardTab]  FOREIGN KEY ([TabId]) REFERENCES [${input:schema}].[DashboardTab]([TabId])
    );

    CREATE INDEX [IX_Layout_TabId] ON [${input:schema}].[Layout]([TabId]);
END
GO

-- =======================
-- LayoutRow
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[LayoutRow]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[LayoutRow] (
        [LayoutRowId]  varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutId]     varchar(36) NULL,
        [LayoutTypeId] int         NOT NULL,
        [RowIndex]     int         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_LayoutRow]          PRIMARY KEY ([LayoutRowId]),
        CONSTRAINT [FK_LayoutRow_LayoutType] FOREIGN KEY ([LayoutTypeId]) REFERENCES [${input:schema}].[LayoutType]([LayoutTypeId])
    );

    CREATE INDEX [IX_LayoutRow_LayoutId]     ON [${input:schema}].[LayoutRow]([LayoutId]);
    CREATE INDEX [IX_LayoutRow_LayoutTypeId] ON [${input:schema}].[LayoutRow]([LayoutTypeId]);
END
GO

-- =======================
-- DashboardDefault
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[DashboardDefault]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[DashboardDefault] (
        [DefaultId] varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutId]  varchar(36) NOT NULL,
        [PlanId]    int         NULL,
        CONSTRAINT [PK_DashboardDefault]       PRIMARY KEY ([DefaultId]),
        CONSTRAINT [FK_DashboardDefault_Layout] FOREIGN KEY ([LayoutId]) REFERENCES [${input:schema}].[Layout]([LayoutId]),
        CONSTRAINT [FK_DashboardDefault_Plan]   FOREIGN KEY ([PlanId])   REFERENCES [${input:schema}].[Plan]([PlanId])
    );

    CREATE INDEX [IX_DashboardDefault_LayoutId] ON [${input:schema}].[DashboardDefault]([LayoutId]);
    CREATE INDEX [IX_DashboardDefault_PlanId]   ON [${input:schema}].[DashboardDefault]([PlanId]);
END
GO

-- =======================
-- DashboardDefaultWidget
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[DashboardDefaultWidget]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[DashboardDefaultWidget] (
        [DefaultWidgetId]    varchar(36) NOT NULL DEFAULT (newid()),
        [DashboardDefaultId] varchar(36) NOT NULL,
        [LayoutRowId]        varchar(36) NOT NULL,
        [WidgetId]           varchar(36) NOT NULL,
        [ColumnIndex]        int         NOT NULL DEFAULT 0,
        [WidgetIndex]        int         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_DashboardDefaultWidget]                   PRIMARY KEY ([DefaultWidgetId]),
        CONSTRAINT [FK_DashboardDefaultWidget_DashboardDefault]  FOREIGN KEY ([DashboardDefaultId]) REFERENCES [${input:schema}].[DashboardDefault]([DefaultId]),
        CONSTRAINT [FK_DashboardDefaultWidget_LayoutRow]         FOREIGN KEY ([LayoutRowId])        REFERENCES [${input:schema}].[LayoutRow]([LayoutRowId]),
        CONSTRAINT [FK_DashboardDefaultWidget_Widget]            FOREIGN KEY ([WidgetId])           REFERENCES [${input:schema}].[Widget]([WidgetId])
    );

    CREATE INDEX [IX_DashboardDefaultWidget_DashboardDefaultId] ON [${input:schema}].[DashboardDefaultWidget]([DashboardDefaultId]);
    CREATE INDEX [IX_DashboardDefaultWidget_LayoutRowId]        ON [${input:schema}].[DashboardDefaultWidget]([LayoutRowId]);
    CREATE INDEX [IX_DashboardDefaultWidget_WidgetId]           ON [${input:schema}].[DashboardDefaultWidget]([WidgetId]);
END
GO

-- =======================
-- WidgetPlacement
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[WidgetPlacement]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[WidgetPlacement] (
        [WidgetPlacementId] varchar(36) NOT NULL DEFAULT (newid()),
        [LayoutRowId]       varchar(36) NOT NULL,
        [WidgetId]          varchar(36) NOT NULL,
        [ColumnIndex]       int         NOT NULL DEFAULT 0,
        [WidgetIndex]       int         NOT NULL DEFAULT 0,
        [Collapsed]         bit         NOT NULL DEFAULT 0,
        [UseSettings]       bit         NOT NULL DEFAULT 0,
        [UseTemplate]       bit         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WidgetPlacement]              PRIMARY KEY ([WidgetPlacementId]),
        CONSTRAINT [FK_WidgetPlacement_LayoutRow1]   FOREIGN KEY ([LayoutRowId]) REFERENCES [${input:schema}].[LayoutRow]([LayoutRowId]),
        CONSTRAINT [FK_WidgetPlacement_Widget1]      FOREIGN KEY ([WidgetId])    REFERENCES [${input:schema}].[Widget]([WidgetId])
    );

    CREATE INDEX [IX_WidgetPlacement_LayoutRowId] ON [${input:schema}].[WidgetPlacement]([LayoutRowId]);
    CREATE INDEX [IX_WidgetPlacement_WidgetId]    ON [${input:schema}].[WidgetPlacement]([WidgetId]);
END
GO

-- =======================
-- WidgetSetting
-- =======================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[${input:schema}].[WidgetSetting]') AND type = 'U')
BEGIN
    CREATE TABLE [${input:schema}].[WidgetSetting] (
        [WidgetSettingId]    varchar(36)  NOT NULL DEFAULT (newid()),
        [WidgetPlacementId]  varchar(36)  NOT NULL,
        [WidgetDefaultId]    varchar(36)  NOT NULL,
        [Value]              varchar(MAX) NOT NULL,
        CONSTRAINT [PK_WidgetSetting]                 PRIMARY KEY ([WidgetSettingId]),
        CONSTRAINT [FK_WidgetSetting_WidgetPlacement]  FOREIGN KEY ([WidgetPlacementId]) REFERENCES [${input:schema}].[WidgetPlacement]([WidgetPlacementId]),
        CONSTRAINT [FK_WidgetSetting_WidgetDefault]    FOREIGN KEY ([WidgetDefaultId])   REFERENCES [${input:schema}].[WidgetDefault]([WidgetDefaultId])
    );

    CREATE INDEX [IX_WidgetSetting_WidgetPlacementId] ON [${input:schema}].[WidgetSetting]([WidgetPlacementId]);
    CREATE INDEX [IX_WidgetSetting_WidgetDefaultId]   ON [${input:schema}].[WidgetSetting]([WidgetDefaultId]);
END
GO

-- =============================================================
-- Seed Data
-- =============================================================

-- LayoutType seed
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[LayoutType] WHERE [LayoutTypeId] = 1)
BEGIN
    INSERT INTO [${input:schema}].[LayoutType] ([LayoutTypeId], [Title], [Layout]) VALUES
        (1, 'Three Columns, Equal',      'col-4/col-4/col-4'),
        (2, 'Three Columns, 50% Middle', 'col-3/col-6/col-3'),
        (3, 'Four Columns, 25%',         'col-3/col-3/col-3/col-3'),
        (4, 'Two Columns, 50%',          'col-6/col-6');
END
GO

-- Widget seed
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[Widget] WHERE [WidgetId] = '1885170C-7C48-4557-ABC7-BC06D3FC51EE')
BEGIN
    INSERT INTO [${input:schema}].[Widget]
        ([WidgetId], [Name], [Title], [Description], [ImageUrl], [GroupName], [Permission], [Moveable], [CanDelete], [UseSettings], [UseTemplate])
    VALUES
        ('1885170C-7C48-4557-ABC7-BC06D3FC51EE', 'generalinfo', 'General Info',   'Display General Information',    '', 'General', 0, 0, 0, 0, 0),
        ('C9A9DB53-14CA-4551-87E7-F9656F39A396', 'helloworld',  'Hello World',    'A Simple Hello World Widget',    '', 'Example', 0, 1, 1, 1, 1),
        ('EE84443B-7EE7-4754-BB3C-313CC0DA6039', 'table',       'Sample Table',   'Demonstration of data table',    '', 'General', 0, 1, 1, 1, 1);
END
GO

-- WidgetDefault seed
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[WidgetDefault] WHERE [WidgetDefaultId] = '046F4AA8-5E45-4C86-B2F8-CBF3E42647E7')
BEGIN
    INSERT INTO [${input:schema}].[WidgetDefault]
        ([WidgetDefaultId], [WidgetId], [SettingName], [SettingTitle], [DefaultValue], [SettingIndex])
    VALUES
        ('046F4AA8-5E45-4C86-B2F8-CBF3E42647E7', 'EE84443B-7EE7-4754-BB3C-313CC0DA6039', 'widgettitle', 'Title', 'Sample Table', 1),
        ('5C85537A-1319-48ED-A475-83D3DC3E7A8D', 'C9A9DB53-14CA-4551-87E7-F9656F39A396', 'widgettitle', 'Title', 'Projects',     1);
END
GO

-- Layout seed  (template layout — not linked to any DashboardTab)
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[Layout] WHERE [LayoutId] = '5267DA05-AFE4-4753-9CEE-D5D32C2B068E')
BEGIN
    INSERT INTO [${input:schema}].[Layout] ([LayoutId], [TabId], [LayoutIndex])
    VALUES ('5267DA05-AFE4-4753-9CEE-D5D32C2B068E', NULL, 1);
END
GO

-- LayoutRow seed  (one default "Two Columns, 50%" row for the template layout)
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[LayoutRow] WHERE [LayoutRowId] = 'D58AFCD2-2007-4FD0-87A9-93C85C667F3F')
BEGIN
    INSERT INTO [${input:schema}].[LayoutRow] ([LayoutRowId], [LayoutId], [LayoutTypeId], [RowIndex])
    VALUES ('D58AFCD2-2007-4FD0-87A9-93C85C667F3F', '5267DA05-AFE4-4753-9CEE-D5D32C2B068E', 4, 0);
END
GO

-- DashboardDefault seed
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[DashboardDefault] WHERE [DefaultId] = '0D96A18E-90B8-4A9F-9DF1-126653D68FE6')
BEGIN
    INSERT INTO [${input:schema}].[DashboardDefault] ([DefaultId], [LayoutId], [PlanId])
    VALUES ('0D96A18E-90B8-4A9F-9DF1-126653D68FE6', '5267DA05-AFE4-4753-9CEE-D5D32C2B068E', NULL);
END
GO

-- DashboardDefaultWidget seed  (Hello World widget placed in column 0 of the default row)
IF NOT EXISTS (SELECT 1 FROM [${input:schema}].[DashboardDefaultWidget] WHERE [DefaultWidgetId] = 'D21E94CF-86A9-4058-BB72-F269728AC8AD')
BEGIN
    INSERT INTO [${input:schema}].[DashboardDefaultWidget]
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

### Step 3 — Fix the Dashboard UserId column

After creating the file, open it and replace the placeholder `/* REPLACE_USERID_TYPE */` in the `Dashboard` table definition with the correct SQL type based on `${input:userIdType}`:

- If `${input:userIdType}` is `int` → replace with `int`
- If `${input:userIdType}` is `guid` or `uniqueidentifier` → replace with `uniqueidentifier`

The resulting column declaration must be exactly one of:
- `[UserId] int NULL,`
- `[UserId] uniqueidentifier NULL,`

### Step 4 — Execute the script

Run the following command using `runCommands`, substituting actual input values:

```powershell
sqlcmd -S "${input:serverName}" -E -i "create-tuxboard-database.sql"
```

If Windows Authentication is not available, prompt the user for a SQL login and use:

```powershell
sqlcmd -S "${input:serverName}" -U "<login>" -P "<password>" -i "create-tuxboard-database.sql"
```

If `sqlcmd` is not found, try the PowerShell alternative:

```powershell
Invoke-Sqlcmd -ServerInstance "${input:serverName}" -InputFile "create-tuxboard-database.sql"
```

### Step 5 — Validate

After execution, run the following validation query to confirm all tables were created:

```sql
USE [${input:databaseName}];
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
  AND TABLE_SCHEMA = '${input:schema}'
ORDER BY TABLE_NAME;
```

Execute it with:

```powershell
sqlcmd -S "${input:serverName}" -E -d "${input:databaseName}" -Q "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = '${input:schema}' ORDER BY TABLE_NAME;"
```

Report the list of tables found and confirm all 13 expected tables are present:
`Dashboard`, `DashboardDefault`, `DashboardDefaultWidget`, `DashboardTab`,
`Layout`, `LayoutRow`, `LayoutType`, `Plan`, `Widget`, `WidgetDefault`,
`WidgetDefaultOption`, `WidgetPlan`, `WidgetPlacement`, `WidgetSetting`.

## Error Handling

- **Database already exists**: The `IF NOT EXISTS` guard on `CREATE DATABASE` is safe to re-run.
- **Table already exists**: All `CREATE TABLE` statements are wrapped in `IF NOT EXISTS` — the script is idempotent.
- **Schema not found**: The schema creation block only runs when the schema does not already exist.
- **sqlcmd not found**: Suggest the user install [SQL Server command-line tools](https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility) or use SQL Server Management Studio to run the generated `.sql` file manually.
- **Login failure**: Advise the user to verify the server name and credentials, then retry with `-U`/`-P` flags.
