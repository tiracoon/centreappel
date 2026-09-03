using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentreAppel.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggerNumActionParLigne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NUM_ACTION numérote les tentatives d'UNE ligne (repart à 1 par id_l_campagne) - le
            // calcul MAX+1 précédemment fait côté C# (CampagneService.SaveActionAsync) n'était pas
            // protégé contre les accès concurrents sur la même ligne, et ne s'appliquait pas à un
            // éventuel import direct en base (hors application). Déplacé ici pour être atomique et
            // valable quelle que soit la voie d'insertion.
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION fn_actions_campagne_num_action()
                RETURNS trigger AS $$
                BEGIN
                    -- Verrouille la ligne parente : sans ce verrou, deux insertions concurrentes sur
                    -- la même ligne pourraient toutes les deux lire le même MAX(num_action) avant que
                    -- l'une des deux ne commite, et produire un doublon.
                    PERFORM 1 FROM l_campagnes WHERE id_l_campagne = NEW.id_l_campagne FOR UPDATE;

                    SELECT COALESCE(MAX(num_action), 0) + 1
                    INTO NEW.num_action
                    FROM actions_campagne
                    WHERE id_l_campagne = NEW.id_l_campagne;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trg_actions_campagne_num_action
                BEFORE INSERT ON actions_campagne
                FOR EACH ROW
                EXECUTE FUNCTION fn_actions_campagne_num_action();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS trg_actions_campagne_num_action ON actions_campagne;
                DROP FUNCTION IF EXISTS fn_actions_campagne_num_action();
                """);
        }
    }
}
