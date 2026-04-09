using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArztbriefWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArztbriefStatus",
                table: "documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Diagnoses",
                table: "documents",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddress",
                table: "documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "documents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArztbriefStatus",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "Diagnoses",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "RecipientAddress",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "documents");
        }
    }
}
