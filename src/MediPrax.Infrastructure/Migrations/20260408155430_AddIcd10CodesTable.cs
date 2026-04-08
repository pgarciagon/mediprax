using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIcd10CodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "icd10_codes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icd10_codes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_icd10_codes_Category",
                table: "icd10_codes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_icd10_codes_Code",
                table: "icd10_codes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_icd10_codes_IsActive",
                table: "icd10_codes",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "icd10_codes");
        }
    }
}
