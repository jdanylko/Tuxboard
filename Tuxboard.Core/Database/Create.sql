USE [TuxBoard]
GO
/****** Object:  Schema [tool]    Script Date: 1/31/2020 8:33:40 PM ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'tool')
EXEC sys.sp_executesql N'CREATE SCHEMA [tool]'
GO
/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Split]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[Split](
          @delimited NVARCHAR(MAX),
          @delimiter NVARCHAR(100)
        ) RETURNS @t TABLE (id INT IDENTITY(1,1), val NVARCHAR(MAX))
        AS
        BEGIN
          DECLARE @xml XML
          SET @xml = N''<t>'' + REPLACE(@delimited,@delimiter,''</t><t>'') + ''</t>''

          INSERT INTO @t(val)
          SELECT  r.value(''.'',''varchar(MAX)'') as item
          FROM  @xml.nodes(''/t'') as records(r)
          RETURN
        END' 
END
GO
/****** Object:  Table [dbo].[Dashboard]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Dashboard]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Dashboard](
	[DashboardId] [varchar](36) NOT NULL,
	[SelectedTab] [int] NOT NULL,
	[UserId] [varchar](36) NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[DashboardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DashboardDefault]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DashboardDefault]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DashboardDefault](
	[DefaultId] [varchar](36) NOT NULL,
	[LayoutId] [varchar](36) NOT NULL,
	[PlanId] [int] NULL,
 CONSTRAINT [PK_DashboardDefault] PRIMARY KEY CLUSTERED 
(
	[DefaultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DashboardDefaultWidget]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DashboardDefaultWidget](
	[DefaultWidgetId] [varchar](36) NOT NULL,
	[DashboardDefaultId] [varchar](36) NOT NULL,
	[LayoutRowId] [varchar](36) NOT NULL,
	[WidgetId] [varchar](36) NOT NULL,
	[ColumnIndex] [int] NOT NULL,
	[WidgetIndex] [int] NOT NULL,
 CONSTRAINT [PK_DashboardDefaultWidget] PRIMARY KEY CLUSTERED 
(
	[DefaultWidgetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DashboardTab]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DashboardTab]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DashboardTab](
	[TabId] [varchar](36) NOT NULL,
	[DashboardId] [varchar](36) NOT NULL,
	[TabTitle] [varchar](30) NOT NULL,
	[TabIndex] [int] NOT NULL,
 CONSTRAINT [PK_DashboardTab] PRIMARY KEY CLUSTERED 
(
	[TabId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Layout]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Layout]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Layout](
	[LayoutId] [varchar](36) NOT NULL,
	[TabId] [varchar](36) NULL,
	[LayoutIndex] [int] NOT NULL,
 CONSTRAINT [PK_DashboardLayout_1] PRIMARY KEY CLUSTERED 
(
	[LayoutId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[LayoutRow]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LayoutRow]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LayoutRow](
	[LayoutRowId] [varchar](36) NOT NULL,
	[LayoutId] [varchar](36) NULL,
	[LayoutTypeId] [varchar](36) NULL,
	[RowIndex] [int] NOT NULL,
 CONSTRAINT [PK_LayoutRow] PRIMARY KEY CLUSTERED 
(
	[LayoutRowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[LayoutType]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LayoutType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LayoutType](
	[LayoutTypeId] [varchar](36) NOT NULL,
	[Title] [varchar](30) NOT NULL,
	[Layout] [varchar](max) NOT NULL,
 CONSTRAINT [PK_DashboardLayout] PRIMARY KEY CLUSTERED 
(
	[LayoutTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Plan]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Plan]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Plan](
	[PlanId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Plan] PRIMARY KEY CLUSTERED 
(
	[PlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Widget]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Widget]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Widget](
	[WidgetId] [varchar](36) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Title] [varchar](30) NOT NULL,
	[Description] [text] NOT NULL,
	[ImageUrl] [varchar](200) NOT NULL,
	[GroupName] [varchar](15) NOT NULL,
	[Permission] [int] NOT NULL,
	[Moveable] [bit] NOT NULL,
	[CanDelete] [bit] NOT NULL,
	[UseSettings] [bit] NOT NULL,
	[UseTemplate] [bit] NOT NULL,
 CONSTRAINT [PK_Widget] PRIMARY KEY CLUSTERED 
(
	[WidgetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WidgetDefault]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WidgetDefault]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WidgetDefault](
	[WidgetDefaultId] [varchar](36) NOT NULL,
	[WidgetId] [varchar](36) NOT NULL,
	[SettingName] [varchar](20) NOT NULL,
	[SettingTitle] [varchar](100) NOT NULL,
	[SettingType] [smallint] NOT NULL,
	[DefaultValue] [varchar](max) NOT NULL,
	[SettingIndex] [int] NOT NULL,
 CONSTRAINT [PK_WidgetSettings] PRIMARY KEY CLUSTERED 
(
	[WidgetDefaultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WidgetDefaultOption]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WidgetDefaultOption]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WidgetDefaultOption](
	[WidgetOptionId] [varchar](36) NOT NULL,
	[WidgetDefaultId] [varchar](36) NOT NULL,
	[SettingLabel] [varchar](30) NOT NULL,
	[SettingValue] [varchar](30) NOT NULL,
	[SettingIndex] [int] NOT NULL,
 CONSTRAINT [PK_WidgetSettingOption] PRIMARY KEY CLUSTERED 
(
	[WidgetOptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WidgetPlacement]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WidgetPlacement]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WidgetPlacement](
	[WidgetPlacementId] [varchar](36) NOT NULL,
	[LayoutRowId] [varchar](36) NOT NULL,
	[WidgetId] [varchar](36) NOT NULL,
	[ColumnIndex] [int] NOT NULL,
	[WidgetIndex] [int] NOT NULL,
	[Collapsed] [bit] NOT NULL,
	[UseSettings] [bit] NOT NULL,
	[UseTemplate] [bit] NOT NULL,
 CONSTRAINT [PK_WidgetPlacement] PRIMARY KEY CLUSTERED 
(
	[WidgetPlacementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WidgetPlan]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WidgetPlan]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WidgetPlan](
	[WidgetId] [varchar](36) NOT NULL,
	[PlanId] [int] NOT NULL,
 CONSTRAINT [PK_WidgetPlan] PRIMARY KEY CLUSTERED 
(
	[WidgetId] ASC,
	[PlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WidgetSetting]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WidgetSetting]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WidgetSetting](
	[WidgetSettingId] [varchar](36) NOT NULL,
	[WidgetPlacementId] [varchar](36) NOT NULL,
	[WidgetDefaultId] [varchar](36) NOT NULL,
	[Value] [varchar](max) NOT NULL,
 CONSTRAINT [PK_WidgetSetting] PRIMARY KEY CLUSTERED 
(
	[WidgetSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Dashboard_DashboardId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Dashboard] ADD  CONSTRAINT [DF_Dashboard_DashboardId]  DEFAULT (newid()) FOR [DashboardId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardDefault_DefaultId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DashboardDefault] ADD  CONSTRAINT [DF_DashboardDefault_DefaultId]  DEFAULT (newid()) FOR [DefaultId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardDefaultWidget_DefaultWidgetId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DashboardDefaultWidget] ADD  CONSTRAINT [DF_DashboardDefaultWidget_DefaultWidgetId]  DEFAULT (newid()) FOR [DefaultWidgetId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardTab_DashboardTabId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DashboardTab] ADD  CONSTRAINT [DF_DashboardTab_DashboardTabId]  DEFAULT (newid()) FOR [TabId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardLayout_DashboardLayoutId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Layout] ADD  CONSTRAINT [DF_DashboardLayout_DashboardLayoutId]  DEFAULT (newid()) FOR [LayoutId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Layout_TabId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Layout] ADD  CONSTRAINT [DF_Layout_TabId]  DEFAULT (NULL) FOR [TabId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardLayout_LayoutIndex]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Layout] ADD  CONSTRAINT [DF_DashboardLayout_LayoutIndex]  DEFAULT ((0)) FOR [LayoutIndex]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_LayoutRow_DashboardLayoutRowId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LayoutRow] ADD  CONSTRAINT [DF_LayoutRow_DashboardLayoutRowId]  DEFAULT (newid()) FOR [LayoutRowId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_LayoutRow_RowIndex]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LayoutRow] ADD  CONSTRAINT [DF_LayoutRow_RowIndex]  DEFAULT ((0)) FOR [RowIndex]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DashboardLayout_LayoutId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LayoutType] ADD  CONSTRAINT [DF_DashboardLayout_LayoutId]  DEFAULT (newid()) FOR [LayoutTypeId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Widget_WidgetId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Widget] ADD  CONSTRAINT [DF_Widget_WidgetId]  DEFAULT (newid()) FOR [WidgetId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_WidgetDefault_WidgetDefaultId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WidgetDefault] ADD  CONSTRAINT [DF_WidgetDefault_WidgetDefaultId]  DEFAULT (newid()) FOR [WidgetDefaultId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_WidgetDefaultOption_WidgetOptionId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WidgetDefaultOption] ADD  CONSTRAINT [DF_WidgetDefaultOption_WidgetOptionId]  DEFAULT (newid()) FOR [WidgetOptionId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_WidgetPlacement_WidgetPlacementId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WidgetPlacement] ADD  CONSTRAINT [DF_WidgetPlacement_WidgetPlacementId]  DEFAULT (newid()) FOR [WidgetPlacementId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_WidgetSetting_WidgetSettingId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WidgetSetting] ADD  CONSTRAINT [DF_WidgetSetting_WidgetSettingId]  DEFAULT (newid()) FOR [WidgetSettingId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefault_Layout]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefault]'))
ALTER TABLE [dbo].[DashboardDefault]  WITH CHECK ADD  CONSTRAINT [FK_DashboardDefault_Layout] FOREIGN KEY([LayoutId])
REFERENCES [dbo].[Layout] ([LayoutId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefault_Layout]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefault]'))
ALTER TABLE [dbo].[DashboardDefault] CHECK CONSTRAINT [FK_DashboardDefault_Layout]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefault_Plan]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefault]'))
ALTER TABLE [dbo].[DashboardDefault]  WITH CHECK ADD  CONSTRAINT [FK_DashboardDefault_Plan] FOREIGN KEY([PlanId])
REFERENCES [dbo].[Plan] ([PlanId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefault_Plan]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefault]'))
ALTER TABLE [dbo].[DashboardDefault] CHECK CONSTRAINT [FK_DashboardDefault_Plan]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_DashboardDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget]  WITH CHECK ADD  CONSTRAINT [FK_DashboardDefaultWidget_DashboardDefault] FOREIGN KEY([DashboardDefaultId])
REFERENCES [dbo].[DashboardDefault] ([DefaultId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_DashboardDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget] CHECK CONSTRAINT [FK_DashboardDefaultWidget_DashboardDefault]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_LayoutRow]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget]  WITH CHECK ADD  CONSTRAINT [FK_DashboardDefaultWidget_LayoutRow] FOREIGN KEY([LayoutRowId])
REFERENCES [dbo].[LayoutRow] ([LayoutRowId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_LayoutRow]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget] CHECK CONSTRAINT [FK_DashboardDefaultWidget_LayoutRow]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget]  WITH CHECK ADD  CONSTRAINT [FK_DashboardDefaultWidget_Widget] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widget] ([WidgetId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardDefaultWidget_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardDefaultWidget]'))
ALTER TABLE [dbo].[DashboardDefaultWidget] CHECK CONSTRAINT [FK_DashboardDefaultWidget_Widget]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardTab_Dashboard]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardTab]'))
ALTER TABLE [dbo].[DashboardTab]  WITH CHECK ADD  CONSTRAINT [FK_DashboardTab_Dashboard] FOREIGN KEY([DashboardId])
REFERENCES [dbo].[Dashboard] ([DashboardId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardTab_Dashboard]') AND parent_object_id = OBJECT_ID(N'[dbo].[DashboardTab]'))
ALTER TABLE [dbo].[DashboardTab] CHECK CONSTRAINT [FK_DashboardTab_Dashboard]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardLayout_DashboardTab]') AND parent_object_id = OBJECT_ID(N'[dbo].[Layout]'))
ALTER TABLE [dbo].[Layout]  WITH CHECK ADD  CONSTRAINT [FK_DashboardLayout_DashboardTab] FOREIGN KEY([TabId])
REFERENCES [dbo].[DashboardTab] ([TabId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DashboardLayout_DashboardTab]') AND parent_object_id = OBJECT_ID(N'[dbo].[Layout]'))
ALTER TABLE [dbo].[Layout] CHECK CONSTRAINT [FK_DashboardLayout_DashboardTab]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LayoutRow_Layout]') AND parent_object_id = OBJECT_ID(N'[dbo].[LayoutRow]'))
ALTER TABLE [dbo].[LayoutRow]  WITH CHECK ADD  CONSTRAINT [FK_LayoutRow_Layout] FOREIGN KEY([LayoutId])
REFERENCES [dbo].[Layout] ([LayoutId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LayoutRow_Layout]') AND parent_object_id = OBJECT_ID(N'[dbo].[LayoutRow]'))
ALTER TABLE [dbo].[LayoutRow] CHECK CONSTRAINT [FK_LayoutRow_Layout]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LayoutRow_LayoutType]') AND parent_object_id = OBJECT_ID(N'[dbo].[LayoutRow]'))
ALTER TABLE [dbo].[LayoutRow]  WITH CHECK ADD  CONSTRAINT [FK_LayoutRow_LayoutType] FOREIGN KEY([LayoutTypeId])
REFERENCES [dbo].[LayoutType] ([LayoutTypeId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LayoutRow_LayoutType]') AND parent_object_id = OBJECT_ID(N'[dbo].[LayoutRow]'))
ALTER TABLE [dbo].[LayoutRow] CHECK CONSTRAINT [FK_LayoutRow_LayoutType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetDefault_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetDefault]'))
ALTER TABLE [dbo].[WidgetDefault]  WITH CHECK ADD  CONSTRAINT [FK_WidgetDefault_Widget] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widget] ([WidgetId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetDefault_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetDefault]'))
ALTER TABLE [dbo].[WidgetDefault] CHECK CONSTRAINT [FK_WidgetDefault_Widget]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetDefaultOption_WidgetDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetDefaultOption]'))
ALTER TABLE [dbo].[WidgetDefaultOption]  WITH CHECK ADD  CONSTRAINT [FK_WidgetDefaultOption_WidgetDefault] FOREIGN KEY([WidgetDefaultId])
REFERENCES [dbo].[WidgetDefault] ([WidgetDefaultId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetDefaultOption_WidgetDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetDefaultOption]'))
ALTER TABLE [dbo].[WidgetDefaultOption] CHECK CONSTRAINT [FK_WidgetDefaultOption_WidgetDefault]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlacement_LayoutRow1]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlacement]'))
ALTER TABLE [dbo].[WidgetPlacement]  WITH CHECK ADD  CONSTRAINT [FK_WidgetPlacement_LayoutRow1] FOREIGN KEY([LayoutRowId])
REFERENCES [dbo].[LayoutRow] ([LayoutRowId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlacement_LayoutRow1]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlacement]'))
ALTER TABLE [dbo].[WidgetPlacement] CHECK CONSTRAINT [FK_WidgetPlacement_LayoutRow1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlacement_Widget1]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlacement]'))
ALTER TABLE [dbo].[WidgetPlacement]  WITH CHECK ADD  CONSTRAINT [FK_WidgetPlacement_Widget1] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widget] ([WidgetId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlacement_Widget1]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlacement]'))
ALTER TABLE [dbo].[WidgetPlacement] CHECK CONSTRAINT [FK_WidgetPlacement_Widget1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlan_Plan]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlan]'))
ALTER TABLE [dbo].[WidgetPlan]  WITH CHECK ADD  CONSTRAINT [FK_WidgetPlan_Plan] FOREIGN KEY([PlanId])
REFERENCES [dbo].[Plan] ([PlanId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlan_Plan]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlan]'))
ALTER TABLE [dbo].[WidgetPlan] CHECK CONSTRAINT [FK_WidgetPlan_Plan]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlan_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlan]'))
ALTER TABLE [dbo].[WidgetPlan]  WITH CHECK ADD  CONSTRAINT [FK_WidgetPlan_Widget] FOREIGN KEY([WidgetId])
REFERENCES [dbo].[Widget] ([WidgetId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetPlan_Widget]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetPlan]'))
ALTER TABLE [dbo].[WidgetPlan] CHECK CONSTRAINT [FK_WidgetPlan_Widget]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetSetting_WidgetDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetSetting]'))
ALTER TABLE [dbo].[WidgetSetting]  WITH CHECK ADD  CONSTRAINT [FK_WidgetSetting_WidgetDefault] FOREIGN KEY([WidgetDefaultId])
REFERENCES [dbo].[WidgetDefault] ([WidgetDefaultId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetSetting_WidgetDefault]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetSetting]'))
ALTER TABLE [dbo].[WidgetSetting] CHECK CONSTRAINT [FK_WidgetSetting_WidgetDefault]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetSetting_WidgetPlacement]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetSetting]'))
ALTER TABLE [dbo].[WidgetSetting]  WITH CHECK ADD  CONSTRAINT [FK_WidgetSetting_WidgetPlacement] FOREIGN KEY([WidgetPlacementId])
REFERENCES [dbo].[WidgetPlacement] ([WidgetPlacementId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WidgetSetting_WidgetPlacement]') AND parent_object_id = OBJECT_ID(N'[dbo].[WidgetSetting]'))
ALTER TABLE [dbo].[WidgetSetting] CHECK CONSTRAINT [FK_WidgetSetting_WidgetPlacement]
GO
/****** Object:  StoredProcedure [tool].[DeleteDashboard]    Script Date: 1/31/2020 8:33:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[tool].[DeleteDashboard]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [tool].[DeleteDashboard] AS' 
END
GO

ALTER PROCEDURE [tool].[DeleteDashboard] @DashboardId VARCHAR(36)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @WPTable TABLE ( wpId VARCHAR(36) NOT NULL );

	DECLARE @DashboardTabId VARCHAR(36)
	DECLARE @LayoutId VARCHAR(36)
	DECLARE @LayoutRowId VARCHAR(36)

	
	-- Grab the dashboard Id
	-- SELECT @DashboardId=DashboardId FROM Dashboard d WHERE d.UserId=@UserId
	
	-- Grab the dashboard tab
	SELECT @DashboardTabId=TabId FROM DashboardTab dt WHERE dt.DashboardId=@DashboardId

	-- Grab the dashboard Layout
	SELECT @LayoutId=l.LayoutId FROM Layout l WHERE l.TabId=@DashboardTabId

	-- Grab the layout's rows
	SELECT @LayoutRowId=lr.LayoutRowId FROM LayoutRow lr WHERE lr.LayoutId=@LayoutId

	INSERT INTO @WPTable
	SELECT wp.WidgetPlacementId AS wpId
	FROM WidgetPlacement wp WHERE wp.LayoutRowId=@LayoutRowId
	
	-- Widget Settings
	DELETE FROM WidgetSetting WHERE WidgetPlacementId IN (SELECT wpId FROM @WPTable)

	-- Widgets on a tab.
	DELETE FROM WidgetPlacement WHERE WidgetPlacementId IN (SELECT wpId FROM @WPTable)

	-- Remove the Layout row.
	DELETE FROM LayoutRow WHERE LayoutId=@LayoutId

	-- Remove the Layout.
	DELETE FROM Layout WHERE LayoutId=@LayoutId
	
	-- Remove the Dashboard Tab
	DELETE FROM DashboardTab WHERE TabId=@DashboardTabId

	-- Finally, remove the Dashboard
	DELETE FROM Dashboard WHERE DashboardId=@DashboardId

	-- Keep the dashboard as a placeholder?

END
/*

exec tool.DeleteDashboard 'DE207A19-63B5-4A96-9872-5DFD10A278F6'

*/
GO

-------- Seed Layout ------
PRINT 'Starting Merge for Layout...'

                
MERGE INTO Layout AS Target 
USING (VALUES 
('5267DA05-AFE4-4753-9CEE-D5D32C2B068E',NULL,1)
) 
AS Source ([LayoutId],[TabId],[LayoutIndex]) ON 
    Target.LayoutId = Source.LayoutId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [TabId] = Source.[TabId],
    [LayoutIndex] = Source.[LayoutIndex]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([LayoutId],[TabId],[LayoutIndex]) 
VALUES ([LayoutId],[TabId],[LayoutIndex])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for Layout is complete.'
PRINT ''

                
-------- Seed LayoutType ------
PRINT 'Starting Merge for LayoutType...'

                
MERGE INTO LayoutType AS Target 
USING (VALUES 
('1','Three Columns, Equal','col-4/col-4/col-4'),
('2','Three Columns, 50% Middle','col-3/col-6/col-3'),
('3','Four Columns, 25%','col-3/col-3/col-3/col-3'),
('4','Two Columns, 50%','col-6/col-6')
) 
AS Source ([LayoutTypeId],[Title],[Layout]) ON 
    Target.LayoutTypeId = Source.LayoutTypeId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [Title] = Source.[Title],
    [Layout] = Source.[Layout]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([LayoutTypeId],[Title],[Layout]) 
VALUES ([LayoutTypeId],[Title],[Layout])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for LayoutType is complete.'
PRINT ''

                
-------- Seed LayoutRow ------
PRINT 'Starting Merge for LayoutRow...'

                
MERGE INTO LayoutRow AS Target 
USING (VALUES 
('D58AFCD2-2007-4FD0-87A9-93C85C667F3F','5267DA05-AFE4-4753-9CEE-D5D32C2B068E','4',0)
) 
AS Source ([LayoutRowId],[LayoutId],[LayoutTypeId],[RowIndex]) ON 
    Target.LayoutRowId = Source.LayoutRowId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [LayoutId] = Source.[LayoutId],
    [LayoutTypeId] = Source.[LayoutTypeId],
    [RowIndex] = Source.[RowIndex]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([LayoutRowId],[LayoutId],[LayoutTypeId],[RowIndex]) 
VALUES ([LayoutRowId],[LayoutId],[LayoutTypeId],[RowIndex])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for LayoutRow is complete.'
PRINT ''

                
-------- Seed DashboardDefault ------
PRINT 'Starting Merge for DashboardDefault...'

                
MERGE INTO DashboardDefault AS Target 
USING (VALUES 
('0D96A18E-90B8-4A9F-9DF1-126653D68FE6','5267DA05-AFE4-4753-9CEE-D5D32C2B068E',NULL)
) 
AS Source ([DefaultId],[LayoutId],[PlanId]) ON 
    Target.DefaultId = Source.DefaultId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [LayoutId] = Source.[LayoutId],
    [PlanId] = Source.[PlanId]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([DefaultId],[LayoutId],[PlanId]) 
VALUES ([DefaultId],[LayoutId],[PlanId])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for DashboardDefault is complete.'
PRINT ''

                
-------- Seed Widget ------
PRINT 'Starting Merge for Widget...'

                
MERGE INTO Widget AS Target 
USING (VALUES 
('1885170C-7C48-4557-ABC7-BC06D3FC51EE','generalinfo','General Info','Display General Information','','',0,0,0,0,0),
('C9A9DB53-14CA-4551-87E7-F9656F39A396','helloworld','Hello World','A Simple Hello World Widget','','',0,1,1,1,1),
('EE84443B-7EE7-4754-BB3C-313CC0DA6039','table','Sample Table','Demonstration of data table','','',0,1,1,1,1)
) 
AS Source ([WidgetId],[Name],[Title],[Description],[ImageUrl],[GroupName],[Permission],[Moveable],[CanDelete],[UseSettings],[UseTemplate]) ON 
    Target.WidgetId = Source.WidgetId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [Name] = Source.[Name],
    [Title] = Source.[Title],
    [Description] = Source.[Description],
    [ImageUrl] = Source.[ImageUrl],
    [GroupName] = Source.[GroupName],
    [Permission] = Source.[Permission],
    [Moveable] = Source.[Moveable],
    [CanDelete] = Source.[CanDelete],
    [UseSettings] = Source.[UseSettings],
    [UseTemplate] = Source.[UseTemplate]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([WidgetId],[Name],[Title],[Description],[ImageUrl],[GroupName],[Permission],[Moveable],[CanDelete],[UseSettings],[UseTemplate]) 
VALUES ([WidgetId],[Name],[Title],[Description],[ImageUrl],[GroupName],[Permission],[Moveable],[CanDelete],[UseSettings],[UseTemplate])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for Widget is complete.'
PRINT ''

                
-------- Seed WidgetDefault ------
PRINT 'Starting Merge for WidgetDefault...'

                
MERGE INTO WidgetDefault AS Target 
USING (VALUES 
('046F4AA8-5E45-4C86-B2F8-CBF3E42647E7','EE84443B-7EE7-4754-BB3C-313CC0DA6039','widgettitle','Title','0','Sample Table',1),
('5C85537A-1319-48ED-A475-83D3DC3E7A8D','C9A9DB53-14CA-4551-87E7-F9656F39A396','widgettitle','Title','0','Projects',1)
) 
AS Source ([WidgetDefaultId],[WidgetId],[SettingName],[SettingTitle],[SettingType],[DefaultValue],[SettingIndex]) ON 
    Target.WidgetDefaultId = Source.WidgetDefaultId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [WidgetId] = Source.[WidgetId],
    [SettingName] = Source.[SettingName],
    [SettingTitle] = Source.[SettingTitle],
    [SettingType] = Source.[SettingType],
    [DefaultValue] = Source.[DefaultValue],
    [SettingIndex] = Source.[SettingIndex]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([WidgetDefaultId],[WidgetId],[SettingName],[SettingTitle],[SettingType],[DefaultValue],[SettingIndex]) 
VALUES ([WidgetDefaultId],[WidgetId],[SettingName],[SettingTitle],[SettingType],[DefaultValue],[SettingIndex])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for WidgetDefault is complete.'
PRINT ''

                
-------- Seed DashboardDefaultWidget ------
PRINT 'Starting Merge for DashboardDefaultWidget...'

                
MERGE INTO DashboardDefaultWidget AS Target 
USING (VALUES 
('D21E94CF-86A9-4058-BB72-F269728AC8AD','0D96A18E-90B8-4A9F-9DF1-126653D68FE6','D58AFCD2-2007-4FD0-87A9-93C85C667F3F','C9A9DB53-14CA-4551-87E7-F9656F39A396',0,0)
) 
AS Source ([DefaultWidgetId],[DashboardDefaultId],[LayoutRowId],[WidgetId],[ColumnIndex],[WidgetIndex]) ON 
    Target.DefaultWidgetId = Source.DefaultWidgetId
-- Update Matched Rows
WHEN MATCHED THEN 
UPDATE SET 
    [DashboardDefaultId] = Source.[DashboardDefaultId],
    [LayoutRowId] = Source.[LayoutRowId],
    [WidgetId] = Source.[WidgetId],
    [ColumnIndex] = Source.[ColumnIndex],
    [WidgetIndex] = Source.[WidgetIndex]
-- Insert new Rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([DefaultWidgetId],[DashboardDefaultId],[LayoutRowId],[WidgetId],[ColumnIndex],[WidgetIndex]) 
VALUES ([DefaultWidgetId],[DashboardDefaultId],[LayoutRowId],[WidgetId],[ColumnIndex],[WidgetIndex])

-- Delete Rows that are in target, but not in source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO


                
PRINT 'Merging for DashboardDefaultWidget is complete.'
PRINT ''

                