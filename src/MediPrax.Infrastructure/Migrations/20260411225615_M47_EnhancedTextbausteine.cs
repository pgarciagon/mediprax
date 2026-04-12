using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class M47_EnhancedTextbausteine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicTitle",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "text_modules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetSection",
                table: "text_modules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_text_modules_TargetSection",
                table: "text_modules",
                column: "TargetSection");

            // Update seed user with AcademicTitle
            migrationBuilder.Sql(@"
UPDATE users SET ""AcademicTitle"" = 'Dr. med.' WHERE ""Role"" = 1 AND ""AcademicTitle"" IS NULL;
");

            // Seed default Textbausteine with fixed GUIDs
            migrationBuilder.Sql(@"
DO $seed$
DECLARE
    v_user uuid;
    v_now timestamptz := now();
BEGIN
    SELECT ""Id"" INTO v_user FROM users WHERE ""Role"" = 1 LIMIT 1;
    IF v_user IS NULL THEN
        SELECT ""Id"" INTO v_user FROM users LIMIT 1;
    END IF;

    -- Psychiatrie/Befund: Normaler psychopathologischer Befund
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000001', 'normpsy', 'Normaler psychopathologischer Befund',
    'Bewusstsein klar, allseits orientiert. Auffassung, Konzentration und Gedaechtnis ungestoert. Formales Denken geordnet, keine inhaltlichen Denkstoerrungen. Stimmung ausgeglichen, Affekt modulationsfaehig und situationsadaequat. Antrieb und Psychomotorik unauffaellig. Keine Ich-Stoerungen. Keine Suizidalitaet.',
    'Psychiatrie/Befund', true, 0, 1, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Psychiatrie/Befund: Depressiver Befund
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000002', 'depbefund', 'Depressiver Befund',
    'Bewusstsein klar, allseits orientiert. Stimmung deutlich gedrueckt, Affekt eingeschraenkt modulationsfaehig. Antrieb vermindert. Konzentration leicht beeintraechtigt. Gruebelneigung ohne formale Denkstoerrungen. Keine Wahnsymptome, keine Halluzinationen. Schlafstoerrungen berichtet. Appetit vermindert. Keine akute Suizidalitaet, Absprachefaehigkeit gegeben.',
    'Psychiatrie/Befund', true, 0, 1, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Neurologie/Befund: Normaler neurologischer Befund
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000003', 'normneuro', 'Normaler neurologischer Befund',
    'Hirnnerven I-XII intakt. Keine Paresen, Muskeltonus und -trophik unauffaellig. Eigenreflexe seitengleich mittellebhaft ausloeosbar. Keine pathologischen Reflexe. Koordination intakt (Finger-Nase-Versuch, Knie-Hacke-Versuch). Sensibilitaet intakt. Gang- und Standbild unauffaellig, Romberg negativ.',
    'Neurologie/Befund', true, 0, 1, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Psychiatrie/Anamnese: Erstgespraech Anamnese
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000004', 'erstanamnese', 'Erstgespraech Anamnese',
    'Vorstellungsgrund:
Aktuelle Beschwerden:
Beginn und Verlauf:
Bisherige Behandlung:
Psychiatrische Vorgeschichte:
Somatische Anamnese:
Familienanamnese:
Sozialanamnese: Beruf, Familienstand, Wohnsituation
Substanzanamnese: Nikotin, Alkohol, Drogen',
    'Psychiatrie/Anamnese', true, 0, 0, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Therapie: Standard Therapie
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000005', 'therplan', 'Therapieplan Standard',
    'Medikamentoese Therapie: Fortfuehrung der aktuellen Medikation.
Psychotherapeutische Massnahmen: Stuetzende Gespraeche.
Psychoedukation: Aufklaerung ueber Erkrankung und Behandlung.
Weitere Massnahmen: ',
    'Allgemein/Therapie', true, 0, 3, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Procedere: Wiedervorstellung
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000006', 'wv', 'Wiedervorstellung',
    'Wiedervorstellung in     Wochen zur Verlaufskontrolle.
Bei Verschlechterung fruehere Vorstellung.
Laborkontrolle:     .
Naechster Termin: ',
    'Allgemein/Procedere', true, 0, 4, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Arztbrief: Brief Einleitung
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000007', 'briefein', 'Arztbrief Einleitung',
    'Sehr geehrte Kolleginnen und Kollegen,

wir berichten ueber o.g. Patienten, {Patient.Name}, geb. am {Patient.Geburtsdatum} ({Patient.Alter} Jahre), der sich am {Datum} in unserer Sprechstunde vorstellte.',
    'Allgemein/Arztbrief', true, 0, null, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Arztbrief: Brief Schluss
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000008', 'briefschluss', 'Arztbrief Schluss',
    'Fuer Rueckfragen stehen wir Ihnen gerne zur Verfuegung.

Mit freundlichen kollegialen Gruessen

{Arzt.Titel} {Arzt.Name}',
    'Allgemein/Arztbrief', true, 0, null, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Diagnose: Diagnosen mit Dauerdiagnosen
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000009', 'diagliste', 'Diagnosenliste',
    'Dauerdiagnosen:
{Dauerdiagnosen}

Aktuelle Diagnosen:
{Diagnosen}',
    'Allgemein/Diagnose', true, 0, 2, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Allgemein/Therapie: Aktuelle Medikation
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000010', 'medliste', 'Aktuelle Medikation',
    'Aktuelle Medikation:
{Medikation}',
    'Allgemein/Therapie', true, 0, 3, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Psychiatrie/Befund: Kurzkonsultation
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000011', 'kurz', 'Kurzkonsultation',
    'Patient berichtet ueber stabilen Verlauf. Keine neuen Beschwerden. Medikation wird gut vertragen. Psychopathologisch unveraendert zum Vorbefund.',
    'Psychiatrie/Kurzkonsultation', true, 0, 1, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

    -- Neurologie/Anamnese: Kopfschmerz Anamnese
    INSERT INTO text_modules (""Id"",""Shortcut"",""Title"",""Content"",""Category"",""IsGlobal"",""UsageCount"",""TargetSection"",""CreatedById"",""CreatedAt"",""UpdatedAt"",""IsDeleted"")
    VALUES ('b0000047-0001-0000-0000-000000000012', 'ksanamnese', 'Kopfschmerz-Anamnese',
    'Kopfschmerzcharakter:
Lokalisation:
Intensitaet (NRS 0-10):
Dauer:
Haeufigkeit:
Begleitsymptome: Uebelkeit, Erbrechen, Photophobie, Phonophobie
Aura:
Trigger:
Bisherige Therapie:
Kopfschmerztagebuch: ',
    'Neurologie/Anamnese', true, 0, 0, v_user, v_now, v_now, false)
    ON CONFLICT (""Id"") DO NOTHING;

END;
$seed$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_text_modules_TargetSection",
                table: "text_modules");

            migrationBuilder.DropColumn(
                name: "AcademicTitle",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "text_modules");

            migrationBuilder.DropColumn(
                name: "TargetSection",
                table: "text_modules");
        }
    }
}
