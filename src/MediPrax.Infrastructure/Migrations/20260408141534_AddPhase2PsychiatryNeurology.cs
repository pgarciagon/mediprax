using System;
using System.Collections.Generic;
using MediPrax.Core.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase2PsychiatryNeurology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "neurological_examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExaminedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NarrativeText = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Coordination = table.Column<string>(type: "jsonb", nullable: false),
                    CranialNerves = table.Column<string>(type: "jsonb", nullable: false),
                    Gait = table.Column<string>(type: "jsonb", nullable: false),
                    MeningealSigns = table.Column<string>(type: "jsonb", nullable: false),
                    MotorSystem = table.Column<string>(type: "jsonb", nullable: false),
                    Reflexes = table.Column<string>(type: "jsonb", nullable: false),
                    SensorySystem = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_neurological_examinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_neurological_examinations_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_neurological_examinations_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_neurological_examinations_users_ExaminedById",
                        column: x => x.ExaminedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "psychometric_tests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdministeredById = table.Column<Guid>(type: "uuid", nullable: false),
                    TestType = table.Column<int>(type: "integer", nullable: false),
                    TestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Responses = table.Column<List<TestResponse>>(type: "jsonb", nullable: false),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    Interpretation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_psychometric_tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_psychometric_tests_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_psychometric_tests_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_psychometric_tests_users_AdministeredById",
                        column: x => x.AdministeredById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "psychopathological_findings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Findings = table.Column<List<SymptomFinding>>(type: "jsonb", nullable: false),
                    NarrativeText = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_psychopathological_findings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_psychopathological_findings_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_psychopathological_findings_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_psychopathological_findings_users_AssessedById",
                        column: x => x.AssessedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "therapy_cases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    TherapistId = table.Column<Guid>(type: "uuid", nullable: false),
                    TherapyType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InsuranceApprovalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InsuranceApprovalRef = table.Column<string>(type: "text", nullable: true),
                    ApprovedSessions = table.Column<int>(type: "integer", nullable: false),
                    CompletedSessions = table.Column<int>(type: "integer", nullable: false),
                    SessionDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsGroupTherapy = table.Column<bool>(type: "boolean", nullable: false),
                    GutachterStatus = table.Column<int>(type: "integer", nullable: true),
                    Diagnoses = table.Column<string>(type: "jsonb", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_therapy_cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_therapy_cases_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_therapy_cases_users_TherapistId",
                        column: x => x.TherapistId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ptv_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TherapyCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormType = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FormData = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    PdfData = table.Column<byte[]>(type: "bytea", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptv_forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptv_forms_therapy_cases_TherapyCaseId",
                        column: x => x.TherapyCaseId,
                        principalTable: "therapy_cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "therapy_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TherapyCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionNumber = table.Column<int>(type: "integer", nullable: false),
                    SessionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    SessionType = table.Column<int>(type: "integer", nullable: false),
                    IsVideoSession = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    BilledGop = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_therapy_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_therapy_sessions_appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "appointments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_therapy_sessions_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_therapy_sessions_therapy_cases_TherapyCaseId",
                        column: x => x.TherapyCaseId,
                        principalTable: "therapy_cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_neurological_examinations_EncounterId",
                table: "neurological_examinations",
                column: "EncounterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_neurological_examinations_ExaminedById",
                table: "neurological_examinations",
                column: "ExaminedById");

            migrationBuilder.CreateIndex(
                name: "IX_neurological_examinations_PatientId_ExamDate",
                table: "neurological_examinations",
                columns: new[] { "PatientId", "ExamDate" });

            migrationBuilder.CreateIndex(
                name: "IX_psychometric_tests_AdministeredById",
                table: "psychometric_tests",
                column: "AdministeredById");

            migrationBuilder.CreateIndex(
                name: "IX_psychometric_tests_EncounterId",
                table: "psychometric_tests",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_psychometric_tests_PatientId_TestType_TestDate",
                table: "psychometric_tests",
                columns: new[] { "PatientId", "TestType", "TestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_psychopathological_findings_AssessedById",
                table: "psychopathological_findings",
                column: "AssessedById");

            migrationBuilder.CreateIndex(
                name: "IX_psychopathological_findings_EncounterId",
                table: "psychopathological_findings",
                column: "EncounterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_psychopathological_findings_PatientId_AssessmentDate",
                table: "psychopathological_findings",
                columns: new[] { "PatientId", "AssessmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ptv_forms_TherapyCaseId_FormType",
                table: "ptv_forms",
                columns: new[] { "TherapyCaseId", "FormType" });

            migrationBuilder.CreateIndex(
                name: "IX_therapy_cases_PatientId",
                table: "therapy_cases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_cases_Status",
                table: "therapy_cases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_cases_TherapistId",
                table: "therapy_cases",
                column: "TherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_AppointmentId",
                table: "therapy_sessions",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_EncounterId",
                table: "therapy_sessions",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_SessionDate",
                table: "therapy_sessions",
                column: "SessionDate");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_TherapyCaseId_SessionNumber",
                table: "therapy_sessions",
                columns: new[] { "TherapyCaseId", "SessionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "neurological_examinations");

            migrationBuilder.DropTable(
                name: "psychometric_tests");

            migrationBuilder.DropTable(
                name: "psychopathological_findings");

            migrationBuilder.DropTable(
                name: "ptv_forms");

            migrationBuilder.DropTable(
                name: "therapy_sessions");

            migrationBuilder.DropTable(
                name: "therapy_cases");
        }
    }
}
