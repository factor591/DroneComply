using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DroneComply.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTakeoffWeightKg = table.Column<double>(type: "REAL", nullable: false),
                    MaxFlightTimeMinutes = table.Column<double>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CommissionedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMaintenance = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MissionPlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PilotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AircraftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlightDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DistanceCoveredKm = table.Column<double>(type: "REAL", nullable: false),
                    Purpose = table.Column<int>(type: "INTEGER", nullable: false),
                    RiskAssessment = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AirspaceClassification = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MissionSummary = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AircraftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PerformedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MissionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PilotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AircraftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlannedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LaunchLocation = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    LandingLocation = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RiskLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    MissionObjective = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CertificateNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CertificationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecurrentTrainingDue = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Waivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApprovedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AircraftEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InstalledOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastServicedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AircraftId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AircraftEquipment_Aircraft_AircraftId",
                        column: x => x.AircraftId,
                        principalTable: "Aircraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceViolations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComplianceReportId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Recommendation = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceViolations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplianceViolations_ComplianceReports_ComplianceReportId",
                        column: x => x.ComplianceReportId,
                        principalTable: "ComplianceReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlightLogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightDocuments_FlightLogs_FlightLogId",
                        column: x => x.FlightLogId,
                        principalTable: "FlightLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightLogEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlightLogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightLogEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightLogEvents_FlightLogs_FlightLogId",
                        column: x => x.FlightLogId,
                        principalTable: "FlightLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaintenanceRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceTasks_MaintenanceRecords_MaintenanceRecordId",
                        column: x => x.MaintenanceRecordId,
                        principalTable: "MaintenanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionAirspaceAdvisories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MissionPlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdvisoryId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AdvisoryType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionAirspaceAdvisories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionAirspaceAdvisories_MissionPlans_MissionPlanId",
                        column: x => x.MissionPlanId,
                        principalTable: "MissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MissionPlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionChecklistItems_MissionPlans_MissionPlanId",
                        column: x => x.MissionPlanId,
                        principalTable: "MissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherBriefings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MissionPlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RetrievedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherBriefings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherBriefings_MissionPlans_MissionPlanId",
                        column: x => x.MissionPlanId,
                        principalTable: "MissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PilotCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PilotId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PilotCertifications_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PilotCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Issuer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PilotId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PilotCredentials_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaiverConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WaiverId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    IsSatisfied = table.Column<bool>(type: "INTEGER", nullable: false),
                    SatisfiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaiverConditions_Waivers_WaiverId",
                        column: x => x.WaiverId,
                        principalTable: "Waivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeatherBriefingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Effective = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Expires = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherAlerts_WeatherBriefings_WeatherBriefingId",
                        column: x => x.WeatherBriefingId,
                        principalTable: "WeatherBriefings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeatherBriefingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ObservationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TemperatureCelsius = table.Column<double>(type: "REAL", nullable: false),
                    WindSpeedKnots = table.Column<double>(type: "REAL", nullable: false),
                    WindGustKnots = table.Column<double>(type: "REAL", nullable: false),
                    WindDirectionDegrees = table.Column<int>(type: "INTEGER", nullable: false),
                    VisibilityMiles = table.Column<double>(type: "REAL", nullable: false),
                    CeilingFeet = table.Column<double>(type: "REAL", nullable: false),
                    WeatherPhenomena = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherConditions_WeatherBriefings_WeatherBriefingId",
                        column: x => x.WeatherBriefingId,
                        principalTable: "WeatherBriefings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AircraftEquipment_AircraftId",
                table: "AircraftEquipment",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceViolations_ComplianceReportId",
                table: "ComplianceViolations",
                column: "ComplianceReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightDocuments_FlightLogId",
                table: "FlightDocuments",
                column: "FlightLogId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogEvents_FlightLogId",
                table: "FlightLogEvents",
                column: "FlightLogId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_MaintenanceRecordId",
                table: "MaintenanceTasks",
                column: "MaintenanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionAirspaceAdvisories_MissionPlanId",
                table: "MissionAirspaceAdvisories",
                column: "MissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionChecklistItems_MissionPlanId",
                table: "MissionChecklistItems",
                column: "MissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PilotCertifications_PilotId",
                table: "PilotCertifications",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_PilotCredentials_PilotId",
                table: "PilotCredentials",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverConditions_WaiverId",
                table: "WaiverConditions",
                column: "WaiverId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherAlerts_WeatherBriefingId",
                table: "WeatherAlerts",
                column: "WeatherBriefingId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherBriefings_MissionPlanId",
                table: "WeatherBriefings",
                column: "MissionPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeatherConditions_WeatherBriefingId",
                table: "WeatherConditions",
                column: "WeatherBriefingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AircraftEquipment");

            migrationBuilder.DropTable(
                name: "ComplianceViolations");

            migrationBuilder.DropTable(
                name: "FlightDocuments");

            migrationBuilder.DropTable(
                name: "FlightLogEvents");

            migrationBuilder.DropTable(
                name: "MaintenanceTasks");

            migrationBuilder.DropTable(
                name: "MissionAirspaceAdvisories");

            migrationBuilder.DropTable(
                name: "MissionChecklistItems");

            migrationBuilder.DropTable(
                name: "PilotCertifications");

            migrationBuilder.DropTable(
                name: "PilotCredentials");

            migrationBuilder.DropTable(
                name: "WaiverConditions");

            migrationBuilder.DropTable(
                name: "WeatherAlerts");

            migrationBuilder.DropTable(
                name: "WeatherConditions");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "ComplianceReports");

            migrationBuilder.DropTable(
                name: "FlightLogs");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Waivers");

            migrationBuilder.DropTable(
                name: "WeatherBriefings");

            migrationBuilder.DropTable(
                name: "MissionPlans");
        }
    }
}
