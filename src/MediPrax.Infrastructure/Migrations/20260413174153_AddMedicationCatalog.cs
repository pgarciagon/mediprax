using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPrax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medication_catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Pzn = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Handelsname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Wirkstoff = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WirkstoffAsk = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AtcCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Staerke = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Darreichungsform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Packungsgroesse = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NormPackungsgroesse = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Hersteller = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsBtm = table.Column<bool>(type: "boolean", nullable: false),
                    IsTRezeptPflichtig = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerschreibungspflichtig = table.Column<bool>(type: "boolean", nullable: false),
                    Festbetrag = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Avp = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DataSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "SEED"),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medication_catalog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_AtcCode",
                table: "medication_catalog",
                column: "AtcCode");

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_Category",
                table: "medication_catalog",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_Handelsname",
                table: "medication_catalog",
                column: "Handelsname");

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_IsActive_Category",
                table: "medication_catalog",
                columns: new[] { "IsActive", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_Pzn",
                table: "medication_catalog",
                column: "Pzn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_medication_catalog_Wirkstoff",
                table: "medication_catalog",
                column: "Wirkstoff");

            // ── Seed: ~300 psychiatry/neurology medications ──────────────────
            migrationBuilder.Sql("""
                INSERT INTO medication_catalog ("Id","Pzn","Handelsname","Wirkstoff","AtcCode","Staerke","Darreichungsform","Packungsgroesse","NormPackungsgroesse","Hersteller","IsBtm","IsTRezeptPflichtig","IsVerschreibungspflichtig","Category","IsActive","DataSource","LastUpdated")
                VALUES
                -- ═══ ANTIDEPRESSIVA (SSRI) ═══
                ('a0000001-0001-0001-0001-000000000001','10010001','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000002','10010002','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000003','10010003','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000004','10010004','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000005','10010005','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000006','10010006','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000007','10010007','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000008','10010008','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000009','10010009','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','100 Stk','N3','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000010','10010010','Cipralex 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000011','10010011','Cipralex 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000012','10010012','Escitalopram-ratiopharm 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000013','10010013','Escitalopram-ratiopharm 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000014','10010014','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000015','10010015','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000016','10010016','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000017','10010017','Citalopram HEXAL 40mg','Citalopram','N06AB04','40 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000018','10010018','Citalopram HEXAL 40mg','Citalopram','N06AB04','40 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000019','10010019','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000020','10010020','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','50 Stk','N2','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000021','10010021','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000022','10010022','Paroxetin-1A Pharma 20mg','Paroxetin','N06AB05','20 mg','Filmtabletten','20 Stk','N1','1A Pharma',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000023','10010023','Paroxetin-1A Pharma 20mg','Paroxetin','N06AB05','20 mg','Filmtabletten','50 Stk','N2','1A Pharma',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0001-0001-0001-000000000024','10010024','Fluvoxamin-neuraxpharm 100mg','Fluvoxamin','N06AB08','100 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

                -- ═══ ANTIDEPRESSIVA (SNRI) ═══
                ('a0000001-0002-0001-0001-000000000001','10020001','Venlafaxin-ratiopharm 75mg','Venlafaxin','N06AX16','75 mg','Retardkapseln','30 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000002','10020002','Venlafaxin-ratiopharm 75mg','Venlafaxin','N06AX16','75 mg','Retardkapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000003','10020003','Venlafaxin-ratiopharm 150mg','Venlafaxin','N06AX16','150 mg','Retardkapseln','30 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000004','10020004','Venlafaxin-ratiopharm 150mg','Venlafaxin','N06AX16','150 mg','Retardkapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000005','10020005','Cymbalta 30mg','Duloxetin','N06AX21','30 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000006','10020006','Cymbalta 60mg','Duloxetin','N06AX21','60 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000007','10020007','Cymbalta 60mg','Duloxetin','N06AX21','60 mg','Kapseln','98 Stk','N3','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000008','10020008','Duloxetin Zentiva 30mg','Duloxetin','N06AX21','30 mg','Kapseln','28 Stk','N1','Zentiva',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0002-0001-0001-000000000009','10020009','Duloxetin Zentiva 60mg','Duloxetin','N06AX21','60 mg','Kapseln','28 Stk','N1','Zentiva',false,false,true,'Antidepressivum',true,'SEED',NOW()),

                -- ═══ ANTIDEPRESSIVA (Trizyklisch) ═══
                ('a0000001-0003-0001-0001-000000000001','10030001','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000002','10030002','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000003','10030003','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','100 Stk','N3','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000004','10030004','Amitriptylin-neuraxpharm 75mg','Amitriptylin','N06AA09','75 mg','Retardtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000005','10030005','Amitriptylin-neuraxpharm 75mg','Amitriptylin','N06AA09','75 mg','Retardtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000006','10030006','Clomipramin-neuraxpharm 25mg','Clomipramin','N06AA04','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000007','10030007','Clomipramin-neuraxpharm 75mg','Clomipramin','N06AA04','75 mg','Retardtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000008','10030008','Clomipramin-neuraxpharm 75mg','Clomipramin','N06AA04','75 mg','Retardtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000009','10030009','Doxepin-ratiopharm 25mg','Doxepin','N06AA12','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000010','10030010','Doxepin-ratiopharm 50mg','Doxepin','N06AA12','50 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000011','10030011','Doxepin-ratiopharm 50mg','Doxepin','N06AA12','50 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000012','10030012','Trimipramin-neuraxpharm 25mg','Trimipramin','N06AA06','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0003-0001-0001-000000000013','10030013','Trimipramin-neuraxpharm 100mg','Trimipramin','N06AA06','100 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

                -- ═══ ANTIDEPRESSIVA (Andere) ═══
                ('a0000001-0004-0001-0001-000000000001','10040001','Mirtazapin Sandoz 15mg','Mirtazapin','N06AX11','15 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000002','10040002','Mirtazapin Sandoz 15mg','Mirtazapin','N06AX11','15 mg','Schmelztabletten','96 Stk','N3','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000003','10040003','Mirtazapin Sandoz 30mg','Mirtazapin','N06AX11','30 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000004','10040004','Mirtazapin Sandoz 30mg','Mirtazapin','N06AX11','30 mg','Schmelztabletten','96 Stk','N3','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000005','10040005','Mirtazapin Sandoz 45mg','Mirtazapin','N06AX11','45 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000006','10040006','Elontril 150mg','Bupropion','N06AX12','150 mg','Retardtabletten','30 Stk','N1','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000007','10040007','Elontril 300mg','Bupropion','N06AX12','300 mg','Retardtabletten','30 Stk','N1','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000008','10040008','Elontril 300mg','Bupropion','N06AX12','300 mg','Retardtabletten','90 Stk','N3','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000009','10040009','Valdoxan 25mg','Agomelatin','N06AX22','25 mg','Filmtabletten','28 Stk','N1','Servier',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000010','10040010','Valdoxan 25mg','Agomelatin','N06AX22','25 mg','Filmtabletten','98 Stk','N3','Servier',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000011','10040011','Trazodon-neuraxpharm 100mg','Trazodon','N06AX05','100 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
                ('a0000001-0004-0001-0001-000000000012','10040012','Trazodon-neuraxpharm 100mg','Trazodon','N06AX05','100 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

                -- ═══ ANTIPSYCHOTIKA (Atypisch) ═══
                ('a0000002-0001-0001-0001-000000000001','20010001','Risperidon-ratiopharm 1mg','Risperidon','N05AX08','1 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000002','20010002','Risperidon-ratiopharm 2mg','Risperidon','N05AX08','2 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000003','20010003','Risperidon-ratiopharm 2mg','Risperidon','N05AX08','2 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000004','20010004','Risperidon-ratiopharm 4mg','Risperidon','N05AX08','4 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000005','20010005','Zyprexa 5mg','Olanzapin','N05AH03','5 mg','Filmtabletten','28 Stk','N1','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000006','20010006','Zyprexa 10mg','Olanzapin','N05AH03','10 mg','Filmtabletten','28 Stk','N1','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000007','20010007','Zyprexa 10mg','Olanzapin','N05AH03','10 mg','Filmtabletten','70 Stk','N3','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000008','20010008','Olanzapin AbZ 15mg','Olanzapin','N05AH03','15 mg','Filmtabletten','28 Stk','N1','AbZ',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000009','20010009','Seroquel 25mg','Quetiapin','N05AH04','25 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000010','20010010','Seroquel 100mg','Quetiapin','N05AH04','100 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000011','20010011','Seroquel 200mg','Quetiapin','N05AH04','200 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000012','20010012','Seroquel Prolong 300mg','Quetiapin','N05AH04','300 mg','Retardtabletten','30 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000013','20010013','Quetiapin-ratiopharm 25mg','Quetiapin','N05AH04','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000014','20010014','Quetiapin-ratiopharm 100mg','Quetiapin','N05AH04','100 mg','Filmtabletten','100 Stk','N3','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000015','20010015','Abilify 10mg','Aripiprazol','N05AX12','10 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000016','20010016','Abilify 15mg','Aripiprazol','N05AX12','15 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000017','20010017','Abilify 30mg','Aripiprazol','N05AX12','30 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000018','20010018','Aripiprazol AbZ 10mg','Aripiprazol','N05AX12','10 mg','Tabletten','28 Stk','N1','AbZ',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000019','20010019','Leponex 25mg','Clozapin','N05AH02','25 mg','Tabletten','50 Stk','N2','Novartis',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000020','20010020','Leponex 100mg','Clozapin','N05AH02','100 mg','Tabletten','50 Stk','N2','Novartis',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000021','20010021','Clozapin-neuraxpharm 25mg','Clozapin','N05AH02','25 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000022','20010022','Clozapin-neuraxpharm 100mg','Clozapin','N05AH02','100 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000023','20010023','Zeldox 40mg','Ziprasidon','N05AE04','40 mg','Kapseln','30 Stk','N1','Pfizer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000024','20010024','Zeldox 80mg','Ziprasidon','N05AE04','80 mg','Kapseln','30 Stk','N1','Pfizer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000025','20010025','Latuda 37mg','Lurasidon','N05AE05','37 mg','Filmtabletten','28 Stk','N1','Aziende Chimiche',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000026','20010026','Latuda 74mg','Lurasidon','N05AE05','74 mg','Filmtabletten','28 Stk','N1','Aziende Chimiche',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000027','20010027','Reagila 1.5mg','Cariprazin','N05AX15','1.5 mg','Kapseln','28 Stk','N1','Gedeon Richter',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0001-0001-0001-000000000028','20010028','Reagila 3mg','Cariprazin','N05AX15','3 mg','Kapseln','28 Stk','N1','Gedeon Richter',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

                -- ═══ ANTIPSYCHOTIKA (Typisch) ═══
                ('a0000002-0002-0001-0001-000000000001','20020001','Haldol 1mg','Haloperidol','N05AD01','1 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000002','20020002','Haldol 5mg','Haloperidol','N05AD01','5 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000003','20020003','Haldol Tropfen 2mg/ml','Haloperidol','N05AD01','2 mg/ml','Tropfen','30 ml','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000004','20020004','Melperon-ratiopharm 25mg','Melperon','N05AD03','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000005','20020005','Melperon-ratiopharm 100mg','Melperon','N05AD03','100 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000006','20020006','Atosil 25mg','Promethazin','N05AA02','25 mg','Filmtabletten','20 Stk','N1','Bayer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000007','20020007','Atosil 25mg','Promethazin','N05AA02','25 mg','Filmtabletten','50 Stk','N2','Bayer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000008','20020008','Dipiperon 40mg','Pipamperon','N05AD05','40 mg','Tabletten','20 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000009','20020009','Dipiperon 40mg','Pipamperon','N05AD05','40 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000010','20020010','Fluanxol 0.5mg','Flupentixol','N05AF01','0.5 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0002-0001-0001-000000000011','20020011','Fluanxol 0.5mg','Flupentixol','N05AF01','0.5 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

                -- ═══ ANTIPSYCHOTIKA (Depot) ═══
                ('a0000002-0003-0001-0001-000000000001','20030001','Xeplion 75mg','Paliperidon','N05AX13','75 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000002','20030002','Xeplion 150mg','Paliperidon','N05AX13','150 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000003','20030003','Trevicta 263mg','Paliperidon','N05AX13','263 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000004','20030004','Abilify Maintena 400mg','Aripiprazol','N05AX12','400 mg','Depot-Injektion','1 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000005','20030005','Risperdal Consta 25mg','Risperidon','N05AX08','25 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000006','20030006','Risperdal Consta 37.5mg','Risperidon','N05AX08','37.5 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
                ('a0000002-0003-0001-0001-000000000007','20030007','Fluanxol Depot 20mg/ml','Flupentixol','N05AF01','20 mg/ml','Depot-Injektion','1 ml','N1','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

                -- ═══ STIMMUNGSSTABILISIERER ═══
                ('a0000003-0001-0001-0001-000000000001','30010001','Quilonum retard 450mg','Lithium','N05AN01','450 mg','Retardtabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000002','30010002','Quilonum retard 450mg','Lithium','N05AN01','450 mg','Retardtabletten','100 Stk','N3','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000003','30010003','Hypnorex retard 400mg','Lithium','N05AN01','400 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000004','30010004','Ergenyl chrono 300mg','Valproat','N03AG01','300 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000005','30010005','Ergenyl chrono 300mg','Valproat','N03AG01','300 mg','Retardtabletten','100 Stk','N3','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000006','30010006','Ergenyl chrono 500mg','Valproat','N03AG01','500 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000007','30010007','Valproat-ratiopharm 600mg','Valproat','N03AG01','600 mg','Retardtabletten','50 Stk','N2','ratiopharm',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000008','30010008','Tegretal 200mg','Carbamazepin','N03AF01','200 mg','Retardtabletten','50 Stk','N2','Novartis',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000009','30010009','Tegretal 400mg','Carbamazepin','N03AF01','400 mg','Retardtabletten','50 Stk','N2','Novartis',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000010','30010010','Carbamazepin-ratiopharm 200mg','Carbamazepin','N03AF01','200 mg','Retardtabletten','50 Stk','N2','ratiopharm',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000011','30010011','Lamictal 25mg','Lamotrigin','N03AX09','25 mg','Tabletten','42 Stk','N1','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000012','30010012','Lamictal 50mg','Lamotrigin','N03AX09','50 mg','Tabletten','42 Stk','N1','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000013','30010013','Lamictal 100mg','Lamotrigin','N03AX09','100 mg','Tabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000014','30010014','Lamictal 200mg','Lamotrigin','N03AX09','200 mg','Tabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000015','30010015','Lamotrigin HEXAL 100mg','Lamotrigin','N03AX09','100 mg','Tabletten','100 Stk','N3','HEXAL',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
                ('a0000003-0001-0001-0001-000000000016','30010016','Lamotrigin HEXAL 200mg','Lamotrigin','N03AX09','200 mg','Tabletten','100 Stk','N3','HEXAL',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),

                -- ═══ ANXIOLYTIKA & HYPNOTIKA ═══
                ('a0000004-0001-0001-0001-000000000001','40010001','Tavor 0.5mg','Lorazepam','N05BA06','0.5 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000002','40010002','Tavor 1mg','Lorazepam','N05BA06','1 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000003','40010003','Tavor 1mg','Lorazepam','N05BA06','1 mg','Tabletten','50 Stk','N2','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000004','40010004','Tavor 2.5mg','Lorazepam','N05BA06','2.5 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000005','40010005','Lorazepam-neuraxpharm 1mg','Lorazepam','N05BA06','1 mg','Tabletten','20 Stk','N1','neuraxpharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000006','40010006','Valium 5mg','Diazepam','N05BA01','5 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000007','40010007','Valium 10mg','Diazepam','N05BA01','10 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000008','40010008','Diazepam-ratiopharm 5mg','Diazepam','N05BA01','5 mg','Tabletten','20 Stk','N1','ratiopharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000009','40010009','Adumbran 10mg','Oxazepam','N05BA04','10 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000010','40010010','Adumbran 50mg','Oxazepam','N05BA04','50 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000011','40010011','Oxazepam-ratiopharm 10mg','Oxazepam','N05BA04','10 mg','Tabletten','20 Stk','N1','ratiopharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000012','40010012','Lexotanil 6mg','Bromazepam','N05BA08','6 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000013','40010013','Lexotanil 6mg','Bromazepam','N05BA08','6 mg','Tabletten','50 Stk','N2','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000014','40010014','Buspiron-neuraxpharm 5mg','Buspiron','N05BE01','5 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Anxiolytikum',true,'SEED',NOW()),
                ('a0000004-0001-0001-0001-000000000015','40010015','Buspiron-neuraxpharm 10mg','Buspiron','N05BE01','10 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Anxiolytikum',true,'SEED',NOW()),

                -- Hypnotika
                ('a0000004-0002-0001-0001-000000000001','40020001','Stilnox 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','10 Stk','N1','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000002','40020002','Stilnox 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','20 Stk','N2','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000003','40020003','Zolpidem-ratiopharm 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','10 Stk','N1','ratiopharm',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000004','40020004','Ximovan 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','10 Stk','N1','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000005','40020005','Ximovan 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','20 Stk','N2','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000006','40020006','Zopiclon-ratiopharm 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','10 Stk','N1','ratiopharm',true,false,true,'Hypnotikum',true,'SEED',NOW()),
                ('a0000004-0002-0001-0001-000000000007','40020007','Chloralhydrat Rectiole 600mg','Chloralhydrat','N05CC01','600 mg','Rektallösung','5 Stk','N1','Pohl-Boskamp',false,false,true,'Hypnotikum',true,'SEED',NOW()),

                -- ═══ ANTIKONVULSIVA ═══
                ('a0000005-0001-0001-0001-000000000001','50010001','Keppra 250mg','Levetiracetam','N03AX14','250 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000002','50010002','Keppra 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000003','50010003','Keppra 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','100 Stk','N3','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000004','50010004','Keppra 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000005','50010005','Keppra 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','100 Stk','N3','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000006','50010006','Levetiracetam HEXAL 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000007','50010007','Levetiracetam HEXAL 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000008','50010008','Vimpat 50mg','Lacosamid','N03AX18','50 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000009','50010009','Vimpat 100mg','Lacosamid','N03AX18','100 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000010','50010010','Vimpat 100mg','Lacosamid','N03AX18','100 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000011','50010011','Vimpat 200mg','Lacosamid','N03AX18','200 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000012','50010012','Topamax 25mg','Topiramat','N03AX11','25 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000013','50010013','Topamax 50mg','Topiramat','N03AX11','50 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000014','50010014','Topamax 100mg','Topiramat','N03AX11','100 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000015','50010015','Topiramat HEXAL 100mg','Topiramat','N03AX11','100 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000016','50010016','Neurontin 300mg','Gabapentin','N03AX12','300 mg','Kapseln','50 Stk','N1','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000017','50010017','Neurontin 300mg','Gabapentin','N03AX12','300 mg','Kapseln','100 Stk','N2','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000018','50010018','Neurontin 600mg','Gabapentin','N03AX12','600 mg','Filmtabletten','50 Stk','N1','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000019','50010019','Gabapentin-ratiopharm 300mg','Gabapentin','N03AX12','300 mg','Kapseln','100 Stk','N2','ratiopharm',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000020','50010020','Lyrica 75mg','Pregabalin','N03AX16','75 mg','Kapseln','14 Stk','N1','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000021','50010021','Lyrica 75mg','Pregabalin','N03AX16','75 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000022','50010022','Lyrica 150mg','Pregabalin','N03AX16','150 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000023','50010023','Lyrica 300mg','Pregabalin','N03AX16','300 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000024','50010024','Pregabalin-ratiopharm 75mg','Pregabalin','N03AX16','75 mg','Kapseln','56 Stk','N2','ratiopharm',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000025','50010025','Pregabalin-ratiopharm 150mg','Pregabalin','N03AX16','150 mg','Kapseln','56 Stk','N2','ratiopharm',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000026','50010026','Trileptal 300mg','Oxcarbazepin','N03AF02','300 mg','Filmtabletten','50 Stk','N2','Novartis',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000027','50010027','Trileptal 600mg','Oxcarbazepin','N03AF02','600 mg','Filmtabletten','50 Stk','N2','Novartis',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000028','50010028','Oxcarbazepin-ratiopharm 300mg','Oxcarbazepin','N03AF02','300 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000029','50010029','Phenhydan 100mg','Phenytoin','N03AB02','100 mg','Tabletten','50 Stk','N2','Desitin',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000030','50010030','Briviact 25mg','Brivaracetam','N03AX23','25 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000031','50010031','Briviact 50mg','Brivaracetam','N03AX23','50 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000032','50010032','Briviact 50mg','Brivaracetam','N03AX23','50 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000033','50010033','Briviact 100mg','Brivaracetam','N03AX23','100 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000034','50010034','Zonegran 100mg','Zonisamid','N03AX15','100 mg','Kapseln','28 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000035','50010035','Fycompa 2mg','Perampanel','N03AX22','2 mg','Filmtabletten','7 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000036','50010036','Fycompa 4mg','Perampanel','N03AX22','4 mg','Filmtabletten','28 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000037','50010037','Luminal 100mg','Phenobarbital','N03AA02','100 mg','Tabletten','50 Stk','N2','Desitin',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
                ('a0000005-0001-0001-0001-000000000038','50010038','Petnidan 250mg','Ethosuximid','N03AD01','250 mg','Kapseln','50 Stk','N2','Desitin',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),

                -- ═══ PARKINSON-MEDIKAMENTE ═══
                ('a0000006-0001-0001-0001-000000000001','60010001','Madopar 125 (100/25)','Levodopa/Benserazid','N04BA02','100/25 mg','Kapseln','30 Stk','N1','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000002','60010002','Madopar 125 (100/25)','Levodopa/Benserazid','N04BA02','100/25 mg','Kapseln','100 Stk','N3','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000003','60010003','Madopar 250 (200/50)','Levodopa/Benserazid','N04BA02','200/50 mg','Tabletten','30 Stk','N1','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000004','60010004','Madopar 250 (200/50)','Levodopa/Benserazid','N04BA02','200/50 mg','Tabletten','100 Stk','N3','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000005','60010005','Nacom 100/25','Levodopa/Carbidopa','N04BA02','100/25 mg','Tabletten','30 Stk','N1','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000006','60010006','Nacom 100/25','Levodopa/Carbidopa','N04BA02','100/25 mg','Tabletten','100 Stk','N3','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000007','60010007','Nacom 200/50','Levodopa/Carbidopa','N04BA02','200/50 mg','Tabletten','100 Stk','N3','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000008','60010008','Sifrol 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','30 Stk','N1','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000009','60010009','Sifrol 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','100 Stk','N3','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000010','60010010','Sifrol 0.7mg','Pramipexol','N04BC05','0.7 mg','Tabletten','30 Stk','N1','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000011','60010011','Pramipexol-ratiopharm 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','30 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000012','60010012','Requip 2mg','Ropinirol','N04BC04','2 mg','Filmtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000013','60010013','Requip-Modutab 4mg','Ropinirol','N04BC04','4 mg','Retardtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000014','60010014','Requip-Modutab 8mg','Ropinirol','N04BC04','8 mg','Retardtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000015','60010015','Neupro 2mg/24h','Rotigotin','N04BC09','2 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000016','60010016','Neupro 4mg/24h','Rotigotin','N04BC09','4 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000017','60010017','Neupro 6mg/24h','Rotigotin','N04BC09','6 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000018','60010018','Neupro 8mg/24h','Rotigotin','N04BC09','8 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000019','60010019','Azilect 1mg','Rasagilin','N04BD02','1 mg','Tabletten','28 Stk','N1','Teva',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000020','60010020','Rasagilin-ratiopharm 1mg','Rasagilin','N04BD02','1 mg','Tabletten','28 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000021','60010021','Selegilin-ratiopharm 5mg','Selegilin','N04BD01','5 mg','Tabletten','30 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000022','60010022','Xadago 50mg','Safinamid','N04BD03','50 mg','Filmtabletten','28 Stk','N1','Zambon',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000023','60010023','Xadago 100mg','Safinamid','N04BD03','100 mg','Filmtabletten','28 Stk','N1','Zambon',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000024','60010024','Comtess 200mg','Entacapon','N04BX02','200 mg','Filmtabletten','30 Stk','N1','Orion',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000025','60010025','Comtess 200mg','Entacapon','N04BX02','200 mg','Filmtabletten','100 Stk','N3','Orion',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000026','60010026','Ongentys 50mg','Opicapon','N04BX04','50 mg','Kapseln','30 Stk','N1','Bial',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000027','60010027','PK-Merz 100mg','Amantadin','N04BB01','100 mg','Filmtabletten','30 Stk','N1','Merz',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000028','60010028','PK-Merz 100mg','Amantadin','N04BB01','100 mg','Filmtabletten','100 Stk','N3','Merz',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
                ('a0000006-0001-0001-0001-000000000029','60010029','Amantadin-neuraxpharm 200mg','Amantadin','N04BB01','200 mg','Retardtabletten','30 Stk','N1','neuraxpharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),

                -- ═══ MS-THERAPEUTIKA ═══
                ('a0000007-0001-0001-0001-000000000001','70010001','Tecfidera 120mg','Dimethylfumarat','L04AX07','120 mg','Kapseln','14 Stk','N1','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000002','70010002','Tecfidera 240mg','Dimethylfumarat','L04AX07','240 mg','Kapseln','56 Stk','N2','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000003','70010003','Tecfidera 240mg','Dimethylfumarat','L04AX07','240 mg','Kapseln','168 Stk','N3','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000004','70010004','Gilenya 0.5mg','Fingolimod','L04AA27','0.5 mg','Kapseln','28 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000005','70010005','Gilenya 0.5mg','Fingolimod','L04AA27','0.5 mg','Kapseln','98 Stk','N3','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000006','70010006','Aubagio 14mg','Teriflunomid','L04AA31','14 mg','Filmtabletten','28 Stk','N1','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000007','70010007','Aubagio 14mg','Teriflunomid','L04AA31','14 mg','Filmtabletten','84 Stk','N3','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000008','70010008','Ocrevus 300mg','Ocrelizumab','L04AA36','300 mg','Inf.-Lösung','1 Stk','N1','Roche',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000009','70010009','Tysabri 300mg','Natalizumab','L04AA23','300 mg','Inf.-Lösung','1 Stk','N1','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000010','70010010','Lemtrada 12mg','Alemtuzumab','L04AA34','12 mg','Inf.-Lösung','1 Stk','N1','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000011','70010011','Copaxone 20mg','Glatirameracetat','L03AX13','20 mg','Inj.-Lösung','28 Stk','N1','Teva',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000012','70010012','Copaxone 40mg','Glatirameracetat','L03AX13','40 mg','Inj.-Lösung','12 Stk','N1','Teva',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000013','70010013','Mavenclad 10mg','Cladribin','L04AA40','10 mg','Tabletten','1 Stk','N1','Merck',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000014','70010014','Kesimpta 20mg','Ofatumumab','L04AA52','20 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000015','70010015','Zeposia 0.92mg','Ozanimod','L04AA38','0.92 mg','Kapseln','28 Stk','N1','Bristol-MS',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000016','70010016','Mayzent 2mg','Siponimod','L04AA42','2 mg','Filmtabletten','28 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000017','70010017','Ponvory 20mg','Ponesimod','L04AA50','20 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000018','70010018','Vumerity 231mg','Diroximelfumarat','L04AX09','231 mg','Kapseln','56 Stk','N2','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
                ('a0000007-0001-0001-0001-000000000019','70010019','Betaferon 250mcg','Interferon beta-1b','L03AB08','250 mcg','Inj.-Lösung','15 Stk','N1','Bayer',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),

                -- ═══ SCHMERZ / MIGRÄNE ═══
                ('a0000008-0001-0001-0001-000000000001','80010001','Imigran 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','2 Stk','N1','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000002','80010002','Imigran 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','6 Stk','N2','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000003','80010003','Imigran 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','2 Stk','N1','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000004','80010004','Imigran 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','6 Stk','N2','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000005','80010005','Sumatriptan-1A Pharma 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','6 Stk','N2','1A Pharma',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000006','80010006','Sumatriptan-1A Pharma 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','6 Stk','N2','1A Pharma',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000007','80010007','Maxalt 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','3 Stk','N1','MSD',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000008','80010008','Maxalt 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','6 Stk','N2','MSD',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000009','80010009','Rizatriptan HEXAL 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','6 Stk','N2','HEXAL',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000010','80010010','AscoTop 2.5mg','Zolmitriptan','N02CC03','2.5 mg','Schmelztabletten','3 Stk','N1','AstraZeneca',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000011','80010011','AscoTop 2.5mg','Zolmitriptan','N02CC03','2.5 mg','Schmelztabletten','6 Stk','N2','AstraZeneca',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000012','80010012','Dociton 40mg','Propranolol','C07AA05','40 mg','Filmtabletten','50 Stk','N2','MEDA',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000013','80010013','Dociton 80mg','Propranolol','C07AA05','80 mg','Filmtabletten','50 Stk','N2','MEDA',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000014','80010014','Propranolol-ratiopharm 40mg','Propranolol','C07AA05','40 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000015','80010015','Flunarizin-ratiopharm 5mg','Flunarizin','N07CA03','5 mg','Kapseln','20 Stk','N1','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000016','80010016','Flunarizin-ratiopharm 5mg','Flunarizin','N07CA03','5 mg','Kapseln','50 Stk','N2','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000017','80010017','Aimovig 70mg','Erenumab','N02CD01','70 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000018','80010018','Aimovig 140mg','Erenumab','N02CD01','140 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000019','80010019','Ajovy 225mg','Fremanezumab','N02CD03','225 mg','Inj.-Lösung','1 Stk','N1','Teva',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000020','80010020','Ajovy 225mg','Fremanezumab','N02CD03','225 mg','Inj.-Lösung','3 Stk','N2','Teva',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000021','80010021','Emgality 120mg','Galcanezumab','N02CD02','120 mg','Inj.-Lösung','1 Stk','N1','Lilly',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000022','80010022','Emgality 120mg','Galcanezumab','N02CD02','120 mg','Inj.-Lösung','3 Stk','N2','Lilly',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
                ('a0000008-0001-0001-0001-000000000023','80010023','Vydura 75mg','Rimegepant','N02CD06','75 mg','Schmelztabletten','8 Stk','N1','Pfizer',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),

                -- ═══ STIMULANZIEN ═══
                ('a0000009-0001-0001-0001-000000000001','90010001','Medikinet retard 10mg','Methylphenidat','N06BA04','10 mg','Retardkapseln','30 Stk','N1','Medice',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000002','90010002','Medikinet retard 20mg','Methylphenidat','N06BA04','20 mg','Retardkapseln','30 Stk','N1','Medice',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000003','90010003','Concerta 36mg','Methylphenidat','N06BA04','36 mg','Retardtabletten','30 Stk','N1','Janssen',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000004','90010004','Concerta 54mg','Methylphenidat','N06BA04','54 mg','Retardtabletten','30 Stk','N1','Janssen',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000005','90010005','Ritalin adult 10mg','Methylphenidat','N06BA04','10 mg','Tabletten','30 Stk','N1','Novartis',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000006','90010006','Ritalin adult 20mg','Methylphenidat','N06BA04','20 mg','Retardkapseln','30 Stk','N1','Novartis',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000007','90010007','Methylphenidat HEXAL 10mg','Methylphenidat','N06BA04','10 mg','Tabletten','50 Stk','N2','HEXAL',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000008','90010008','Elvanse 30mg','Lisdexamfetamin','N06BA12','30 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000009','90010009','Elvanse 50mg','Lisdexamfetamin','N06BA12','50 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000010','90010010','Elvanse 70mg','Lisdexamfetamin','N06BA12','70 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000011','90010011','Strattera 10mg','Atomoxetin','N06BA09','10 mg','Kapseln','7 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000012','90010012','Strattera 25mg','Atomoxetin','N06BA09','25 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000013','90010013','Strattera 40mg','Atomoxetin','N06BA09','40 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
                ('a0000009-0001-0001-0001-000000000014','90010014','Strattera 60mg','Atomoxetin','N06BA09','60 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),

                -- ═══ SONSTIGE ═══
                ('a0000010-0001-0001-0001-000000000001','99010001','Circadin 2mg','Melatonin','N05CH01','2 mg','Retardtabletten','21 Stk','N1','Neurim',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000002','99010002','Akineton 2mg','Biperiden','N04AA02','2 mg','Tabletten','30 Stk','N1','Desma',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000003','99010003','Akineton 4mg','Biperiden','N04AA02','4 mg','Retardtabletten','30 Stk','N1','Desma',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000004','99010004','Artane 2mg','Trihexyphenidyl','N04AA01','2 mg','Tabletten','30 Stk','N1','Teofarma',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000005','99010005','Artane 5mg','Trihexyphenidyl','N04AA01','5 mg','Tabletten','30 Stk','N1','Teofarma',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000006','99010006','Adepend 50mg','Naltrexon','N07BB04','50 mg','Filmtabletten','28 Stk','N1','Desitin',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000007','99010007','Campral 333mg','Acamprosat','N07BB03','333 mg','Filmtabletten','84 Stk','N1','Merck',false,false,true,'Sonstiges',true,'SEED',NOW()),
                ('a0000010-0001-0001-0001-000000000008','99010008','Antabus 200mg','Disulfiram','N07BB01','200 mg','Tabletten','50 Stk','N2','Actavis',false,false,true,'Sonstiges',true,'SEED',NOW()),

                -- ═══ SUBSTITUTIONSMITTEL (BtM) ═══
                ('a0000010-0002-0001-0001-000000000001','99020001','L-Polamidon 5mg/ml','Levomethadon','N07BC05','5 mg/ml','Lösung','100 ml','N2','Sanofi',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
                ('a0000010-0002-0001-0001-000000000002','99020002','Methaddict 5mg','Methadon','N07BC02','5 mg','Tabletten','50 Stk','N2','Hexal Medical',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
                ('a0000010-0002-0001-0001-000000000003','99020003','Methaddict 10mg','Methadon','N07BC02','10 mg','Tabletten','50 Stk','N2','Hexal Medical',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
                ('a0000010-0002-0001-0001-000000000004','99020004','Subutex 2mg','Buprenorphin','N07BC01','2 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
                ('a0000010-0002-0001-0001-000000000005','99020005','Subutex 8mg','Buprenorphin','N07BC01','8 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
                ('a0000010-0002-0001-0001-000000000006','99020006','Suboxone 8/2mg','Buprenorphin/Naloxon','N07BC51','8/2 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW())

                ON CONFLICT ("Pzn") DO NOTHING;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medication_catalog");
        }
    }
}
