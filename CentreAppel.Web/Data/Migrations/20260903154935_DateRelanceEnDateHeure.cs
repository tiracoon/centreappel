using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class DateRelanceEnDateHeure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // v_derniere_action (SELECT DISTINCT ON (...) a.*) dépend de la colonne : PostgreSQL
            // refuse un ALTER COLUMN TYPE tant que la vue existe, même en "SELECT *". On la
            // supprime et on la recrée à l'identique (même définition que la migration
            // AddActionsEtReferentiels) après le changement de type.
            migrationBuilder.Sql("DROP VIEW v_derniere_action;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_relance",
                table: "actions_campagne",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<DateOnly>(
                name: "date_relance",
                table: "actions_campagne",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.Sql(
                """
                CREATE VIEW v_derniere_action AS
                SELECT DISTINCT ON (a.id_l_campagne) a.*
                FROM   actions_campagne a
                ORDER  BY a.id_l_campagne, a.num_action DESC;
                """);
        }
    }
}
