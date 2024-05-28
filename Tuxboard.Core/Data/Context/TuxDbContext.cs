using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;

public class TuxDbContext : DbContext, ITuxDbContext
{
    private IConfiguration _config;

    public TuxDbContext(DbContextOptions<TuxDbContext> options, IConfiguration config)
        : base(options)
    {
            _config = config;
        }

    public virtual DbSet<Dashboard> Dashboard { get; set; }
    public virtual DbSet<DashboardDefault> DashboardDefault { get; set; }
    public virtual DbSet<DashboardDefaultWidget> DashboardDefaultWidget { get; set; }
    public virtual DbSet<DashboardTab> DashboardTab { get; set; }
    public virtual DbSet<Layout> Layout { get; set; }
    public virtual DbSet<LayoutRow> LayoutRow { get; set; }
    public virtual DbSet<LayoutType> LayoutType { get; set; }
    public virtual DbSet<Plan> Plan { get; set; }
    public virtual DbSet<Widget> Widget { get; set; }
    public virtual DbSet<WidgetDefault> WidgetDefault { get; set; }
    public virtual DbSet<WidgetDefaultOption> WidgetDefaultOption { get; set; }
    public virtual DbSet<WidgetPlacement> WidgetPlacement { get; set; }
    public virtual DbSet<WidgetPlan> WidgetPlan { get; set; }
    public virtual DbSet<WidgetSetting> WidgetSetting { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            base.OnModelCreating(modelBuilder);

            var tuxboardConfig = _config.GetSection(nameof(TuxboardConfig));

            if (tuxboardConfig.Exists())
            {
                var schema = string.IsNullOrEmpty(tuxboardConfig[nameof(TuxboardConfig.Schema)]) 
                    ? "dbo" 
                    : tuxboardConfig[nameof(TuxboardConfig.Schema)];
                modelBuilder.HasDefaultSchema(schema);
            }

            modelBuilder.Entity<Dashboard>(entity =>
            {
                entity.Property(e => e.DashboardId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DashboardDefault>(entity =>
            {
                entity.HasKey(e => e.DefaultId);

                entity.Property(e => e.DefaultId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LayoutId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.Layout)
                    .WithMany(p => p.DashboardDefaults)
                    .HasForeignKey(d => d.LayoutId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardDefault_Layout");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.DashboardDefaults)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK_DashboardDefault_Plan");
            });

            modelBuilder.Entity<DashboardDefaultWidget>(entity =>
            {
                entity.HasKey(e => e.DefaultWidgetId);

                entity.Property(e => e.DefaultWidgetId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DashboardDefaultId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.LayoutRowId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.WidgetId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.DashboardDefault)
                    .WithMany(p => p.DashboardDefaultWidgets)
                    .HasForeignKey(d => d.DashboardDefaultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardDefaultWidget_DashboardDefault");

                entity.HasOne(d => d.LayoutRow)
                    .WithMany(p => p.DashboardDefaultWidgets)
                    .HasForeignKey(d => d.LayoutRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardDefaultWidget_LayoutRow");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.DashboardDefaultWidgets)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardDefaultWidget_Widget");
            });

            modelBuilder.Entity<DashboardTab>(entity =>
            {
                entity.HasKey(e => e.TabId);

                entity.Property(e => e.TabId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DashboardId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.TabTitle)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dashboard)
                    .WithMany(p => p.Tabs)
                    .HasForeignKey(d => d.DashboardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardTab_Dashboard");
            });

            modelBuilder.Entity<Layout>(entity =>
            {
                entity.Property(e => e.LayoutId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.TabId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.Tab)
                    .WithMany(p => p.Layouts)
                    .HasForeignKey(d => d.TabId)
                    .HasConstraintName("FK_DashboardLayout_DashboardTab");
            });

            modelBuilder.Entity<LayoutRow>(entity =>
            {
                entity.Property(e => e.LayoutRowId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LayoutId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.LayoutTypeId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.Layout)
                    .WithMany(p => p.LayoutRows)
                    .HasForeignKey(d => d.LayoutId)
                    .HasConstraintName("FK_LayoutRow_Layout");

                entity.HasOne(d => d.LayoutType)
                    .WithMany(p => p.LayoutRows)
                    .HasForeignKey(d => d.LayoutTypeId)
                    .HasConstraintName("FK_LayoutRow_LayoutType");
            });

            modelBuilder.Entity<LayoutType>(entity =>
            {
                entity.Property(e => e.LayoutTypeId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Layout)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Widget>(entity =>
            {
                entity.Property(e => e.WidgetId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WidgetDefault>(entity =>
            {
                entity.Property(e => e.WidgetDefaultId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DefaultValue)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.SettingName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SettingTitle)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WidgetId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.WidgetDefaults)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetDefault_Widget");
            });

            modelBuilder.Entity<WidgetDefaultOption>(entity =>
            {
                entity.HasKey(e => e.WidgetOptionId)
                    .HasName("PK_WidgetSettingOption");

                entity.Property(e => e.WidgetOptionId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.SettingLabel)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SettingValue)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.WidgetDefaultId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.WidgetDefault)
                    .WithMany(p => p.WidgetDefaultOptions)
                    .HasForeignKey(d => d.WidgetDefaultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetDefaultOption_WidgetDefault");
            });

            modelBuilder.Entity<WidgetPlacement>(entity =>
            {
                entity.Property(e => e.WidgetPlacementId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LayoutRowId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.WidgetId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.LayoutRow)
                    .WithMany(p => p.WidgetPlacements)
                    .HasForeignKey(d => d.LayoutRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetPlacement_LayoutRow1");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.WidgetPlacements)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetPlacement_Widget1");
            });

            modelBuilder.Entity<WidgetPlan>(entity =>
            {
                entity.HasKey(e => new { e.WidgetId, e.PlanId });

                entity.Property(e => e.WidgetId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.WidgetPlans)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetPlan_Plan");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.WidgetPlans)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetPlan_Widget");
            });

            modelBuilder.Entity<WidgetSetting>(entity =>
            {
                entity.Property(e => e.WidgetSettingId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.WidgetDefaultId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.WidgetPlacementId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne(d => d.WidgetDefault)
                    .WithMany(p => p.WidgetSettings)
                    .HasForeignKey(d => d.WidgetDefaultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetSetting_WidgetDefault");

                entity.HasOne(d => d.WidgetPlacement)
                    .WithMany(p => p.WidgetSettings)
                    .HasForeignKey(d => d.WidgetPlacementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetSetting_WidgetPlacement");
            });

            SeedData(modelBuilder);
        }

    private void SeedData(ModelBuilder modelBuilder)
    {
            // Test Data
            modelBuilder.Entity<Layout>()
                .HasData(
                    new List<Layout>
                    {
                        new() {LayoutId = "5267DA05-AFE4-4753-9CEE-D5D32C2B068E", TabId = null, LayoutIndex = 1}
                    }
                );

            modelBuilder.Entity<LayoutType>()
                .HasData(
                    new List<LayoutType>
                    {
                        new() {LayoutTypeId = "1", Title = "Three Columns, Equal", Layout = "col-4/col-4/col-4"},
                        new() {LayoutTypeId = "2", Title = "Three Columns, 50% Middle", Layout = "col-3/col-6/col-3"},
                        new() {LayoutTypeId = "3", Title = "Four Columns, 25%", Layout = "col-3/col-3/col-3/col-3"},
                        new() {LayoutTypeId = "4", Title = "Two Columns, 50%", Layout = "col-6/col-6"}
                    }
                );

            modelBuilder.Entity<LayoutRow>()
                .HasData(
                    new List<LayoutRow>
                    {
                        new()
                        {
                            LayoutRowId = "D58AFCD2-2007-4FD0-87A9-93C85C667F3F",
                            LayoutId = "5267DA05-AFE4-4753-9CEE-D5D32C2B068E",
                            LayoutTypeId = "4",
                            RowIndex = 0
                        }
                    }
                );

            modelBuilder.Entity<DashboardDefault>()
                .HasData(
                    new List<DashboardDefault>
                    {
                        new()
                        {
                            DefaultId = "0D96A18E-90B8-4A9F-9DF1-126653D68FE6",
                            LayoutId = "5267DA05-AFE4-4753-9CEE-D5D32C2B068E",
                            PlanId = null
                        }
                    }
                );

            modelBuilder.Entity<Widget>()
                .HasData(
                    new List<Widget>
                    {
                        new()
                        {
                            WidgetId = "1885170C-7C48-4557-ABC7-BC06D3FC51EE",
                            Name = "generalinfo",
                            Title = "General Info",
                            Description = "Display General Information",
                            ImageUrl = "", GroupName = "", Permission = 0, Moveable = false, CanDelete = false,
                            UseSettings = false, UseTemplate = false
                        },
                        new()
                        {
                            WidgetId = "C9A9DB53-14CA-4551-87E7-F9656F39A396",
                            Name = "helloworld",
                            Title = "Hello World",
                            Description = "A Simple Hello World Widget",
                            ImageUrl = "", GroupName = "", Permission = 0, Moveable = true, CanDelete = true,
                            UseSettings = true, UseTemplate = true
                        },
                        new()
                        {
                            WidgetId = "EE84443B-7EE7-4754-BB3C-313CC0DA6039",
                            Name = "table",
                            Title = "Sample Table",
                            Description = "Demonstration of data table",
                            ImageUrl = "", GroupName = "", Permission = 0, Moveable = true, CanDelete = true,
                            UseSettings = true, UseTemplate = true
                        }
                    }
                );

            modelBuilder.Entity<WidgetDefault>()
                .HasData(
                    new List<WidgetDefault>
                    {
                        new()
                        {
                            WidgetDefaultId = "046F4AA8-5E45-4C86-B2F8-CBF3E42647E7",
                            WidgetId = "EE84443B-7EE7-4754-BB3C-313CC0DA6039",
                            SettingName = "widgettitle",
                            SettingTitle = "Title",
                            DefaultValue = "Sample Table",
                            SettingIndex = 1
                        },
                        new()
                        {
                            WidgetDefaultId = "5C85537A-1319-48ED-A475-83D3DC3E7A8D",
                            WidgetId = "C9A9DB53-14CA-4551-87E7-F9656F39A396",
                            SettingName = "widgettitle",
                            SettingTitle = "Title",
                            DefaultValue = "Projects",
                            SettingIndex = 1
                        }
                    }
                );

            modelBuilder.Entity<DashboardDefaultWidget>()
                .HasData(
                    new List<DashboardDefaultWidget>
                    {
                        new()
                        {
                            DefaultWidgetId = "D21E94CF-86A9-4058-BB72-F269728AC8AD",
                            DashboardDefaultId = "0D96A18E-90B8-4A9F-9DF1-126653D68FE6",
                            LayoutRowId = "D58AFCD2-2007-4FD0-87A9-93C85C667F3F",
                            WidgetId = "C9A9DB53-14CA-4551-87E7-F9656F39A396",
                            ColumnIndex = 0,
                            WidgetIndex = 0
                        }
                    }
                );
        }
}