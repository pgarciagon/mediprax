using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientDiagnosisManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patient_diagnoses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Icd10Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Icd10Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Certainty = table.Column<int>(type: "integer", nullable: false),
                    Laterality = table.Column<int>(type: "integer", nullable: true),
                    DiagnosisType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OnsetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ResolvedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedByDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_diagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_patient_diagnoses_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_patient_diagnoses_users_CreatedByDoctorId",
                        column: x => x.CreatedByDoctorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "encounter_diagnoses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientDiagnosisId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsNewInThisEncounter = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encounter_diagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_encounter_diagnoses_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_encounter_diagnoses_patient_diagnoses_PatientDiagnosisId",
                        column: x => x.PatientDiagnosisId,
                        principalTable: "patient_diagnoses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_encounter_diagnoses_EncounterId",
                table: "encounter_diagnoses",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_encounter_diagnoses_EncounterId_PatientDiagnosisId",
                table: "encounter_diagnoses",
                columns: new[] { "EncounterId", "PatientDiagnosisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_encounter_diagnoses_PatientDiagnosisId",
                table: "encounter_diagnoses",
                column: "PatientDiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_CreatedByDoctorId",
                table: "patient_diagnoses",
                column: "CreatedByDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_DiagnosisType",
                table: "patient_diagnoses",
                column: "DiagnosisType");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_PatientId",
                table: "patient_diagnoses",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_PatientId_Icd10Code",
                table: "patient_diagnoses",
                columns: new[] { "PatientId", "Icd10Code" });

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_Status",
                table: "patient_diagnoses",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "encounter_diagnoses");

            migrationBuilder.DropTable(
                name: "patient_diagnoses");
        }
    }
}
