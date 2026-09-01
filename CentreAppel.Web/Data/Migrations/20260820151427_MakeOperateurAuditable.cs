using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeOperateurAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Lignes existantes sans date de création réelle : backfill avec l'heure UTC courante.
            migrationBuilder.AddColumn<DateTime>(
                name: "dh_creation",
                table: "operateurs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "dh_modif",
                table: "operateurs",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dh_creation",
                table: "operateurs");

            migrationBuilder.DropColumn(
                name: "dh_modif",
                table: "operateurs");
        }
    }
}
