using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampagnesAndBigIntOperateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "operateurs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "e_campagnes",
                columns: table => new
                {
                    id_campagne = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    date_campagne = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    nb_lignes = table.Column<int>(type: "integer", nullable: false),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_operateur_cm = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_e_campagnes", x => x.id_campagne);
                    table.ForeignKey(
                        name: "fk_e_campagnes_operateurs_id_operateur_cm",
                        column: x => x.id_operateur_cm,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "l_campagnes",
                columns: table => new
                {
                    id_l_campagne = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_campagne = table.Column<long>(type: "bigint", nullable: false),
                    num_ligne = table.Column<int>(type: "integer", nullable: false),
                    code_soc = table.Column<string>(type: "character(3)", nullable: false),
                    num_cli = table.Column<decimal>(type: "numeric(12,0)", precision: 12, scale: 0, nullable: false),
                    id_operateur_assigne = table.Column<long>(type: "bigint", nullable: true),
                    siret = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    raison_sociale = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    sous_activite = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    magasin_affilie = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    correspondant = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    telephone = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    email = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    adresse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cp = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ville = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    pays = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    langue = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    rfm = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ca_ht = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: true),
                    date_dernier_achat = table.Column<DateOnly>(type: "date", nullable: true),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_operateur_cm = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_l_campagnes", x => x.id_l_campagne);
                    table.ForeignKey(
                        name: "fk_l_campagnes_e_campagnes_id_campagne",
                        column: x => x.id_campagne,
                        principalTable: "e_campagnes",
                        principalColumn: "id_campagne",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_l_campagnes_operateurs_id_operateur_assigne",
                        column: x => x.id_operateur_assigne,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_l_campagnes_operateurs_id_operateur_cm",
                        column: x => x.id_operateur_cm,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_e_campagnes_id_operateur_cm",
                table: "e_campagnes",
                column: "id_operateur_cm");

            migrationBuilder.CreateIndex(
                name: "ix_l_campagnes_id_campagne_code_soc_num_cli",
                table: "l_campagnes",
                columns: new[] { "id_campagne", "code_soc", "num_cli" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_l_campagnes_id_operateur_assigne",
                table: "l_campagnes",
                column: "id_operateur_assigne");

            migrationBuilder.CreateIndex(
                name: "ix_l_campagnes_id_operateur_cm",
                table: "l_campagnes",
                column: "id_operateur_cm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "l_campagnes");

            migrationBuilder.DropTable(
                name: "e_campagnes");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "operateurs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
