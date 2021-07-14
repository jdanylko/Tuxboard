using Microsoft.EntityFrameworkCore.Migrations;

namespace UserDashboard.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dashboard");

            migrationBuilder.CreateTable(
                name: "Dashboard",
                schema: "dashboard",
                columns: table => new
                {
                    DashboardId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    SelectedTab = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.DashboardId);
                });

            migrationBuilder.CreateTable(
                name: "LayoutType",
                schema: "dashboard",
                columns: table => new
                {
                    LayoutTypeId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Layout = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutType", x => x.LayoutTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                schema: "dashboard",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "Widget",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    GroupName = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false),
                    Moveable = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UseSettings = table.Column<bool>(type: "bit", nullable: false),
                    UseTemplate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.WidgetId);
                });

            migrationBuilder.CreateTable(
                name: "DashboardTab",
                schema: "dashboard",
                columns: table => new
                {
                    TabId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    DashboardId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    TabTitle = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    TabIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardTab", x => x.TabId);
                    table.ForeignKey(
                        name: "FK_DashboardTab_Dashboard",
                        column: x => x.DashboardId,
                        principalSchema: "dashboard",
                        principalTable: "Dashboard",
                        principalColumn: "DashboardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetDefault",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetDefaultId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    WidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    SettingName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SettingTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    SettingType = table.Column<short>(type: "smallint", nullable: false),
                    DefaultValue = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    SettingIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetDefault", x => x.WidgetDefaultId);
                    table.ForeignKey(
                        name: "FK_WidgetDefault_Widget",
                        column: x => x.WidgetId,
                        principalSchema: "dashboard",
                        principalTable: "Widget",
                        principalColumn: "WidgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetPlan",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetPlan", x => new { x.WidgetId, x.PlanId });
                    table.ForeignKey(
                        name: "FK_WidgetPlan_Plan",
                        column: x => x.PlanId,
                        principalSchema: "dashboard",
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WidgetPlan_Widget",
                        column: x => x.WidgetId,
                        principalSchema: "dashboard",
                        principalTable: "Widget",
                        principalColumn: "WidgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Layout",
                schema: "dashboard",
                columns: table => new
                {
                    LayoutId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    TabId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    LayoutIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layout", x => x.LayoutId);
                    table.ForeignKey(
                        name: "FK_DashboardLayout_DashboardTab",
                        column: x => x.TabId,
                        principalSchema: "dashboard",
                        principalTable: "DashboardTab",
                        principalColumn: "TabId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetDefaultOption",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetOptionId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    WidgetDefaultId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    SettingLabel = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    SettingValue = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    SettingIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetSettingOption", x => x.WidgetOptionId);
                    table.ForeignKey(
                        name: "FK_WidgetDefaultOption_WidgetDefault",
                        column: x => x.WidgetDefaultId,
                        principalSchema: "dashboard",
                        principalTable: "WidgetDefault",
                        principalColumn: "WidgetDefaultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardDefault",
                schema: "dashboard",
                columns: table => new
                {
                    DefaultId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    LayoutId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardDefault", x => x.DefaultId);
                    table.ForeignKey(
                        name: "FK_DashboardDefault_Layout",
                        column: x => x.LayoutId,
                        principalSchema: "dashboard",
                        principalTable: "Layout",
                        principalColumn: "LayoutId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DashboardDefault_Plan",
                        column: x => x.PlanId,
                        principalSchema: "dashboard",
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LayoutRow",
                schema: "dashboard",
                columns: table => new
                {
                    LayoutRowId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    LayoutId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    LayoutTypeId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutRow", x => x.LayoutRowId);
                    table.ForeignKey(
                        name: "FK_LayoutRow_Layout",
                        column: x => x.LayoutId,
                        principalSchema: "dashboard",
                        principalTable: "Layout",
                        principalColumn: "LayoutId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LayoutRow_LayoutType",
                        column: x => x.LayoutTypeId,
                        principalSchema: "dashboard",
                        principalTable: "LayoutType",
                        principalColumn: "LayoutTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardDefaultWidget",
                schema: "dashboard",
                columns: table => new
                {
                    DefaultWidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    DashboardDefaultId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    LayoutRowId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    WidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    ColumnIndex = table.Column<int>(type: "int", nullable: false),
                    WidgetIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardDefaultWidget", x => x.DefaultWidgetId);
                    table.ForeignKey(
                        name: "FK_DashboardDefaultWidget_DashboardDefault",
                        column: x => x.DashboardDefaultId,
                        principalSchema: "dashboard",
                        principalTable: "DashboardDefault",
                        principalColumn: "DefaultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DashboardDefaultWidget_LayoutRow",
                        column: x => x.LayoutRowId,
                        principalSchema: "dashboard",
                        principalTable: "LayoutRow",
                        principalColumn: "LayoutRowId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DashboardDefaultWidget_Widget",
                        column: x => x.WidgetId,
                        principalSchema: "dashboard",
                        principalTable: "Widget",
                        principalColumn: "WidgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetPlacement",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetPlacementId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    LayoutRowId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    WidgetId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    ColumnIndex = table.Column<int>(type: "int", nullable: false),
                    WidgetIndex = table.Column<int>(type: "int", nullable: false),
                    Collapsed = table.Column<bool>(type: "bit", nullable: false),
                    UseSettings = table.Column<bool>(type: "bit", nullable: false),
                    UseTemplate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetPlacement", x => x.WidgetPlacementId);
                    table.ForeignKey(
                        name: "FK_WidgetPlacement_LayoutRow1",
                        column: x => x.LayoutRowId,
                        principalSchema: "dashboard",
                        principalTable: "LayoutRow",
                        principalColumn: "LayoutRowId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WidgetPlacement_Widget1",
                        column: x => x.WidgetId,
                        principalSchema: "dashboard",
                        principalTable: "Widget",
                        principalColumn: "WidgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetSetting",
                schema: "dashboard",
                columns: table => new
                {
                    WidgetSettingId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "(newid())"),
                    WidgetPlacementId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    WidgetDefaultId = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    Value = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetSetting", x => x.WidgetSettingId);
                    table.ForeignKey(
                        name: "FK_WidgetSetting_WidgetDefault",
                        column: x => x.WidgetDefaultId,
                        principalSchema: "dashboard",
                        principalTable: "WidgetDefault",
                        principalColumn: "WidgetDefaultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WidgetSetting_WidgetPlacement",
                        column: x => x.WidgetPlacementId,
                        principalSchema: "dashboard",
                        principalTable: "WidgetPlacement",
                        principalColumn: "WidgetPlacementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "Layout",
                columns: new[] { "LayoutId", "LayoutIndex", "TabId" },
                values: new object[] { "5267DA05-AFE4-4753-9CEE-D5D32C2B068E", 1, null });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "LayoutType",
                columns: new[] { "LayoutTypeId", "Layout", "Title" },
                values: new object[,]
                {
                    { "1", "col-4/col-4/col-4", "Three Columns, Equal" },
                    { "2", "col-3/col-6/col-3", "Three Columns, 50% Middle" },
                    { "3", "col-3/col-3/col-3/col-3", "Four Columns, 25%" },
                    { "4", "col-6/col-6", "Two Columns, 50%" }
                });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "Widget",
                columns: new[] { "WidgetId", "CanDelete", "Description", "GroupName", "ImageUrl", "Moveable", "Name", "Permission", "Title", "UseSettings", "UseTemplate" },
                values: new object[,]
                {
                    { "1885170C-7C48-4557-ABC7-BC06D3FC51EE", false, "Display General Information", "", "", false, "generalinfo", 0, "General Info", false, false },
                    { "C9A9DB53-14CA-4551-87E7-F9656F39A396", true, "A Simple Hello World Widget", "", "", true, "helloworld", 0, "Hello World", true, true },
                    { "EE84443B-7EE7-4754-BB3C-313CC0DA6039", true, "Demonstration of data table", "", "", true, "table", 0, "Sample Table", true, true }
                });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "DashboardDefault",
                columns: new[] { "DefaultId", "LayoutId", "PlanId" },
                values: new object[] { "0D96A18E-90B8-4A9F-9DF1-126653D68FE6", "5267DA05-AFE4-4753-9CEE-D5D32C2B068E", null });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "LayoutRow",
                columns: new[] { "LayoutRowId", "LayoutId", "LayoutTypeId", "RowIndex" },
                values: new object[] { "D58AFCD2-2007-4FD0-87A9-93C85C667F3F", "5267DA05-AFE4-4753-9CEE-D5D32C2B068E", "4", 0 });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "WidgetDefault",
                columns: new[] { "WidgetDefaultId", "DefaultValue", "SettingIndex", "SettingName", "SettingTitle", "SettingType", "WidgetId" },
                values: new object[,]
                {
                    { "5C85537A-1319-48ED-A475-83D3DC3E7A8D", "Projects", 1, "widgettitle", "Title", (short)0, "C9A9DB53-14CA-4551-87E7-F9656F39A396" },
                    { "046F4AA8-5E45-4C86-B2F8-CBF3E42647E7", "Sample Table", 1, "widgettitle", "Title", (short)0, "EE84443B-7EE7-4754-BB3C-313CC0DA6039" }
                });

            migrationBuilder.InsertData(
                schema: "dashboard",
                table: "DashboardDefaultWidget",
                columns: new[] { "DefaultWidgetId", "ColumnIndex", "DashboardDefaultId", "LayoutRowId", "WidgetId", "WidgetIndex" },
                values: new object[] { "D21E94CF-86A9-4058-BB72-F269728AC8AD", 0, "0D96A18E-90B8-4A9F-9DF1-126653D68FE6", "D58AFCD2-2007-4FD0-87A9-93C85C667F3F", "C9A9DB53-14CA-4551-87E7-F9656F39A396", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardDefault_LayoutId",
                schema: "dashboard",
                table: "DashboardDefault",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardDefault_PlanId",
                schema: "dashboard",
                table: "DashboardDefault",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardDefaultWidget_DashboardDefaultId",
                schema: "dashboard",
                table: "DashboardDefaultWidget",
                column: "DashboardDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardDefaultWidget_LayoutRowId",
                schema: "dashboard",
                table: "DashboardDefaultWidget",
                column: "LayoutRowId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardDefaultWidget_WidgetId",
                schema: "dashboard",
                table: "DashboardDefaultWidget",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTab_DashboardId",
                schema: "dashboard",
                table: "DashboardTab",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Layout_TabId",
                schema: "dashboard",
                table: "Layout",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_LayoutRow_LayoutId",
                schema: "dashboard",
                table: "LayoutRow",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_LayoutRow_LayoutTypeId",
                schema: "dashboard",
                table: "LayoutRow",
                column: "LayoutTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetDefault_WidgetId",
                schema: "dashboard",
                table: "WidgetDefault",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetDefaultOption_WidgetDefaultId",
                schema: "dashboard",
                table: "WidgetDefaultOption",
                column: "WidgetDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetPlacement_LayoutRowId",
                schema: "dashboard",
                table: "WidgetPlacement",
                column: "LayoutRowId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetPlacement_WidgetId",
                schema: "dashboard",
                table: "WidgetPlacement",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetPlan_PlanId",
                schema: "dashboard",
                table: "WidgetPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSetting_WidgetDefaultId",
                schema: "dashboard",
                table: "WidgetSetting",
                column: "WidgetDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSetting_WidgetPlacementId",
                schema: "dashboard",
                table: "WidgetSetting",
                column: "WidgetPlacementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardDefaultWidget",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "WidgetDefaultOption",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "WidgetPlan",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "WidgetSetting",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "DashboardDefault",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "WidgetDefault",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "WidgetPlacement",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Plan",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "LayoutRow",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Widget",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Layout",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "LayoutType",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "DashboardTab",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Dashboard",
                schema: "dashboard");
        }
    }
}
