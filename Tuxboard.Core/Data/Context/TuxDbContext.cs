using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context
{
    public partial class TuxDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public TuxDbContext(DbContextOptions<TuxDbContext> options, IConfiguration config)
            : base(options)
        {
            _configuration = config;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            var appConfig = new TuxboardConfig();
            _configuration
                .GetSection(nameof(TuxboardConfig))
                .Bind(appConfig);
            optionsBuilder.UseSqlServer(appConfig.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dashboard>(entity =>
            {
                entity.Property<string>(e => e.DashboardId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.UserId)
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DashboardDefault>(entity =>
            {
                entity.HasKey(e => e.DefaultId);

                entity.Property<string>(e => e.DefaultId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.LayoutId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne<Layout>(d => d.Layout)
                    .WithMany(p => p.DashboardDefaults)
                    .HasForeignKey(d => d.LayoutId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardDefault_Layout");

                entity.HasOne<Plan>(d => d.Plan)
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

                entity.Property<string>(e => e.TabId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.DashboardId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property<string>(e => e.TabTitle)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne<Dashboard>(d => d.Dashboard)
                    .WithMany(p => p.Tabs)
                    .HasForeignKey(d => d.DashboardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardTab_Dashboard");
            });

            modelBuilder.Entity<Layout>(entity =>
            {
                entity.Property<string>(e => e.LayoutId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.TabId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne<DashboardTab>(d => d.Tab)
                    .WithMany(p => p.Layouts)
                    .HasForeignKey(d => d.TabId)
                    .HasConstraintName("FK_DashboardLayout_DashboardTab");
            });

            modelBuilder.Entity<LayoutRow>(entity =>
            {
                entity.Property<string>(e => e.LayoutRowId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.LayoutId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property<string>(e => e.LayoutTypeId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne<Layout>(d => d.Layout)
                    .WithMany(p => p.LayoutRows)
                    .HasForeignKey(d => d.LayoutId)
                    .HasConstraintName("FK_LayoutRow_Layout");

                entity.HasOne<LayoutType>(d => d.LayoutType)
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
                entity.Property<string>(e => e.WidgetId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property<string>(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property<string>(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property<string>(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property<string>(e => e.Title)
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
                entity.Property<string>(e => e.WidgetPlacementId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.LayoutRowId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property<string>(e => e.WidgetId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne<LayoutRow>(d => d.LayoutRow)
                    .WithMany(p => p.WidgetPlacements)
                    .HasForeignKey(d => d.LayoutRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetPlacement_LayoutRow1");

                entity.HasOne<Widget>(d => d.Widget)
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
                entity.Property<string>(e => e.WidgetSettingId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property<string>(e => e.Value)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property<string>(e => e.WidgetDefaultId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property<string>(e => e.WidgetPlacementId)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.HasOne<WidgetDefault>(d => d.WidgetDefault)
                    .WithMany(p => p.WidgetSettings)
                    .HasForeignKey(d => d.WidgetDefaultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetSetting_WidgetDefault");

                entity.HasOne<WidgetPlacement>(d => d.WidgetPlacement)
                    .WithMany(p => p.WidgetSettings)
                    .HasForeignKey(d => d.WidgetPlacementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WidgetSetting_WidgetPlacement");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
