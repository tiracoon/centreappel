namespace CentreAppel.Web.Application.Models;

// Chiffres affichés sur /tableau-de-bord (§5.6) pour la campagne sélectionnée.
public class TableauDeBordDonnees
{
    public int NbLignes { get; set; }
    public int Traites { get; set; }
    public int Restants { get; set; }
    public int ARappeler { get; set; }

    // Contacts "Contact argumenté" / contacts traités, en pourcentage (0-100).
    public decimal TauxArgumentation { get; set; }

    public int VentesValidees { get; set; }

    public int PourcentageTraite => NbLignes == 0 ? 0 : (int)Math.Round(Traites * 100m / NbLignes);
}
