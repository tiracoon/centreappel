using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatutCampagneEnEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Postgres ne convertit pas automatiquement varchar -> integer : mapping explicite
            // des valeurs existantes vers les ordinaux de StatutCampagne (EnPreparation=0, Active=1, Cloturee=2, Archivee=3).
            migrationBuilder.Sql(
                """
                ALTER TABLE e_campagnes ALTER COLUMN statut TYPE integer USING (
                    CASE statut
                        WHEN 'EN_PREPARATION' THEN 0
                        WHEN 'ACTIVE' THEN 1
                        WHEN 'CLOTUREE' THEN 2
                        WHEN 'ARCHIVEE' THEN 3
                    END
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE e_campagnes ALTER COLUMN statut TYPE character varying(20) USING (
                    CASE statut
                        WHEN 0 THEN 'EN_PREPARATION'
                        WHEN 1 THEN 'ACTIVE'
                        WHEN 2 THEN 'CLOTUREE'
                        WHEN 3 THEN 'ARCHIVEE'
                    END
                );
                """);
        }
    }
}
