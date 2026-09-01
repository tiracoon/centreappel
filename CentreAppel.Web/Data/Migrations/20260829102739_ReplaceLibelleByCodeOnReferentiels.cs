using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceLibelleByCodeOnReferentiels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_types_contact_code",
                table: "types_contact");

            migrationBuilder.DropIndex(
                name: "ix_types_contact_libelle",
                table: "types_contact");

            migrationBuilder.DropIndex(
                name: "ix_interets_client_code",
                table: "interets_client");

            migrationBuilder.DropIndex(
                name: "ix_interets_client_libelle",
                table: "interets_client");

            migrationBuilder.DropIndex(
                name: "ix_deroulements_code",
                table: "deroulements");

            migrationBuilder.DropIndex(
                name: "ix_deroulements_libelle",
                table: "deroulements");

            migrationBuilder.DropIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat");

            migrationBuilder.DropIndex(
                name: "ix_canaux_achat_libelle",
                table: "canaux_achat");

            migrationBuilder.DropColumn(
                name: "libelle",
                table: "types_contact");

            migrationBuilder.DropColumn(
                name: "libelle",
                table: "interets_client");

            migrationBuilder.DropColumn(
                name: "libelle",
                table: "deroulements");

            migrationBuilder.DropColumn(
                name: "libelle",
                table: "canaux_achat");

            migrationBuilder.RenameColumn(
                name: "libelle",
                table: "roles",
                newName: "code");

            migrationBuilder.RenameIndex(
                name: "ix_roles_libelle",
                table: "roles",
                newName: "ix_roles_code");

            // Le renommage ci-dessus copie tel quel l'ancien Libelle ("Conseiller", "Administrateur")
            // dans code — on normalise en majuscules pour rester cohérent avec la convention des
            // autres référentiels (CONTACT_ARGUMENTE, etc.) et avec [Authorize(Roles = "ADMINISTRATEUR")].
            migrationBuilder.Sql(
                """
                UPDATE roles SET code = 'CONSEILLER' WHERE code = 'Conseiller';
                UPDATE roles SET code = 'ADMINISTRATEUR' WHERE code = 'Administrateur';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "types_contact",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "interets_client",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "deroulements",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "canaux_achat",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_code",
                table: "types_contact",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interets_client_code",
                table: "interets_client",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deroulements_code",
                table: "deroulements",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_types_contact_code",
                table: "types_contact");

            migrationBuilder.DropIndex(
                name: "ix_interets_client_code",
                table: "interets_client");

            migrationBuilder.DropIndex(
                name: "ix_deroulements_code",
                table: "deroulements");

            migrationBuilder.DropIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "roles",
                newName: "libelle");

            migrationBuilder.RenameIndex(
                name: "ix_roles_code",
                table: "roles",
                newName: "ix_roles_libelle");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "types_contact",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "libelle",
                table: "types_contact",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "interets_client",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "libelle",
                table: "interets_client",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "deroulements",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "libelle",
                table: "deroulements",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "canaux_achat",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "libelle",
                table: "canaux_achat",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_code",
                table: "types_contact",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_types_contact_libelle",
                table: "types_contact",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interets_client_code",
                table: "interets_client",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_interets_client_libelle",
                table: "interets_client",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deroulements_code",
                table: "deroulements",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_deroulements_libelle",
                table: "deroulements",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_canaux_achat_code",
                table: "canaux_achat",
                column: "code",
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_canaux_achat_libelle",
                table: "canaux_achat",
                column: "libelle",
                unique: true);
        }
    }
}
