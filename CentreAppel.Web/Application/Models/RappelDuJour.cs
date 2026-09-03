namespace CentreAppel.Web.Application.Models;

// Ligne affichée dans /rappels-du-jour (§5.4) : dernière action "À rappeler" avec DateRelance
// <= aujourd'hui, toutes campagnes actives visibles par l'opérateur — indépendamment de qui a
// fixé le rappel (§10.8). Le déroulement est toujours "À rappeler" par construction du filtre,
// donc pas besoin de le porter ici : seul l'opérateur qui l'a fixé est utile à l'affichage.
public class RappelDuJour
{
    public long IdLCampagne { get; set; }
    public string NomCampagne { get; set; } = string.Empty;
    public string? RaisonSociale { get; set; }
    public string? Telephone { get; set; }
    public DateTime DateRelance { get; set; }
    public string? LoginOperateurDerniereAction { get; set; }
}
