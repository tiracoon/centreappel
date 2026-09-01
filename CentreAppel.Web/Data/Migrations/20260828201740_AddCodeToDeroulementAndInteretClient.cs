using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToDeroulementAndInteretClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "interets_client",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "deroulements",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            // Peuple le code métier des seules valeurs référencées par les règles de saisissabilité
            // de la popup de saisie d'action — les autres lignes gardent un code NULL (pas de règle
            // qui les concerne).
            migrationBuilder.Sql(
                """
                UPDATE deroulements SET code = 'CONTACT_ARGUMENTE' WHERE libelle = 'Contact argumenté';
                UPDATE deroulements SET code = 'A_RAPPELER' WHERE libelle = 'À rappeler';
                UPDATE deroulements SET code = 'NE_PLUS_CONTACTER' WHERE libelle = 'Ne plus contacter';
                UPDATE interets_client SET code = 'VENTE_VALIDEE' WHERE libelle = 'Vente validée';
                """);

            migrationBuilder.CreateIndex(
                name: "ix_interets_client_code",
                table: "interets_client",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_deroulements_code",
                table: "deroulements",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_interets_client_code",
                table: "interets_client");

            migrationBuilder.DropIndex(
                name: "ix_deroulements_code",
                table: "deroulements");

            migrationBuilder.DropColumn(
                name: "code",
                table: "interets_client");

            migrationBuilder.DropColumn(
                name: "code",
                table: "deroulements");
        }
    }
}
