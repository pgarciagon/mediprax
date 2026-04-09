using System;
using System.Collections.Generic;
using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2Milestones23to35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentSuicidalityRisk",
                table: "patients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianContact",
                table: "patients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianName",
                table: "patients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianScope",
                table: "patients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasLegalGuardian",
                table: "patients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuicidalityRiskUpdatedAt",
                table: "patients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "medications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepotIntervalDays",
                table: "medications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDepot",
                table: "medications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastDepotDate",
                table: "medications",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonitoringType",
                table: "medications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresMonitoring",
                table: "medications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TargetDose",
                table: "medications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitrationSchedule",
                table: "medications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentSeriesId",
                table: "appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVideoConsultation",
                table: "appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VideoConsentGiven",
                table: "appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VideoLink",
                table: "appointments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "appointment_series",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TherapyCaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecurrencePattern = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 50),
                    SeriesStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SeriesEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appointment_series_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appointment_series_therapy_cases_TherapyCaseId",
                        column: x => x.TherapyCaseId,
                        principalTable: "therapy_cases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appointment_series_users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "btm_prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescribedById = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Pzn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Substance = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Dosierung = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BtmRecipeNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrescriberBtmNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsEBtm = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_btm_prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_btm_prescriptions_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_btm_prescriptions_users_PrescribedById",
                        column: x => x.PrescribedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmp_enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DmpType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EnrollmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DisenrollmentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dmp_enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dmp_enrollments_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "headache_diaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    DurationHours = table.Column<decimal>(type: "numeric", nullable: true),
                    Intensity = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    AuraPresent = table.Column<bool>(type: "boolean", nullable: false),
                    Nausea = table.Column<bool>(type: "boolean", nullable: false),
                    Photophobia = table.Column<bool>(type: "boolean", nullable: false),
                    Phonophobia = table.Column<bool>(type: "boolean", nullable: false),
                    Triggers = table.Column<string>(type: "jsonb", nullable: true),
                    MedicationTaken = table.Column<string>(type: "text", nullable: true),
                    MedicationEffective = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_headache_diaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_headache_diaries_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lab_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderedById = table.Column<Guid>(type: "uuid", nullable: true),
                    LabName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ResultDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Values = table.Column<List<LabValue>>(type: "jsonb", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImportSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lab_results_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lab_results_users_OrderedById",
                        column: x => x.OrderedById,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ms_documentations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EdssScore = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    IsRelapse = table.Column<bool>(type: "boolean", nullable: false),
                    RelapseDescription = table.Column<string>(type: "text", nullable: true),
                    MriDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MriFindings = table.Column<string>(type: "text", nullable: true),
                    NewLesions = table.Column<int>(type: "integer", nullable: true),
                    GadEnhancing = table.Column<int>(type: "integer", nullable: true),
                    CurrentDmt = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ms_documentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ms_documentations_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parkinson_documentations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HoehnYahrStage = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: false),
                    Tremor = table.Column<int>(type: "integer", nullable: false),
                    Rigidity = table.Column<int>(type: "integer", nullable: false),
                    Bradykinesia = table.Column<int>(type: "integer", nullable: false),
                    PosturalInstability = table.Column<int>(type: "integer", nullable: false),
                    OnOffFluctuations = table.Column<bool>(type: "boolean", nullable: false),
                    Dyskinesia = table.Column<bool>(type: "boolean", nullable: false),
                    FreezeOfGait = table.Column<bool>(type: "boolean", nullable: false),
                    NonMotorSymptoms = table.Column<string>(type: "jsonb", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parkinson_documentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parkinson_documentations_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "private_invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Items = table.Column<List<InvoiceItem>>(type: "jsonb", nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalGross = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PaidDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PdfData = table.Column<byte[]>(type: "bytea", nullable: true),
                    ReminderCount = table.Column<int>(type: "integer", nullable: false),
                    LastReminderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_private_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_private_invoices_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seizure_diaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeizureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SeizureType = table.Column<string>(type: "text", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    Trigger = table.Column<string>(type: "text", nullable: true),
                    AuraPresent = table.Column<bool>(type: "boolean", nullable: false),
                    AuraDescription = table.Column<string>(type: "text", nullable: true),
                    ConsciousnessImpaired = table.Column<bool>(type: "boolean", nullable: false),
                    PostictalState = table.Column<string>(type: "text", nullable: true),
                    MedicationAtTime = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seizure_diaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seizure_diaries_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suicidality_assessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    SuicidalIdeation = table.Column<bool>(type: "boolean", nullable: false),
                    SuicidalPlans = table.Column<bool>(type: "boolean", nullable: false),
                    SuicidalIntent = table.Column<bool>(type: "boolean", nullable: false),
                    PriorAttempts = table.Column<bool>(type: "boolean", nullable: false),
                    PriorAttemptsDetails = table.Column<string>(type: "text", nullable: true),
                    RiskFactors = table.Column<string>(type: "jsonb", nullable: false),
                    ProtectiveFactors = table.Column<string>(type: "jsonb", nullable: false),
                    SafetyPlan = table.Column<string>(type: "text", nullable: true),
                    ActionsTaken = table.Column<string>(type: "jsonb", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suicidality_assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_suicidality_assessments_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suicidality_assessments_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suicidality_assessments_users_AssessedById",
                        column: x => x.AssessedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "text_modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Shortcut = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_text_modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_text_modules_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "waitlist_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferredTherapistId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TherapyTypeRequested = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    PreferredDays = table.Column<string>(type: "jsonb", nullable: true),
                    PreferredTimeSlot = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OfferedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waitlist_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_waitlist_entries_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_waitlist_entries_users_PreferredTherapistId",
                        column: x => x.PreferredTherapistId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "dmp_documentations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DmpEnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Quarter = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FormData = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    Submitted = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dmp_documentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dmp_documentations_dmp_enrollments_DmpEnrollmentId",
                        column: x => x.DmpEnrollmentId,
                        principalTable: "dmp_enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_AppointmentSeriesId",
                table: "appointments",
                column: "AppointmentSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_series_DoctorId",
                table: "appointment_series",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_series_PatientId",
                table: "appointment_series",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_series_TherapyCaseId",
                table: "appointment_series",
                column: "TherapyCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_btm_prescriptions_PatientId",
                table: "btm_prescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_btm_prescriptions_PrescribedById",
                table: "btm_prescriptions",
                column: "PrescribedById");

            migrationBuilder.CreateIndex(
                name: "IX_btm_prescriptions_PrescriptionDate",
                table: "btm_prescriptions",
                column: "PrescriptionDate");

            migrationBuilder.CreateIndex(
                name: "IX_btm_prescriptions_Substance",
                table: "btm_prescriptions",
                column: "Substance");

            migrationBuilder.CreateIndex(
                name: "IX_dmp_documentations_DmpEnrollmentId",
                table: "dmp_documentations",
                column: "DmpEnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_dmp_documentations_Quarter",
                table: "dmp_documentations",
                column: "Quarter");

            migrationBuilder.CreateIndex(
                name: "IX_dmp_enrollments_PatientId",
                table: "dmp_enrollments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_dmp_enrollments_Status",
                table: "dmp_enrollments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_headache_diaries_Date",
                table: "headache_diaries",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_headache_diaries_PatientId",
                table: "headache_diaries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_lab_results_OrderedById",
                table: "lab_results",
                column: "OrderedById");

            migrationBuilder.CreateIndex(
                name: "IX_lab_results_PatientId_OrderDate",
                table: "lab_results",
                columns: new[] { "PatientId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_lab_results_Status",
                table: "lab_results",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ms_documentations_PatientId",
                table: "ms_documentations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_parkinson_documentations_PatientId",
                table: "parkinson_documentations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_private_invoices_InvoiceNumber",
                table: "private_invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_private_invoices_PatientId",
                table: "private_invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_private_invoices_Status",
                table: "private_invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_seizure_diaries_PatientId",
                table: "seizure_diaries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_seizure_diaries_SeizureDate",
                table: "seizure_diaries",
                column: "SeizureDate");

            migrationBuilder.CreateIndex(
                name: "IX_suicidality_assessments_AssessedById",
                table: "suicidality_assessments",
                column: "AssessedById");

            migrationBuilder.CreateIndex(
                name: "IX_suicidality_assessments_EncounterId",
                table: "suicidality_assessments",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_suicidality_assessments_PatientId",
                table: "suicidality_assessments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_text_modules_Category",
                table: "text_modules",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_text_modules_CreatedById",
                table: "text_modules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_text_modules_Shortcut",
                table: "text_modules",
                column: "Shortcut");

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_PatientId",
                table: "waitlist_entries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_PreferredTherapistId",
                table: "waitlist_entries",
                column: "PreferredTherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_Status",
                table: "waitlist_entries",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_appointment_series_AppointmentSeriesId",
                table: "appointments",
                column: "AppointmentSeriesId",
                principalTable: "appointment_series",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_appointment_series_AppointmentSeriesId",
                table: "appointments");

            migrationBuilder.DropTable(
                name: "appointment_series");

            migrationBuilder.DropTable(
                name: "btm_prescriptions");

            migrationBuilder.DropTable(
                name: "dmp_documentations");

            migrationBuilder.DropTable(
                name: "headache_diaries");

            migrationBuilder.DropTable(
                name: "lab_results");

            migrationBuilder.DropTable(
                name: "ms_documentations");

            migrationBuilder.DropTable(
                name: "parkinson_documentations");

            migrationBuilder.DropTable(
                name: "private_invoices");

            migrationBuilder.DropTable(
                name: "seizure_diaries");

            migrationBuilder.DropTable(
                name: "suicidality_assessments");

            migrationBuilder.DropTable(
                name: "text_modules");

            migrationBuilder.DropTable(
                name: "waitlist_entries");

            migrationBuilder.DropTable(
                name: "dmp_enrollments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_AppointmentSeriesId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "CurrentSuicidalityRisk",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "GuardianContact",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "GuardianName",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "GuardianScope",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "HasLegalGuardian",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "SuicidalityRiskUpdatedAt",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "DepotIntervalDays",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "IsDepot",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "LastDepotDate",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "MonitoringType",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "RequiresMonitoring",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "TargetDose",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "TitrationSchedule",
                table: "medications");

            migrationBuilder.DropColumn(
                name: "AppointmentSeriesId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "IsVideoConsultation",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "VideoConsentGiven",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "VideoLink",
                table: "appointments");
        }
    }
}
