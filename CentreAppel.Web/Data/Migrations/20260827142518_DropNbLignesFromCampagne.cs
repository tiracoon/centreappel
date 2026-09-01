using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropNbLignesFromCampagne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nb_lignes",
                table: "e_campagnes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "operateurs",
                newName: "id_operateur");

            migrationBuilder.RenameColumn(
                name: "id_camp_op",
                table: "campagnes_operateur",
                newName: "id_campagne_operateur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_operateur",
                table: "operateurs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "id_campagne_operateur",
                table: "campagnes_operateur",
                newName: "id_camp_op");

            migrationBuilder.AddColumn<int>(
                name: "nb_lignes",
                table: "e_campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
