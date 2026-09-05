using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Data.Context;
using CentreAppel.Web.Enum;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class RechercheClientService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    // Recherche par nom, NUMCLI ou téléphone (§5.5), sur toutes les campagnes actives ou
    // clôturées — sans filtre CampagnesOperateur : portée élargie à tous les rôles (§10.13),
    // un client qui rappelle de lui-même peut tomber sur n'importe quel conseiller. Aucun résultat
    // tant qu'un terme n'a pas été saisi : pas de liste exhaustive accessible.
    public async Task<List<ResultatRechercheClient>> SearchClientAsync(string terme, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(terme))
        {
            return [];
        }

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var termeMinuscule = terme.Trim().ToLower();
        var numCliRecherche = decimal.TryParse(terme.Trim(), out var numCli) ? numCli : (decimal?)null;

        return await (
            from ligne in context.LignesCampagne.AsNoTracking()
            where (ligne.RaisonSociale != null && ligne.RaisonSociale.ToLower().Contains(termeMinuscule))
               || (ligne.Telephone != null && ligne.Telephone.Contains(terme))
               || (numCliRecherche != null && ligne.NumCli == numCliRecherche)
            join campagne in context.Campagnes
                on ligne.IdCampagne equals campagne.IdCampagne
            where campagne.Statut == StatutCampagne.Active || campagne.Statut == StatutCampagne.Cloturee
            join derniereAction in context.DernieresActions
                on ligne.IdLCampagne equals derniereAction.IdLCampagne into actionsLigne
            from derniereAction in actionsLigne.DefaultIfEmpty()
            join deroulement in context.Deroulements
                on derniereAction.IdDeroulement equals deroulement.IdDeroulement into deroulements
            from deroulement in deroulements.DefaultIfEmpty()
            join operateur in context.Operateurs
                on derniereAction.IdOperateur equals operateur.IdOperateur into operateurs
            from operateur in operateurs.DefaultIfEmpty()
            orderby campagne.DateCampagne descending
#pragma warning disable CS0472 // (long?)x != null : idiome EF Core pour détecter un LEFT JOIN non apparié sur une entité sans clé (DerniereAction est keyless, "derniereAction != null" n'est pas traduisible).
            select new ResultatRechercheClient
            {
                IdLCampagne = ligne.IdLCampagne,
                NomCampagne = campagne.Nom,
                StatutCampagne = campagne.Statut,
                RaisonSociale = ligne.RaisonSociale,
                Telephone = ligne.Telephone,
                DateRelance = (long?)derniereAction.IdActionsCampagnes != null ? derniereAction.DateRelance : null,
                DeroulementDerniereActionCode = deroulement != null ? deroulement.Code : null,
                LoginOperateurDerniereAction = operateur != null ? operateur.LoginAd : null,
            }
#pragma warning restore CS0472
        ).ToListAsync(cancellationToken);
    }
}
