using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVerrousParametresHorsContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients_hors_contact",
                columns: table => new
                {
                    id_clients_hc = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    soc = table.Column<string>(type: "character(3)", nullable: false),
                    num_cli = table.Column<decimal>(type: "numeric(12,0)", precision: 12, scale: 0, nullable: false),
                    date_exclusion = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients_hors_contact", x => x.id_clients_hc);
                });

            migrationBuilder.CreateTable(
                name: "parametres",
                columns: table => new
                {
                    id_parametre = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    valeur_texte = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    valeur_num = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parametres", x => x.id_parametre);
                });

            migrationBuilder.CreateTable(
                name: "verrous_ligne",
                columns: table => new
                {
                    id_l_campagne = table.Column<long>(type: "bigint", nullable: false),
                    id_operateur = table.Column<long>(type: "bigint", nullable: false),
                    dh_verrou = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verrous_ligne", x => x.id_l_campagne);
                    table.ForeignKey(
                        name: "fk_verrous_ligne_l_campagnes_id_l_campagne",
                        column: x => x.id_l_campagne,
                        principalTable: "l_campagnes",
                        principalColumn: "id_l_campagne",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_verrous_ligne_operateurs_id_operateur",
                        column: x => x.id_operateur,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_clients_hors_contact_soc_num_cli",
                table: "clients_hors_contact",
                columns: new[] { "soc", "num_cli" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_parametres_libelle",
                table: "parametres",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_verrous_ligne_id_operateur",
                table: "verrous_ligne",
                column: "id_operateur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clients_hors_contact");

            migrationBuilder.DropTable(
                name: "parametres");

            migrationBuilder.DropTable(
                name: "verrous_ligne");
        }
    }
}
