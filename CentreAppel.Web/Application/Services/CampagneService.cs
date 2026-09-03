using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Enum;
using CentreAppel.Web.Data.Context;
using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class CampagneService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ICampagneService
{
    public async Task<List<CampagneEnCours>?> GetCampagnesEnCoursAsync(long idOperateur, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Campagnes
                .AsNoTracking()
                .Where(c => c.Statut == StatutCampagne.Active)
                .Where(c => context.CampagnesOperateur.Any(co => co.IdCampagne == c.IdCampagne && co.IdOperateur == idOperateur))
                .OrderByDescending(c => c.DateCampagne)
                .Select(c => new CampagneEnCours
                {
                    IdCampagne = c.IdCampagne,
                    Nom = c.Nom,
                    DateCampagne = c.DateCampagne,
                    Description = c.Description,
                    Statut = c.Statut,
                    NbLignes = context.LignesCampagne.Count(l => l.IdCampagne == c.IdCampagne),
                    NbATraiter = context.LignesCampagne.Count(l => l.IdCampagne == c.IdCampagne && !context.ActionsCampagne.Any(a => a.IdLCampagne == l.IdLCampagne)),
                })
                .ToListAsync(cancellationToken);
    }

    public async Task<List<LigneCampagneEnCours>?> GetLigneCampagneEnCoursAsync(long idCampagne, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from ligne in context.LignesCampagne.AsNoTracking()
            where ligne.IdCampagne == idCampagne
            join derniereAction in context.DernieresActions
                on ligne.IdLCampagne equals derniereAction.IdLCampagne into actionsLigne
            from derniereAction in actionsLigne.DefaultIfEmpty()
            join typeContact in context.TypesContact
                on derniereAction.IdTypeContact equals typeContact.IdTypeContact into typesContact
            from typeContact in typesContact.DefaultIfEmpty()
            join deroulement in context.Deroulements
                on derniereAction.IdDeroulement equals deroulement.IdDeroulement into deroulements
            from deroulement in deroulements.DefaultIfEmpty()
            join interet in context.InteretsClient
                on derniereAction.IdInteret equals interet.IdInteret into interets
            from interet in interets.DefaultIfEmpty()
            join canal in context.CanauxAchat
                on derniereAction.IdCanal equals canal.IdCanal into canaux
            from canal in canaux.DefaultIfEmpty()
            join commentaire in context.CommentairesCampagne
                on derniereAction.IdCommentaire equals commentaire.IdCommentaire into commentaires
            from commentaire in commentaires.DefaultIfEmpty()
            join operateur in context.Operateurs
                on derniereAction.IdOperateur equals operateur.IdOperateur into operateurs
            from operateur in operateurs.DefaultIfEmpty()
            orderby ligne.NumLigne
#pragma warning disable CS0472 // (long?)x != null : idiome EF Core pour détecter un LEFT JOIN non apparié sur une entité sans clé (DerniereAction est keyless, "derniereAction != null" n'est pas traduisible).
            select new LigneCampagneEnCours
            {
                IdLCampagne = ligne.IdLCampagne,
                IdCampagne = ligne.IdCampagne,
                CodeSoc = ligne.CodeSoc,
                NumCli = ligne.NumCli,
                Rfm = ligne.Rfm,
                RaisonSociale = ligne.RaisonSociale,
                SousActivite = ligne.SousActivite,
                CaHt = ligne.CaHt,
                DateDernierAchat = ligne.DateDernierAchat,
                Correspondant = ligne.Correspondant,
                Telephone = ligne.Telephone,
                Email = ligne.Email,
                MagasinAffilie = ligne.MagasinAffilie,
                DateHeureContact = (long?)derniereAction.IdActionsCampagnes != null ? DateOnly.FromDateTime(derniereAction.DhAction) : null,
                TypeContactCode = typeContact != null ? typeContact.Code : null,
                DeroulementCode = deroulement != null ? deroulement.Code : null,
                DateRelance = (long?)derniereAction.IdActionsCampagnes != null ? derniereAction.DateRelance : null,
                InteretClientCode = interet != null ? interet.Code : null,
                CanalAchatCode = canal != null ? canal.Code : null,
                Commentaire = commentaire != null ? commentaire.Libelle : (long?)derniereAction.IdActionsCampagnes != null ? derniereAction.CommentaireLibre : null,
                NomOperateurEnCours = operateur != null ? $"{operateur.FirstName} {operateur.LastName}" : null
            }
#pragma warning restore CS0472
        ).ToListAsync(cancellationToken);
    }

    public async Task<List<CommentaireCampagne>> GetCommentairesCampagneAsync(long idCampagne, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.CommentairesCampagne
            .AsNoTracking()
            .Where(c => c.IdCampagne == idCampagne)
            .OrderBy(c => c.Libelle)
            .Select(c => new CommentaireCampagne
            {
                IdCommentaire = c.IdCommentaire,
                Libelle = c.Libelle
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<LigneCampagnePopup?> GetLigneCampagnePopupAsync(long idLCampagne, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from ligne in context.LignesCampagne.AsNoTracking()
            where ligne.IdLCampagne == idLCampagne
            join derniereAction in context.DernieresActions
                on ligne.IdLCampagne equals derniereAction.IdLCampagne into actionsLigne
            from derniereAction in actionsLigne.DefaultIfEmpty()
            select new LigneCampagnePopup
            {
                IdLCampagne = ligne.IdLCampagne,
                IdCampagne = ligne.IdCampagne,
                RaisonSociale = ligne.RaisonSociale ?? string.Empty,
                CodeSoc = ligne.CodeSoc,
                NumCli = ligne.NumCli,
                Telephone = ligne.Telephone,
                Rfm = ligne.Rfm,
                MagasinAffilie = ligne.MagasinAffilie,
                Correspondant = ligne.Correspondant,
                IdTypeContact = derniereAction.IdTypeContact,
                IdDeroulement = derniereAction.IdDeroulement,
                IdInteret = derniereAction.IdInteret,
                DateRelance = derniereAction.DateRelance,
                DateAchat = derniereAction.DateAchat,
                IdCanal = derniereAction.IdCanal,
                IdCommentaire = derniereAction.IdCommentaire,
            }
        ).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<HistoriqueAction>> GetHistoriqueAsync(long idLCampagne, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.ActionsCampagne
            .AsNoTracking()
            .Where(a => a.IdLCampagne == idLCampagne)
            .OrderByDescending(a => a.NumAction)
            .Select(a => new HistoriqueAction
            {
                DhAction = a.DhAction,
                LoginOperateur = a.Operateur.LoginAd,
                DeroulementCode = a.Deroulement.Code
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<long?> AcquireProchainContactAsync(long idCampagne, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.LignesCampagne
            .AsNoTracking()
            .Where(l => l.IdCampagne == idCampagne)
            .Where(l => !context.ActionsCampagne.Any(a => a.IdLCampagne == l.IdLCampagne))
            .OrderBy(l => l.DhCreation)
            .Select(l => (long?)l.IdLCampagne)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Enregistre une nouvelle tentative (ActionCampagne) pour la ligne — jamais de modification
    // de L_CAMPAGNES elle-même, l'historique reste un simple ajout de ligne (cf. spec : "un
    // bouton relance crée simplement une nouvelle ligne d'action").
    public async Task SaveActionAsync(SaisieAction saisie, string? commentaireLibre, CancellationToken cancellationToken)
    {
        if (saisie.IdTypeContact is null || saisie.IdDeroulement is null)
        {
            throw new InvalidOperationException("Type de contact et Déroulement sont obligatoires.");
        }

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        // NumAction n'est plus calculé ici : un trigger PostgreSQL (migration
        // AddTriggerNumActionParLigne) l'attribue à l'insertion, de façon atomique et valable
        // aussi pour un éventuel import direct en base.
        context.ActionsCampagne.Add(new ActionCampagneEntity
        {
            IdLCampagne = saisie.IdLCampagne,
            DhAction = DateTime.UtcNow,
            IdOperateur = saisie.IdOperateur,
            IdTypeContact = saisie.IdTypeContact.Value,
            IdDeroulement = saisie.IdDeroulement.Value,
            IdInteret = saisie.IdInteret,
            DateRelance = saisie.DateRelance,
            DateAchat = saisie.DateAchat,
            IdCanal = saisie.IdCanal,
            IdCommentaire = saisie.IdCommentaire,
            CommentaireLibre = commentaireLibre,
        });

        // Effet de bord obligatoire (même transaction) : "Ne plus contacter" exclut
        // définitivement le client de toutes les campagnes futures.
        var deroulement = await context.Deroulements.SingleAsync(d => d.IdDeroulement == saisie.IdDeroulement.Value, cancellationToken);
        if (deroulement.Code == CodesDeroulement.NePlusContacter)
        {
            var ligne = await context.LignesCampagne.SingleAsync(l => l.IdLCampagne == saisie.IdLCampagne, cancellationToken);
            var dejaExclu = await context.ClientsHorsContact
                .AnyAsync(c => c.Soc == ligne.CodeSoc && c.NumCli == ligne.NumCli, cancellationToken);
            if (!dejaExclu)
            {
                context.ClientsHorsContact.Add(new ClientHorsContactEntity
                {
                    Soc = ligne.CodeSoc,
                    NumCli = ligne.NumCli,
                    DateExclusion = DateOnly.FromDateTime(DateTime.UtcNow),
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
