using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Data.Context;
using CentreAppel.Web.Enum;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class RappelsDuJourService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    // Critère de sélection (§5.4) : dernière action Déroulement = "À rappeler" ET DateRelance <=
    // maintenant, sur les campagnes ACTIVE visibles par l'opérateur (§10.8 : peu importe qui a
    // fixé le rappel). La spec compare à date_relance <= current_date (jour près) ; DateRelance
    // portant désormais une heure (cf. Documents/Spécifications à préciser.md), on compare à
    // l'instant précis plutôt qu'au jour civil — un rappel prévu à 16h aujourd'hui n'apparaît
    // qu'à partir de 16h, pas dès 00h00.
    public async Task<List<RappelDuJour>> GetRappelsDuJourAsync(long idOperateur, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var maintenant = DateTime.UtcNow;

        return await (
            from derniereAction in context.DernieresActions.AsNoTracking()
            join deroulement in context.Deroulements on derniereAction.IdDeroulement equals deroulement.IdDeroulement
            where deroulement.Code == CodesDeroulement.ARappeler && derniereAction.DateRelance != null && derniereAction.DateRelance <= maintenant
            join ligne in context.LignesCampagne on derniereAction.IdLCampagne equals ligne.IdLCampagne
            join campagne in context.Campagnes on ligne.IdCampagne equals campagne.IdCampagne
            where campagne.Statut == StatutCampagne.Active
               && context.CampagnesOperateur.Any(co => co.IdCampagne == campagne.IdCampagne && co.IdOperateur == idOperateur)
            join operateur in context.Operateurs on derniereAction.IdOperateur equals operateur.IdOperateur
            orderby derniereAction.DateRelance
            select new RappelDuJour
            {
                IdLCampagne = ligne.IdLCampagne,
                NomCampagne = campagne.Nom,
                RaisonSociale = ligne.RaisonSociale,
                Telephone = ligne.Telephone,
                DateRelance = derniereAction.DateRelance!.Value,
                LoginOperateurDerniereAction = operateur.LoginAd,
            }
        ).ToListAsync(cancellationToken);
    }

    // Alimente le badge permanent sur l'onglet du menu (§5.4) — requête dédiée (pas de recyclage
    // de GetRappelsDuJourAsync().Count) car interrogée en polling depuis toutes les pages, sans
    // besoin de matérialiser les colonnes d'affichage à chaque fois.
    public async Task<int> CountRappelsDuJourAsync(long idOperateur, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var maintenant = DateTime.UtcNow;

        return await (
            from derniereAction in context.DernieresActions.AsNoTracking()
            join deroulement in context.Deroulements on derniereAction.IdDeroulement equals deroulement.IdDeroulement
            where deroulement.Code == CodesDeroulement.ARappeler && derniereAction.DateRelance != null && derniereAction.DateRelance <= maintenant
            join ligne in context.LignesCampagne on derniereAction.IdLCampagne equals ligne.IdLCampagne
            join campagne in context.Campagnes on ligne.IdCampagne equals campagne.IdCampagne
            where campagne.Statut == StatutCampagne.Active
               && context.CampagnesOperateur.Any(co => co.IdCampagne == campagne.IdCampagne && co.IdOperateur == idOperateur)
            select ligne.IdLCampagne
        ).CountAsync(cancellationToken);
    }
}
