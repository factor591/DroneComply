using DroneComply.Core.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DroneComply.Data.Context;

public class DroneComplyDbContext : DbContext
{
    static DroneComplyDbContext()
    {
        Batteries_V2.Init();
    }

    public DroneComplyDbContext(DbContextOptions<DroneComplyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<ComplianceReport> ComplianceReports => Set<ComplianceReport>();
    public DbSet<ComplianceViolation> ComplianceViolations => Set<ComplianceViolation>();
    public DbSet<FlightLog> FlightLogs => Set<FlightLog>();
    public DbSet<FlightLogEvent> FlightLogEvents => Set<FlightLogEvent>();
    public DbSet<FlightDocument> FlightDocuments => Set<FlightDocument>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
    public DbSet<MissionPlan> MissionPlans => Set<MissionPlan>();
    public DbSet<MissionChecklistItem> MissionChecklistItems => Set<MissionChecklistItem>();
    public DbSet<MissionAirspaceAdvisory> MissionAirspaceAdvisories => Set<MissionAirspaceAdvisory>();
    public DbSet<Pilot> Pilots => Set<Pilot>();
    public DbSet<Waiver> Waivers => Set<Waiver>();
    public DbSet<WaiverCondition> WaiverConditions => Set<WaiverCondition>();
    public DbSet<WeatherBriefing> WeatherBriefings => Set<WeatherBriefing>();
    public DbSet<WeatherCondition> WeatherConditions => Set<WeatherCondition>();
    public DbSet<WeatherAlert> WeatherAlerts => Set<WeatherAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAircraft(modelBuilder);
        ConfigurePilot(modelBuilder);
        ConfigureMissionPlan(modelBuilder);
        ConfigureWeather(modelBuilder);
        ConfigureFlightLog(modelBuilder);
        ConfigureMaintenance(modelBuilder);
        ConfigureWaiver(modelBuilder);
        ConfigureCompliance(modelBuilder);
    }

    private static void ConfigureAircraft(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
            entity.Property(e => e.Model).HasMaxLength(200);
            entity.Property(e => e.SerialNumber).HasMaxLength(200);

            entity.OwnsMany(e => e.Equipment, equipmentBuilder =>
            {
                equipmentBuilder.ToTable("AircraftEquipment");
                equipmentBuilder.WithOwner().HasForeignKey("AircraftId");
                equipmentBuilder.HasKey(e => e.Id);
                equipmentBuilder.Property(e => e.Name).IsRequired().HasMaxLength(200);
                equipmentBuilder.Property(e => e.Manufacturer).HasMaxLength(200);
                equipmentBuilder.Property(e => e.Version).HasMaxLength(100);
            });
        });
    }

    private static void ConfigurePilot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pilot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.CertificateNumber).HasMaxLength(100);

            entity.OwnsMany(e => e.Certifications, certificationBuilder =>
            {
                certificationBuilder.ToTable("PilotCertifications");
                certificationBuilder.WithOwner().HasForeignKey("PilotId");
                certificationBuilder.HasKey(e => e.Id);
                certificationBuilder.Property(e => e.Description).HasMaxLength(400);
            });

            entity.OwnsMany(e => e.Credentials, credentialBuilder =>
            {
                credentialBuilder.ToTable("PilotCredentials");
                credentialBuilder.WithOwner().HasForeignKey("PilotId");
                credentialBuilder.HasKey(e => e.Id);
                credentialBuilder.Property(e => e.Name).HasMaxLength(200);
                credentialBuilder.Property(e => e.Issuer).HasMaxLength(200);
            });
        });
    }

    private static void ConfigureMissionPlan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MissionPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LaunchLocation).HasMaxLength(300);
            entity.Property(e => e.LandingLocation).HasMaxLength(300);
            entity.Property(e => e.MissionObjective).HasMaxLength(1000);

            entity.HasMany(e => e.Checklist)
                .WithOne()
                .HasForeignKey(e => e.MissionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AirspaceAdvisories)
                .WithOne()
                .HasForeignKey(e => e.MissionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.WeatherBriefing)
                .WithOne()
                .HasForeignKey<WeatherBriefing>(e => e.MissionPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MissionChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(400);
        });

        modelBuilder.Entity<MissionAirspaceAdvisory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdvisoryId).HasMaxLength(100);
            entity.Property(e => e.AdvisoryType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Severity).HasMaxLength(100);
        });
    }

    private static void ConfigureWeather(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherBriefing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(2000);

            entity.HasMany(e => e.Conditions)
                .WithOne()
                .HasForeignKey(e => e.WeatherBriefingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Alerts)
                .WithOne()
                .HasForeignKey(e => e.WeatherBriefingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WeatherCondition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.WeatherPhenomena).HasMaxLength(400);
        });

        modelBuilder.Entity<WeatherAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Severity).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });
    }

    private static void ConfigureFlightLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlightLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).HasMaxLength(300);
            entity.Property(e => e.AirspaceClassification).HasMaxLength(100);
            entity.Property(e => e.MissionSummary).HasMaxLength(1000);

            entity.HasMany(e => e.Events)
                .WithOne()
                .HasForeignKey(e => e.FlightLogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Attachments)
                .WithOne()
                .HasForeignKey(e => e.FlightLogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlightLogEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Severity).HasMaxLength(100);
        });

        modelBuilder.Entity<FlightDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.StoragePath).HasMaxLength(400);
        });
    }

    private static void ConfigureMaintenance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasMany(e => e.Tasks)
                .WithOne()
                .HasForeignKey(e => e.MaintenanceRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MaintenanceTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(400);
        });
    }

    private static void ConfigureWaiver(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Waiver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasMany(e => e.Conditions)
                .WithOne()
                .HasForeignKey(e => e.WaiverId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WaiverCondition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(400);
        });
    }

    private static void ConfigureCompliance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComplianceReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.HasMany(e => e.Violations)
                .WithOne()
                .HasForeignKey(e => e.ComplianceReportId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ComplianceViolation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Recommendation).HasMaxLength(1000);
        });
    }
}
