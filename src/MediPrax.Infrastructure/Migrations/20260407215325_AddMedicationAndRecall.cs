using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationAndRecall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescribedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Wirkstoff = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Pzn = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Staerke = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Dosierung = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Einheit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Hinweis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SeitDatum = table.Column<DateOnly>(type: "date", nullable: false),
                    BisDatum = table.Column<DateOnly>(type: "date", nullable: true),
                    IsBtm = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medications_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_medications_users_PrescribedById",
                        column: x => x.PrescribedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recalls_patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recalls_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medications_PatientId_IsActive",
                table: "medications",
                columns: new[] { "PatientId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_medications_PrescribedById",
                table: "medications",
                column: "PrescribedById");

            migrationBuilder.CreateIndex(
                name: "IX_recalls_CreatedById",
                table: "recalls",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_recalls_DueDate_Status",
                table: "recalls",
                columns: new[] { "DueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_recalls_PatientId",
                table: "recalls",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medications");

            migrationBuilder.DropTable(
                name: "recalls");
        }
    }
}
