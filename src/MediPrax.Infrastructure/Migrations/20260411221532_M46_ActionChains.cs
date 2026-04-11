using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class M46_ActionChains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "action_chains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Shortcut = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_chains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_action_chains_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "action_chain_steps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ActionChainId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Configuration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_chain_steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_action_chain_steps_action_chains_ActionChainId",
                        column: x => x.ActionChainId,
                        principalTable: "action_chains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_action_chain_steps_ActionChainId_SortOrder",
                table: "action_chain_steps",
                columns: new[] { "ActionChainId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_action_chains_Category",
                table: "action_chains",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_action_chains_CreatedById",
                table: "action_chains",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_action_chains_IsActive",
                table: "action_chains",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_action_chains_Shortcut",
                table: "action_chains",
                column: "Shortcut",
                unique: true);

            // Seed default action chains — use first Arzt user as creator
            migrationBuilder.Sql(@"
DO $seed$
DECLARE
    v_user uuid;
    v_now timestamptz := now();
    v_chain uuid;
BEGIN
    SELECT ""Id"" INTO v_user FROM users WHERE ""Role"" = 1 LIMIT 1;
    IF v_user IS NULL THEN
        SELECT ""Id"" INTO v_user FROM users LIMIT 1;
    END IF;

    -- #dep: Depression Standard
    v_chain := 'a0000001-0000-0000-0000-000000000001';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'dep', 'Depression Standard', 'F32.1 + Grundpauschale + psychiatrische Befundvorlage', 'Psychiatrie', v_user, true, 1, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""F32.1"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""21220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""psych""}', v_now, v_now, false);

    -- #angst: Angststoerung
    v_chain := 'a0000001-0000-0000-0000-000000000002';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'angst', 'Angststoerung', 'F41.1 + Grundpauschale + psychiatrische Befundvorlage', 'Psychiatrie', v_user, true, 2, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""F41.1"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""21220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""psych""}', v_now, v_now, false);

    -- #epi: Epilepsie Kontrolle
    v_chain := 'a0000001-0000-0000-0000-000000000003';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'epi', 'Epilepsie Kontrolle', 'G40.9 + neurologische Grundpauschale + EEG', 'Neurologie', v_user, true, 3, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""G40.9"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""16220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 2, '{""gopCode"":""16311"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 3, '{""template"":""neuro""}', v_now, v_now, false);

    -- #erstgespraech: Psychiatrisches Erstgespraech
    v_chain := 'a0000001-0000-0000-0000-000000000004';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'erstgespraech', 'Psychiatrisches Erstgespraech', 'Grundpauschale + psychiatrisches Gespraech + vollstaendige Vorlage', 'Psychiatrie', v_user, true, 4, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 1, 0, '{""gopCode"":""21210"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""21220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""psych""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 5, 3, '{""durationMinutes"":50}', v_now, v_now, false);

    -- #schmerz: Chronischer Schmerz
    v_chain := 'a0000001-0000-0000-0000-000000000005';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'schmerz', 'Chronischer Schmerz', 'R52.2 + neurologische Grundpauschale', 'Neurologie', v_user, true, 5, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""R52.2"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""16220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""neuro""}', v_now, v_now, false);

    -- #demenz: Demenz Kontrolle
    v_chain := 'a0000001-0000-0000-0000-000000000006';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'demenz', 'Demenz Kontrolle', 'F00.1 + Grundpauschale + Wiedervorlage 6 Monate', 'Psychiatrie', v_user, true, 6, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""F00.1"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""21220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""psych""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 7, 3, '{""reason"":""Demenz-Kontrolle"",""daysFromNow"":180}', v_now, v_now, false);

    -- #park: Parkinson Kontrolle
    v_chain := 'a0000001-0000-0000-0000-000000000007';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'park', 'Parkinson Kontrolle', 'G20 + neurologische Grundpauschale', 'Neurologie', v_user, true, 7, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""G20"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""16220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""neuro""}', v_now, v_now, false);

    -- #ms: Multiple Sklerose
    v_chain := 'a0000001-0000-0000-0000-000000000008';
    INSERT INTO action_chains (""Id"",""Shortcut"",""Title"",""Description"",""Category"",""CreatedById"",""IsGlobal"",""SortOrder"",""IsActive"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES (v_chain, 'ms', 'Multiple Sklerose', 'G35 + neurologische Grundpauschale + Wiedervorlage 3 Monate', 'Neurologie', v_user, true, 8, true, v_now, v_now, false);
    INSERT INTO action_chain_steps (""Id"",""ActionChainId"",""StepType"",""SortOrder"",""Configuration"",""CreatedAt"",""UpdatedAt"",""IsDeleted"") VALUES
        (gen_random_uuid(), v_chain, 0, 0, '{""icd10Code"":""G35"",""certainty"":""G"",""diagnosisType"":""Encounterdiagnose""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 1, 1, '{""gopCode"":""16220"",""quantity"":1}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 2, 2, '{""template"":""neuro""}', v_now, v_now, false),
        (gen_random_uuid(), v_chain, 7, 3, '{""reason"":""MS-Kontrolle"",""daysFromNow"":90}', v_now, v_now, false);
END $seed$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action_chain_steps");

            migrationBuilder.DropTable(
                name: "action_chains");
        }
    }
}
