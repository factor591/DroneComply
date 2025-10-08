using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using DroneComply.Core.Enums;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DroneComply.Data.Seeding;

public sealed class DbSeeder : IDbSeeder
{
    private static readonly Guid PilotAveryId = Guid.Parse("158B66FD-58A2-4A68-8FCE-01700F492E4D");
    private static readonly Guid PilotMiaId = Guid.Parse("9FB3E670-537C-4B1D-961E-2F6225B49AFD");
    private static readonly Guid AircraftMatriceId = Guid.Parse("26E151C4-64FC-4C75-8486-9639A61A1E2C");
    private static readonly Guid AircraftAnafiId = Guid.Parse("77866C12-D7A8-4F34-9A3F-4B93A40B4886");

    private readonly DroneComplyDbContext _dbContext;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(DroneComplyDbContext dbContext, ILogger<DbSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedPilotsAsync(cancellationToken).ConfigureAwait(false);
        await SeedAircraftAsync(cancellationToken).ConfigureAwait(false);
        await SeedMissionPlansAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedPilotsAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Pilots.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        var pilots = new[]
        {
            new Pilot
            {
                Id = PilotAveryId,
                FirstName = "Avery",
                LastName = "Johnson",
                Email = "avery.johnson@skyreachuas.com",
                PhoneNumber = "555-0111",
                CertificateNumber = "RP-4501234",
                CertificationDate = utcNow.AddYears(-3),
                RecurrentTrainingDue = utcNow.AddMonths(8),
                Certifications = new List<PilotCertification>
                {
                    new()
                    {
                        Type = CertificationType.Part107,
                        Description = "FAA Part 107 Remote Pilot Certificate",
                        IssuedOn = utcNow.AddYears(-3),
                        ExpiresOn = utcNow.AddMonths(8)
                    },
                    new()
                    {
                        Type = CertificationType.NightOperationsTraining,
                        Description = "Night operations training waiver completed",
                        IssuedOn = utcNow.AddYears(-1)
                    }
                },
                Credentials = new List<PilotCredential>
                {
                    new()
                    {
                        Name = "NIST UAS Proficiency Level 2",
                        Issuer = "SkyReach UAS Training",
                        IssuedOn = utcNow.AddMonths(-10)
                    },
                    new()
                    {
                        Name = "Thermal Imaging Endorsement",
                        Issuer = "InfraSight Academy",
                        IssuedOn = utcNow.AddMonths(-16),
                        ExpiresOn = utcNow.AddMonths(8)
                    }
                }
            },
            new Pilot
            {
                Id = PilotMiaId,
                FirstName = "Mia",
                LastName = "Sanders",
                Email = "mia.sanders@skyreachuas.com",
                PhoneNumber = "555-0112",
                CertificateNumber = "RP-4789021",
                CertificationDate = utcNow.AddYears(-4),
                RecurrentTrainingDue = utcNow.AddDays(45),
                Certifications = new List<PilotCertification>
                {
                    new()
                    {
                        Type = CertificationType.Part107,
                        Description = "FAA Part 107 Remote Pilot Certificate",
                        IssuedOn = utcNow.AddYears(-4),
                        ExpiresOn = utcNow.AddDays(45)
                    },
                    new()
                    {
                        Type = CertificationType.WaiverBVLOS,
                        Description = "BVLOS waiver team lead",
                        IssuedOn = utcNow.AddYears(-2)
                    }
                },
                Credentials = new List<PilotCredential>
                {
                    new()
                    {
                        Name = "Public Safety Tactical Training",
                        Issuer = "Northwest Public Safety UAS Alliance",
                        IssuedOn = utcNow.AddMonths(-20)
                    }
                }
            }
        };

        await _dbContext.Pilots.AddRangeAsync(pilots, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Seeded {Count} pilot profiles.", pilots.Length);
    }

    private async Task SeedAircraftAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Aircraft.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        var aircraft = new[]
        {
            new Aircraft
            {
                Id = AircraftMatriceId,
                Name = "DJI Matrice 300 RTK",
                Manufacturer = "DJI",
                Model = "M300 RTK",
                SerialNumber = "DJI-M300-0001",
                Type = AircraftType.Multirotor,
                MaxTakeoffWeightKg = 9.0,
                MaxFlightTimeMinutes = 45,
                Status = AircraftStatus.Active,
                CommissionedOn = utcNow.AddYears(-1).AddMonths(-4),
                LastMaintenance = utcNow.AddDays(-30),
                Equipment = new ObservableCollection<AircraftEquipment>
                {
                    new()
                    {
                        Name = "Zenmuse H20T",
                        Manufacturer = "DJI",
                        Version = "v02.02.01",
                        InstalledOn = utcNow.AddYears(-1),
                        LastServicedOn = utcNow.AddMonths(-2)
                    },
                    new()
                    {
                        Name = "RTK Module",
                        Manufacturer = "DJI",
                        Version = "v01.00.01",
                        InstalledOn = utcNow.AddYears(-1).AddMonths(-3),
                        LastServicedOn = utcNow.AddMonths(-4)
                    }
                }
            },
            new Aircraft
            {
                Id = AircraftAnafiId,
                Name = "Parrot Anafi Ai",
                Manufacturer = "Parrot",
                Model = "Anafi Ai",
                SerialNumber = "PAR-ANAFI-3021",
                Type = AircraftType.Multirotor,
                MaxTakeoffWeightKg = 1.98,
                MaxFlightTimeMinutes = 32,
                Status = AircraftStatus.Active,
                CommissionedOn = utcNow.AddYears(-2),
                LastMaintenance = utcNow.AddDays(-75),
                Equipment = new ObservableCollection<AircraftEquipment>
                {
                    new()
                    {
                        Name = "32x Optical Zoom Gimbal",
                        Manufacturer = "Parrot",
                        Version = "v03.04",
                        InstalledOn = utcNow.AddYears(-2),
                        LastServicedOn = utcNow.AddMonths(-6)
                    },
                    new()
                    {
                        Name = "Pix4D RTK Module",
                        Manufacturer = "Parrot",
                        Version = "v02.01",
                        InstalledOn = utcNow.AddYears(-1).AddMonths(-6),
                        LastServicedOn = utcNow.AddMonths(-5)
                    }
                }
            }
        };

        await _dbContext.Aircraft.AddRangeAsync(aircraft, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Seeded {Count} aircraft records.", aircraft.Length);
    }

    private async Task SeedMissionPlansAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.MissionPlans.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        var missionPlans = new[]
        {
            CreateDowntownInspectionPlan(utcNow),
            CreateBridgeInspectionPlan(utcNow)
        };

        await _dbContext.MissionPlans.AddRangeAsync(missionPlans, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Seeded {Count} mission plans.", missionPlans.Length);
    }

    private static MissionPlan CreateDowntownInspectionPlan(DateTime utcNow)
    {
        return new MissionPlan
        {
            Id = Guid.Parse("B2A6D821-5418-433C-BDFD-74DDE016048C"),
            Name = "Downtown Rooftop Inspection",
            PilotId = PilotAveryId,
            AircraftId = AircraftMatriceId,
            PlannedDate = utcNow.AddDays(3).Date.AddHours(16),
            LaunchLocation = "SkyReach Ops Center - Rooftop Pad A",
            LandingLocation = "SkyReach Ops Center - Rooftop Pad A",
            MissionObjective = "Capture high-resolution imagery for structural envelope assessment.",
            Status = MissionStatus.Approved,
            RiskLevel = FlightRiskLevel.Moderate,
            Checklist = new List<MissionChecklistItem>
            {
                new()
                {
                    Description = "Confirm LAANC authorization approved",
                    IsRequired = true
                },
                new()
                {
                    Description = "Verify thermal payload calibration",
                    IsRequired = true
                },
                new()
                {
                    Description = "Review rooftop access plan with facilities manager",
                    IsRequired = false
                }
            },
            AirspaceAdvisories = new List<MissionAirspaceAdvisory>
            {
                new()
                {
                    AdvisoryId = "KBFI-20250929",
                    AdvisoryType = "LAANC",
                    Description = "Class B shelf 0-200 ft AGL authorization required.",
                    EffectiveFrom = utcNow.AddDays(2),
                    EffectiveTo = utcNow.AddDays(3).AddHours(6),
                    Severity = "Moderate"
                }
            },
            WeatherBriefing = new WeatherBriefing
            {
                Source = "FAA AWC METAR",
                Summary = "VFR with light southerly winds and ceilings above 5000 ft.",
                RetrievedAt = utcNow,
                Conditions = new List<WeatherCondition>
                {
                    new()
                    {
                        Location = "KBFI",
                        ObservationTime = utcNow,
                        TemperatureCelsius = 22,
                        WindDirectionDegrees = 180,
                        WindSpeedKnots = 6,
                        WindGustKnots = 10,
                        VisibilityMiles = 10,
                        CeilingFeet = 5500,
                        WeatherPhenomena = "Clear"
                    }
                },
                Alerts = new List<WeatherAlert>
                {
                    new()
                    {
                        Title = "Gusty winds after 2200Z",
                        Severity = "Low",
                        Description = "Light gusts up to 15 kt possible near sunset.",
                        Effective = utcNow.AddHours(-1),
                        Expires = utcNow.AddHours(4)
                    }
                }
            }
        };
    }

    private static MissionPlan CreateBridgeInspectionPlan(DateTime utcNow)
    {
        return new MissionPlan
        {
            Id = Guid.Parse("4E7F6367-96BC-4105-A1FC-C76999F6B4A0"),
            Name = "Riverfront Bridge Deck Survey",
            PilotId = PilotMiaId,
            AircraftId = AircraftAnafiId,
            PlannedDate = utcNow.AddDays(7).Date.AddHours(14),
            LaunchLocation = "Riverfront Park staging area",
            LandingLocation = "Riverfront Park staging area",
            MissionObjective = "Generate photogrammetry model to assess deck spalling.",
            Status = MissionStatus.Draft,
            RiskLevel = FlightRiskLevel.Low,
            Checklist = new List<MissionChecklistItem>
            {
                new()
                {
                    Description = "Verify bridge closure permit on file",
                    IsRequired = true
                },
                new()
                {
                    Description = "Calibrate RTK base station",
                    IsRequired = true
                }
            },
            AirspaceAdvisories = new List<MissionAirspaceAdvisory>
            {
                new()
                {
                    AdvisoryId = "NOTAM-20250902",
                    AdvisoryType = "NOTAM",
                    Description = "Nearby heliport traffic between 1800-2200Z.",
                    EffectiveFrom = utcNow.AddDays(7).AddHours(-2),
                    EffectiveTo = utcNow.AddDays(7).AddHours(2),
                    Severity = "Low"
                }
            },
            WeatherBriefing = new WeatherBriefing
            {
                Source = "Synthetic Forecast",
                Summary = "Partly cloudy skies with light west winds.",
                RetrievedAt = utcNow,
                Conditions = new List<WeatherCondition>
                {
                    new()
                    {
                        Location = "KRNT",
                        ObservationTime = utcNow,
                        TemperatureCelsius = 18,
                        WindDirectionDegrees = 260,
                        WindSpeedKnots = 4,
                        WindGustKnots = 0,
                        VisibilityMiles = 8,
                        CeilingFeet = 6500,
                        WeatherPhenomena = "Few clouds"
                    }
                }
            }
        };
    }
}



