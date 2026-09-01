using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_role",
                table: "operateurs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dh_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dh_modif = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id_role);
                });

            migrationBuilder.CreateIndex(
                name: "ix_operateurs_id_role",
                table: "operateurs",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "ix_roles_libelle",
                table: "roles",
                column: "libelle",
                unique: true);

            // Données de seed jetables (POC) sans rôle valide : purgées pour permettre la contrainte NOT NULL/FK ci-dessous.
            // Le seeder de l'application les recrée avec un rôle au prochain démarrage.
            migrationBuilder.Sql("DELETE FROM operateurs;");

            migrationBuilder.AddForeignKey(
                name: "fk_operateurs_roles_id_role",
                table: "operateurs",
                column: "id_role",
                principalTable: "roles",
                principalColumn: "id_role",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operateurs_roles_id_role",
                table: "operateurs");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropIndex(
                name: "ix_operateurs_id_role",
                table: "operateurs");

            migrationBuilder.DropColumn(
                name: "id_role",
                table: "operateurs");
        }
    }
}
