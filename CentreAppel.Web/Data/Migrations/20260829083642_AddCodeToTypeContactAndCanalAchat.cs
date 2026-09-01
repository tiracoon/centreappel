using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToTypeContactAndCanalAchat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "types_contact",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "canaux_achat",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            // Peuple le code métier de toutes les lignes de ces deux référentiels (jeu fermé et
            // restreint), plus les lignes de deroulements/interets_client non couvertes par la
            // migration précédente (qui ne codait que les 4 valeurs pilotant les règles de
            // saisissabilité de la popup).
            migrationBuilder.Sql(
                """
                UPDATE types_contact SET code = 'APPEL' WHERE libelle = 'Appel';
                UPDATE types_contact SET code = 'EMAIL' WHERE libelle = 'Email';
                UPDATE types_contact SET code = 'SMS' WHERE libelle = 'SMS';
                UPDATE types_contact SET code = 'COURRIER' WHERE libelle = 'Courrier';

                UPDATE canaux_achat SET code = 'WEB' WHERE libelle = 'Web';
                UPDATE canaux_achat SET code = 'MAGASIN' WHERE libelle = 'Magasin';

                UPDATE deroulements SET code = 'NUMERO_NON_ATTRIBUE' WHERE libelle = 'Numéro non attribué';
                UPDATE deroulements SET code = 'FAUX_NUMERO' WHERE libelle = 'Faux numéro';
                UPDATE deroulements SET code = 'ENTREPRISE_FERMEE' WHERE libelle = 'Entreprise fermée';
                UPDATE deroulements SET code = 'MAUVAIS_INTERLOCUTEUR' WHERE libelle = 'Mauvais interlocuteur';
                UPDATE deroulements SET code = 'DOUBLON' WHERE libelle = 'Doublon';
                UPDATE deroulements SET code = 'REPONDEUR' WHERE libelle = 'Répondeur';

                UPDATE interets_client SET code = 'REFRACTAIRE' WHERE libelle = 'Réfractaire';
                UPDATE interets_client SET code = 'INTERESSE_WEB' WHERE libelle = 'Intéressé via Web';
                UPDATE interets_client SET code = 'INTERESSE_MAG' WHERE libelle = 'Intéressé via Mag';
                """);

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_code",
                table: "types_contact",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_types_contact_code",
                table: "types_contact");

            migrationBuilder.DropIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat");

            migrationBuilder.DropColumn(
                name: "code",
                table: "types_contact");

            migrationBuilder.DropColumn(
                name: "code",
                table: "canaux_achat");
        }
    }
}
