using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNeurologicalExamination : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "neurological_examinations");
        }
    }
}
