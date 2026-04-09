using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SuicidalityOptionalFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suicidality_assessments_encounters_EncounterId",
                table: "suicidality_assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_suicidality_assessments_users_AssessedById",
                table: "suicidality_assessments");

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "suicidality_assessments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessedById",
                table: "suicidality_assessments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_suicidality_assessments_encounters_EncounterId",
                table: "suicidality_assessments",
                column: "EncounterId",
                principalTable: "encounters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_suicidality_assessments_users_AssessedById",
                table: "suicidality_assessments",
                column: "AssessedById",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suicidality_assessments_encounters_EncounterId",
                table: "suicidality_assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_suicidality_assessments_users_AssessedById",
                table: "suicidality_assessments");

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "suicidality_assessments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessedById",
                table: "suicidality_assessments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_suicidality_assessments_encounters_EncounterId",
                table: "suicidality_assessments",
                column: "EncounterId",
                principalTable: "encounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_suicidality_assessments_users_AssessedById",
                table: "suicidality_assessments",
                column: "AssessedById",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
