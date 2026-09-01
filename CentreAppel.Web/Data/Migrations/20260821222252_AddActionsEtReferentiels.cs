using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActionsEtReferentiels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campagnes_operateur",
                columns: table => new
                {
                    id_camp_op = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_campagne = table.Column<long>(type: "bigint", nullable: false),
                    id_operateur = table.Column<long>(type: "bigint", nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campagnes_operateur", x => x.id_camp_op);
                    table.ForeignKey(
                        name: "fk_campagnes_operateur_e_campagnes_id_campagne",
                        column: x => x.id_campagne,
                        principalTable: "e_campagnes",
                        principalColumn: "id_campagne",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_campagnes_operateur_operateurs_id_operateur",
                        column: x => x.id_operateur,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "canaux_achat",
                columns: table => new
                {
                    id_canal = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_canaux_achat", x => x.id_canal);
                });

            migrationBuilder.CreateTable(
                name: "commentaires_campagne",
                columns: table => new
                {
                    id_commentaire = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_campagne = table.Column<long>(type: "bigint", nullable: false),
                    libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commentaires_campagne", x => x.id_commentaire);
                    table.ForeignKey(
                        name: "fk_commentaires_campagne_e_campagnes_id_campagne",
                        column: x => x.id_campagne,
                        principalTable: "e_campagnes",
                        principalColumn: "id_campagne",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "deroulements",
                columns: table => new
                {
                    id_deroulement = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deroulements", x => x.id_deroulement);
                });

            migrationBuilder.CreateTable(
                name: "interets_client",
                columns: table => new
                {
                    id_interet = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interets_client", x => x.id_interet);
                });

            migrationBuilder.CreateTable(
                name: "types_contact",
                columns: table => new
                {
                    id_type_contact = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    defaut = table.Column<bool>(type: "boolean", nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types_contact", x => x.id_type_contact);
                });

            migrationBuilder.CreateTable(
                name: "actions_campagne",
                columns: table => new
                {
                    id_actions_campagnes = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_l_campagne = table.Column<long>(type: "bigint", nullable: false),
                    num_action = table.Column<int>(type: "integer", nullable: false),
                    dh_action = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_operateur = table.Column<long>(type: "bigint", nullable: false),
                    id_type_contact = table.Column<int>(type: "integer", nullable: false),
                    id_deroulement = table.Column<int>(type: "integer", nullable: false),
                    id_interet = table.Column<int>(type: "integer", nullable: true),
                    date_relance = table.Column<DateOnly>(type: "date", nullable: true),
                    date_achat = table.Column<DateOnly>(type: "date", nullable: true),
                    id_canal = table.Column<int>(type: "integer", nullable: true),
                    id_commentaire = table.Column<long>(type: "bigint", nullable: true),
                    commentaire_libre = table.Column<string>(type: "text", nullable: true),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_operateur_cm = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_actions_campagne", x => x.id_actions_campagnes);
                    table.ForeignKey(
                        name: "fk_actions_campagne_canaux_achat_id_canal",
                        column: x => x.id_canal,
                        principalTable: "canaux_achat",
                        principalColumn: "id_canal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_commentaires_campagne_id_commentaire",
                        column: x => x.id_commentaire,
                        principalTable: "commentaires_campagne",
                        principalColumn: "id_commentaire",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_deroulements_id_deroulement",
                        column: x => x.id_deroulement,
                        principalTable: "deroulements",
                        principalColumn: "id_deroulement",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_interets_client_id_interet",
                        column: x => x.id_interet,
                        principalTable: "interets_client",
                        principalColumn: "id_interet",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_l_campagnes_id_l_campagne",
                        column: x => x.id_l_campagne,
                        principalTable: "l_campagnes",
                        principalColumn: "id_l_campagne",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_operateurs_id_operateur",
                        column: x => x.id_operateur,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_operateurs_id_operateur_cm",
                        column: x => x.id_operateur_cm,
                        principalTable: "operateurs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_actions_campagne_types_contact_id_type_contact",
                        column: x => x.id_type_contact,
                        principalTable: "types_contact",
                        principalColumn: "id_type_contact",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_date_relance",
                table: "actions_campagne",
                column: "date_relance",
                filter: "date_relance IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_canal",
                table: "actions_campagne",
                column: "id_canal");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_commentaire",
                table: "actions_campagne",
                column: "id_commentaire");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_deroulement",
                table: "actions_campagne",
                column: "id_deroulement");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_interet",
                table: "actions_campagne",
                column: "id_interet");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_l_campagne_num_action",
                table: "actions_campagne",
                columns: new[] { "id_l_campagne", "num_action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_operateur",
                table: "actions_campagne",
                column: "id_operateur");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_operateur_cm",
                table: "actions_campagne",
                column: "id_operateur_cm");

            migrationBuilder.CreateIndex(
                name: "ix_actions_campagne_id_type_contact",
                table: "actions_campagne",
                column: "id_type_contact");

            migrationBuilder.CreateIndex(
                name: "ix_campagnes_operateur_id_campagne_id_operateur",
                table: "campagnes_operateur",
                columns: new[] { "id_campagne", "id_operateur" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_campagnes_operateur_id_operateur",
                table: "campagnes_operateur",
                column: "id_operateur");

            migrationBuilder.CreateIndex(
                name: "ix_canaux_achat_libelle",
                table: "canaux_achat",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_commentaires_campagne_id_campagne",
                table: "commentaires_campagne",
                column: "id_campagne");

            migrationBuilder.CreateIndex(
                name: "ix_deroulements_libelle",
                table: "deroulements",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interets_client_libelle",
                table: "interets_client",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_defaut",
                table: "types_contact",
                column: "defaut",
                unique: true,
                filter: "defaut = true");

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_libelle",
                table: "types_contact",
                column: "libelle",
                unique: true);

            migrationBuilder.Sql(
                """
                CREATE VIEW v_derniere_action AS
                SELECT DISTINCT ON (a.id_l_campagne) a.*
                FROM   actions_campagne a
                ORDER  BY a.id_l_campagne, a.num_action DESC;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW v_derniere_action;");

            migrationBuilder.DropTable(
                name: "actions_campagne");

            migrationBuilder.DropTable(
                name: "campagnes_operateur");

            migrationBuilder.DropTable(
                name: "canaux_achat");

            migrationBuilder.DropTable(
                name: "commentaires_campagne");

            migrationBuilder.DropTable(
                name: "deroulements");

            migrationBuilder.DropTable(
                name: "interets_client");

            migrationBuilder.DropTable(
                name: "types_contact");
        }
    }
}
