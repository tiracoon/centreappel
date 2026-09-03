using CentreAppel.Web.Data.Entites;
using CentreAppel.Web.Enum;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Data.Context;

// Jeu de données de test couvrant systématiquement les cas de SuiviCampagnes/PopupSaisieAction
// (statuts de campagne, dépendances conditionnelles de saisie, historique, prochain contact).
// Idempotent : relançable sans dupliquer (recherche par Code/LoginAd/Nom avant création).
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var roleAdmin = await GetOrCreateRoleAsync(context, "ADMINISTRATEUR", cancellationToken);
        var roleConseiller = await GetOrCreateRoleAsync(context, "CONSEILLER", cancellationToken);

        var spetit = await GetOrCreateOperateurAsync(context, "spetit", "Sophie", "Petit", roleAdmin.IdRole, cancellationToken);
        var mmartin = await GetOrCreateOperateurAsync(context, "mmartin", "Marie", "Martin", roleConseiller.IdRole, cancellationToken);

        await SeedParametreSiAbsentAsync(context, "POLLING_SECONDES", 5, cancellationToken);
        await SeedParametreSiAbsentAsync(context, "VERROU_EXPIRATION_MINUTES", 10, cancellationToken);
        await SeedParametreSiAbsentAsync(context, "ARCHIVAGE_AUTO_JOURS", 365, cancellationToken);
        await SeedParametreSiAbsentAsync(context, "CONSERVATION_RGPD_JOURS", 1095, cancellationToken);

        var typeAppel = await GetOrCreateTypeContactAsync(context, "APPEL", defaut: true, cancellationToken);
        var typeEmail = await GetOrCreateTypeContactAsync(context, "EMAIL", defaut: false, cancellationToken);
        var typeSms = await GetOrCreateTypeContactAsync(context, "SMS", defaut: false, cancellationToken);
        var typeCourrier = await GetOrCreateTypeContactAsync(context, "COURRIER", defaut: false, cancellationToken);

        var dNumeroNonAttribue = await GetOrCreateDeroulementAsync(context, "NUMERO_NON_ATTRIBUE", cancellationToken);
        var dFauxNumero = await GetOrCreateDeroulementAsync(context, "FAUX_NUMERO", cancellationToken);
        var dEntrepriseFermee = await GetOrCreateDeroulementAsync(context, "ENTREPRISE_FERMEE", cancellationToken);
        var dMauvaisInterlocuteur = await GetOrCreateDeroulementAsync(context, "MAUVAIS_INTERLOCUTEUR", cancellationToken);
        var dDoublon = await GetOrCreateDeroulementAsync(context, "DOUBLON", cancellationToken);
        var dRepondeur = await GetOrCreateDeroulementAsync(context, "REPONDEUR", cancellationToken);
        var dARappeler = await GetOrCreateDeroulementAsync(context, "A_RAPPELER", cancellationToken);
        var dContactArgumente = await GetOrCreateDeroulementAsync(context, "CONTACT_ARGUMENTE", cancellationToken);
        var dNePlusContacter = await GetOrCreateDeroulementAsync(context, "NE_PLUS_CONTACTER", cancellationToken);

        var iRefractaire = await GetOrCreateInteretAsync(context, "REFRACTAIRE", cancellationToken);
        var iInteresseWeb = await GetOrCreateInteretAsync(context, "INTERESSE_WEB", cancellationToken);
        var iInteresseMag = await GetOrCreateInteretAsync(context, "INTERESSE_MAG", cancellationToken);
        var iVenteValidee = await GetOrCreateInteretAsync(context, "VENTE_VALIDEE", cancellationToken);

        var cWeb = await GetOrCreateCanalAsync(context, "WEB", cancellationToken);
        var cMagasin = await GetOrCreateCanalAsync(context, "MAGASIN", cancellationToken);

        await SeedCampagneClotureeAsync(context, spetit, mmartin, typeAppel, dRepondeur, dContactArgumente, iInteresseMag, cancellationToken);
        await SeedCampagneArchiveeAsync(context, spetit, mmartin, typeAppel, dDoublon, dARappeler, cancellationToken);
        await SeedCampagnePreparationSansContactAsync(context, spetit, cancellationToken);
        await SeedCampagnePreparationAvecContactsAsync(context, spetit, cancellationToken);
        await SeedCampagneActiveSansLigneATraiterAsync(context, spetit, mmartin, typeAppel, dRepondeur, cancellationToken);
        await SeedCampagneActiveVideAsync(context, spetit, mmartin, "26_402 - Active vide B (test)", new DateOnly(2026, 8, 5), cancellationToken);
        await SeedCampagneVolumeEtProchainContactAsync(context, spetit, mmartin, cancellationToken);
        await SeedCampagneTousLesCasAsync(
            context, spetit, mmartin,
            typeAppel, typeEmail, typeSms, typeCourrier,
            dNumeroNonAttribue, dFauxNumero, dEntrepriseFermee, dMauvaisInterlocuteur, dDoublon, dRepondeur, dARappeler, dContactArgumente, dNePlusContacter,
            iInteresseWeb, iInteresseMag, iRefractaire, iVenteValidee,
            cWeb, cMagasin,
            cancellationToken);
    }

    // --- Référentiels (idempotents par Code/LoginAd) --------------------------------------

    private static async Task<RoleEntity> GetOrCreateRoleAsync(ApplicationDbContext context, string code, CancellationToken cancellationToken)
    {
        var role = await context.Roles.SingleOrDefaultAsync(r => r.Code == code, cancellationToken);
        if (role is not null) return role;

        role = new RoleEntity { Code = code };
        context.Roles.Add(role);
        await context.SaveChangesAsync(cancellationToken);
        return role;
    }

    private static async Task<OperateurEntity> GetOrCreateOperateurAsync(ApplicationDbContext context, string loginAd, string firstName, string lastName, int idRole, CancellationToken cancellationToken)
    {
        var operateur = await context.Operateurs.SingleOrDefaultAsync(o => o.LoginAd == loginAd, cancellationToken);
        if (operateur is not null) return operateur;

        operateur = new OperateurEntity { LoginAd = loginAd, FirstName = firstName, LastName = lastName, IdRole = idRole };
        context.Operateurs.Add(operateur);
        await context.SaveChangesAsync(cancellationToken);
        return operateur;
    }

    private static async Task SeedParametreSiAbsentAsync(ApplicationDbContext context, string libelle, decimal valeurNum, CancellationToken cancellationToken)
    {
        if (await context.Parametres.AnyAsync(p => p.Libelle == libelle, cancellationToken)) return;

        context.Parametres.Add(new ParametreEntity { Libelle = libelle, ValeurNum = valeurNum });
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<TypeContactEntity> GetOrCreateTypeContactAsync(ApplicationDbContext context, string code, bool defaut, CancellationToken cancellationToken)
    {
        var typeContact = await context.TypesContact.SingleOrDefaultAsync(t => t.Code == code, cancellationToken);
        if (typeContact is not null) return typeContact;

        typeContact = new TypeContactEntity { Code = code, Defaut = defaut };
        context.TypesContact.Add(typeContact);
        await context.SaveChangesAsync(cancellationToken);
        return typeContact;
    }

    private static async Task<DeroulementEntity> GetOrCreateDeroulementAsync(ApplicationDbContext context, string code, CancellationToken cancellationToken)
    {
        var deroulement = await context.Deroulements.SingleOrDefaultAsync(d => d.Code == code, cancellationToken);
        if (deroulement is not null) return deroulement;

        deroulement = new DeroulementEntity { Code = code };
        context.Deroulements.Add(deroulement);
        await context.SaveChangesAsync(cancellationToken);
        return deroulement;
    }

    private static async Task<InteretClientEntity> GetOrCreateInteretAsync(ApplicationDbContext context, string code, CancellationToken cancellationToken)
    {
        var interet = await context.InteretsClient.SingleOrDefaultAsync(i => i.Code == code, cancellationToken);
        if (interet is not null) return interet;

        interet = new InteretClientEntity { Code = code };
        context.InteretsClient.Add(interet);
        await context.SaveChangesAsync(cancellationToken);
        return interet;
    }

    private static async Task<CanalAchatEntity> GetOrCreateCanalAsync(ApplicationDbContext context, string code, CancellationToken cancellationToken)
    {
        var canal = await context.CanauxAchat.SingleOrDefaultAsync(c => c.Code == code, cancellationToken);
        if (canal is not null) return canal;

        canal = new CanalAchatEntity { Code = code };
        context.CanauxAchat.Add(canal);
        await context.SaveChangesAsync(cancellationToken);
        return canal;
    }

    // --- Campagnes de test ------------------------------------------------------------------

    // Cas 1 : campagne Clôturée avec 3 contacts, chacun 2 lignes d'historique.
    private static async Task SeedCampagneClotureeAsync(
        ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin,
        TypeContactEntity typeAppel, DeroulementEntity dRepondeur, DeroulementEntity dContactArgumente, InteretClientEntity iInteresseMag,
        CancellationToken cancellationToken)
    {
        const string nom = "26_101 - Campagne clôturée (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 3, 1),
            Description = "Jeu de test : campagne clôturée, 3 contacts avec historique.",
            Statut = StatutCampagne.Cloturee,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        for (var n = 1; n <= 3; n++)
        {
            var ligne = NouvelleLigne(campagne.IdCampagne, n, 510000 + n, $"TEST Clôturée - Contact {n}", spetit.IdOperateur);
            context.LignesCampagne.Add(ligne);
            await context.SaveChangesAsync(cancellationToken);

            context.ActionsCampagne.AddRange(
                NouvelleAction(ligne.IdLCampagne, new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc), mmartin.IdOperateur, typeAppel.IdTypeContact, dRepondeur.IdDeroulement, mmartin.IdOperateur),
                NouvelleAction(ligne.IdLCampagne, new DateTime(2026, 2, 25, 14, 30, 0, DateTimeKind.Utc), spetit.IdOperateur, typeAppel.IdTypeContact, dContactArgumente.IdDeroulement, spetit.IdOperateur, idInteret: iInteresseMag.IdInteret));
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    // Cas 2 : campagne Archivée avec 3 contacts, chacun 2 lignes d'historique.
    private static async Task SeedCampagneArchiveeAsync(
        ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin,
        TypeContactEntity typeAppel, DeroulementEntity dDoublon, DeroulementEntity dARappeler,
        CancellationToken cancellationToken)
    {
        const string nom = "25_201 - Campagne archivée (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2025, 11, 1),
            Description = "Jeu de test : campagne archivée, 3 contacts avec historique.",
            Statut = StatutCampagne.Archivee,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        for (var n = 1; n <= 3; n++)
        {
            var ligne = NouvelleLigne(campagne.IdCampagne, n, 520000 + n, $"TEST Archivée - Contact {n}", spetit.IdOperateur);
            context.LignesCampagne.Add(ligne);
            await context.SaveChangesAsync(cancellationToken);

            context.ActionsCampagne.AddRange(
                NouvelleAction(ligne.IdLCampagne, new DateTime(2025, 10, 15, 10, 0, 0, DateTimeKind.Utc), mmartin.IdOperateur, typeAppel.IdTypeContact, dARappeler.IdDeroulement, mmartin.IdOperateur, dateRelance: new DateOnly(2025, 10, 22)),
                NouvelleAction(ligne.IdLCampagne, new DateTime(2025, 10, 22, 11, 15, 0, DateTimeKind.Utc), spetit.IdOperateur, typeAppel.IdTypeContact, dDoublon.IdDeroulement, spetit.IdOperateur));
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    // Cas 3 : campagne En préparation sans aucun contact.
    private static async Task SeedCampagnePreparationSansContactAsync(ApplicationDbContext context, OperateurEntity spetit, CancellationToken cancellationToken)
    {
        const string nom = "26_301 - Préparation sans contact (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        context.Campagnes.Add(new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 9, 1),
            Description = "Jeu de test : campagne en préparation, aucune ligne.",
            Statut = StatutCampagne.EnPreparation,
            IdOperateurCm = spetit.IdOperateur,
        });
        await context.SaveChangesAsync(cancellationToken);
    }

    // Cas 4 : campagne En préparation avec des contacts (aucune action, campagne pas encore active).
    private static async Task SeedCampagnePreparationAvecContactsAsync(ApplicationDbContext context, OperateurEntity spetit, CancellationToken cancellationToken)
    {
        const string nom = "26_302 - Préparation avec contacts (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 9, 15),
            Description = "Jeu de test : campagne en préparation, contacts déjà importés.",
            Statut = StatutCampagne.EnPreparation,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        for (var n = 1; n <= 3; n++)
        {
            context.LignesCampagne.Add(NouvelleLigne(campagne.IdCampagne, n, 530000 + n, $"TEST Préparation - Contact {n}", spetit.IdOperateur));
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    // Cas 5 : campagne Active avec 5 lignes toutes déjà traitées (aucune ligne "à traiter" -
    // NbATraiter compte les lignes sans aucune ActionCampagne, cf. CampagneService).
    private static async Task SeedCampagneActiveSansLigneATraiterAsync(
        ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin,
        TypeContactEntity typeAppel, DeroulementEntity dRepondeur,
        CancellationToken cancellationToken)
    {
        const string nom = "26_401 - Active sans ligne à traiter (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 8, 1),
            Description = "Jeu de test : campagne active, 5 lignes toutes déjà traitées (aucune ligne à traiter).",
            Statut = StatutCampagne.Active,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        for (var n = 1; n <= 5; n++)
        {
            var ligne = NouvelleLigne(campagne.IdCampagne, n, 540000 + n, $"TEST Sans à traiter - Contact {n}", spetit.IdOperateur);
            context.LignesCampagne.Add(ligne);
            await context.SaveChangesAsync(cancellationToken);

            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, new DateTime(2026, 7, 28, 10, 0, 0, DateTimeKind.Utc), spetit.IdOperateur, typeAppel.IdTypeContact, dRepondeur.IdDeroulement, spetit.IdOperateur));
            await context.SaveChangesAsync(cancellationToken);
        }

        await AjouterVisibiliteAsync(context, campagne.IdCampagne, spetit, mmartin, cancellationToken);
    }

    // Cas 6 : campagne Active sans aucun contact.
    private static async Task SeedCampagneActiveVideAsync(ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin, string nom, DateOnly dateCampagne, CancellationToken cancellationToken)
    {
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = dateCampagne,
            Description = "Jeu de test : campagne active sans aucune ligne.",
            Statut = StatutCampagne.Active,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        await AjouterVisibiliteAsync(context, campagne.IdCampagne, spetit, mmartin, cancellationToken);
    }

    // Cas "10 contacts pour la pagination" + "5 contacts non traités pour Prochain contact".
    // Les 10 lignes sont insérées via des SaveChangesAsync séparés pour obtenir des DhCreation
    // distincts (le timestamp est calculé une seule fois par SaveChangesAsync dans
    // ApplicationDbContext) — indispensable pour vérifier un tri déterministe côté
    // AcquireProchainContactAsync (ORDER BY dh_creation).
    private static async Task SeedCampagneVolumeEtProchainContactAsync(ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin, CancellationToken cancellationToken)
    {
        const string nom = "26_501 - Volume et prochain contact (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 8, 10),
            Description = "Jeu de test : 10 contacts non traités (pagination + ordre de Prochain contact).",
            Statut = StatutCampagne.Active,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        for (var n = 1; n <= 10; n++)
        {
            context.LignesCampagne.Add(NouvelleLigne(campagne.IdCampagne, n, 550000 + n, $"TEST Volume - Contact {n:00}", spetit.IdOperateur));
            await context.SaveChangesAsync(cancellationToken);
        }

        await AjouterVisibiliteAsync(context, campagne.IdCampagne, spetit, mmartin, cancellationToken);
    }

    // Cas 7 : une campagne Active avec un contact par état possible des dépendances
    // conditionnelles de saisie, plus les cas historique vide/volumineux.
    private static async Task SeedCampagneTousLesCasAsync(
        ApplicationDbContext context, OperateurEntity spetit, OperateurEntity mmartin,
        TypeContactEntity typeAppel, TypeContactEntity typeEmail, TypeContactEntity typeSms, TypeContactEntity typeCourrier,
        DeroulementEntity dNumeroNonAttribue, DeroulementEntity dFauxNumero, DeroulementEntity dEntrepriseFermee, DeroulementEntity dMauvaisInterlocuteur,
        DeroulementEntity dDoublon, DeroulementEntity dRepondeur, DeroulementEntity dARappeler, DeroulementEntity dContactArgumente, DeroulementEntity dNePlusContacter,
        InteretClientEntity iInteresseWeb, InteretClientEntity iInteresseMag, InteretClientEntity iRefractaire, InteretClientEntity iVenteValidee,
        CanalAchatEntity cWeb, CanalAchatEntity cMagasin,
        CancellationToken cancellationToken)
    {
        const string nom = "26_601 - Tous les cas de saisie (test)";
        if (await context.Campagnes.AnyAsync(c => c.Nom == nom, cancellationToken)) return;

        var campagne = new CampagneEntity
        {
            Nom = nom,
            DateCampagne = new DateOnly(2026, 8, 15),
            Description = "Jeu de test : un contact par état des dépendances conditionnelles de saisie.",
            Statut = StatutCampagne.Active,
            IdOperateurCm = spetit.IdOperateur,
        };
        context.Campagnes.Add(campagne);
        await context.SaveChangesAsync(cancellationToken);

        var n = 0;
        async Task<LigneCampagneEntity> AjouterLigneAsync(string libelle)
        {
            n++;
            var ligne = NouvelleLigne(campagne.IdCampagne, n, 560000 + n, libelle, spetit.IdOperateur);
            context.LignesCampagne.Add(ligne);
            await context.SaveChangesAsync(cancellationToken);
            return ligne;
        }

        var jour = new DateTime(2026, 8, 20, 9, 0, 0, DateTimeKind.Utc);

        // 1 à 6 : déroulements sans dépendance conditionnelle.
        foreach (var (libelle, deroulement) in new[]
        {
            ("TEST Déroulement - Numéro non attribué", dNumeroNonAttribue),
            ("TEST Déroulement - Faux numéro", dFauxNumero),
            ("TEST Déroulement - Entreprise fermée", dEntrepriseFermee),
            ("TEST Déroulement - Mauvais interlocuteur", dMauvaisInterlocuteur),
            ("TEST Déroulement - Doublon", dDoublon),
            ("TEST Déroulement - Répondeur", dRepondeur),
        })
        {
            var ligne = await AjouterLigneAsync(libelle);
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeAppel.IdTypeContact, deroulement.IdDeroulement, spetit.IdOperateur));
            await context.SaveChangesAsync(cancellationToken);
        }

        // 7 : À rappeler -> Date de relance saisissable.
        {
            var ligne = await AjouterLigneAsync("TEST Déroulement - À rappeler");
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeAppel.IdTypeContact, dARappeler.IdDeroulement, spetit.IdOperateur,
                dateRelance: DateOnly.FromDateTime(jour).AddDays(3)));
            await context.SaveChangesAsync(cancellationToken);
        }

        // 8 : Contact argumenté + Intérêt (sans vente) -> Intérêt saisissable, reste grisé.
        {
            var ligne = await AjouterLigneAsync("TEST Déroulement - Contact argumenté (Intéressé Web)");
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeAppel.IdTypeContact, dContactArgumente.IdDeroulement, spetit.IdOperateur,
                idInteret: iInteresseWeb.IdInteret));
            await context.SaveChangesAsync(cancellationToken);
        }

        // 9 : Contact argumenté + Vente validée -> chaîne complète (Date d'achat + Canal saisissables).
        {
            var ligne = await AjouterLigneAsync("TEST Déroulement - Contact argumenté (Vente validée)");
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeAppel.IdTypeContact, dContactArgumente.IdDeroulement, spetit.IdOperateur,
                idInteret: iVenteValidee.IdInteret,
                dateAchat: DateOnly.FromDateTime(jour).AddDays(-1),
                idCanal: cWeb.IdCanal));
            await context.SaveChangesAsync(cancellationToken);
        }

        // 10 : Ne plus contacter -> Intérêt/Date de relance/Date d'achat/Canal présents en base
        // (pour vérifier que l'UI les grise/masque malgré la donnée) + insertion dans ClientsHorsContact.
        {
            var ligne = await AjouterLigneAsync("TEST Déroulement - Ne plus contacter");
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeAppel.IdTypeContact, dNePlusContacter.IdDeroulement, spetit.IdOperateur,
                idInteret: iInteresseMag.IdInteret,
                dateRelance: DateOnly.FromDateTime(jour).AddDays(5),
                dateAchat: DateOnly.FromDateTime(jour).AddDays(-2),
                idCanal: cMagasin.IdCanal));
            context.ClientsHorsContact.Add(new ClientHorsContactEntity
            {
                Soc = ligne.CodeSoc,
                NumCli = ligne.NumCli,
                DateExclusion = DateOnly.FromDateTime(jour),
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        // 11 à 14 : un contact dédié par Type de contact (Déroulement neutre Répondeur).
        foreach (var (libelle, typeContact) in new[]
        {
            ("TEST Type de contact - Appel", typeAppel),
            ("TEST Type de contact - Email", typeEmail),
            ("TEST Type de contact - SMS", typeSms),
            ("TEST Type de contact - Courrier", typeCourrier),
        })
        {
            var ligne = await AjouterLigneAsync(libelle);
            context.ActionsCampagne.Add(NouvelleAction(ligne.IdLCampagne, jour, spetit.IdOperateur, typeContact.IdTypeContact, dRepondeur.IdDeroulement, spetit.IdOperateur));
            await context.SaveChangesAsync(cancellationToken);
        }

        // 15 : sans aucun historique.
        await AjouterLigneAsync("TEST Historique - Aucune action");

        // 16 : 10 lignes d'historique.
        {
            var ligne = await AjouterLigneAsync("TEST Historique - 10 actions");
            var deroulementsCycle = new[] { dRepondeur, dDoublon, dFauxNumero, dMauvaisInterlocuteur, dEntrepriseFermee, dNumeroNonAttribue, dARappeler, dRepondeur, dDoublon, dContactArgumente };
            for (var i = 0; i < 10; i++)
            {
                var operateurAction = i % 2 == 0 ? spetit : mmartin;
                var deroulement = deroulementsCycle[i];
                context.ActionsCampagne.Add(NouvelleAction(
                    ligne.IdLCampagne, jour.AddDays(i - 10), operateurAction.IdOperateur, typeAppel.IdTypeContact, deroulement.IdDeroulement, operateurAction.IdOperateur,
                    idInteret: deroulement == dContactArgumente ? iInteresseWeb.IdInteret : null));
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        await AjouterVisibiliteAsync(context, campagne.IdCampagne, spetit, mmartin, cancellationToken);
    }

    private static async Task AjouterVisibiliteAsync(ApplicationDbContext context, long idCampagne, OperateurEntity spetit, OperateurEntity mmartin, CancellationToken cancellationToken)
    {
        context.CampagnesOperateur.Add(new CampagneOperateurEntity { IdCampagne = idCampagne, IdOperateur = spetit.IdOperateur });
        context.CampagnesOperateur.Add(new CampagneOperateurEntity { IdCampagne = idCampagne, IdOperateur = mmartin.IdOperateur });
        await context.SaveChangesAsync(cancellationToken);
    }

    // --- Fabriques de lignes/actions ---------------------------------------------------------

    private static LigneCampagneEntity NouvelleLigne(long idCampagne, int numLigne, decimal numCli, string raisonSociale, long idOperateurCm, long? idOperateurAssigne = null)
    {
        return new LigneCampagneEntity
        {
            IdCampagne = idCampagne,
            NumLigne = numLigne,
            CodeSoc = "RET",
            NumCli = numCli,
            RaisonSociale = raisonSociale,
            SousActivite = "Restauration traditionnelle",
            MagasinAffilie = "Metro Test",
            Correspondant = "Contact Test",
            Telephone = "+33472000000",
            Email = $"contact.{numCli}@test.fr",
            Adresse = "1 rue de Test",
            Cp = "75000",
            Ville = "Paris",
            Pays = "France",
            Langue = "fr",
            Rfm = "3-3-3",
            CaHt = 10000m,
            DateDernierAchat = new DateOnly(2026, 6, 1),
            IdOperateurAssigne = idOperateurAssigne,
            IdOperateurCm = idOperateurCm,
        };
    }

    // NumAction n'est pas fourni : un trigger PostgreSQL (migration AddTriggerNumActionParLigne)
    // l'attribue à l'insertion, atomiquement, à partir du MAX existant pour la ligne.
    private static ActionCampagneEntity NouvelleAction(
        long idLCampagne, DateTime dhAction, long idOperateur, int idTypeContact, int idDeroulement, long idOperateurCm,
        int? idInteret = null, DateOnly? dateRelance = null, DateOnly? dateAchat = null, int? idCanal = null, string? commentaireLibre = null)
    {
        return new ActionCampagneEntity
        {
            IdLCampagne = idLCampagne,
            DhAction = dhAction,
            IdOperateur = idOperateur,
            IdTypeContact = idTypeContact,
            IdDeroulement = idDeroulement,
            IdInteret = idInteret,
            DateRelance = dateRelance,
            DateAchat = dateAchat,
            IdCanal = idCanal,
            CommentaireLibre = commentaireLibre,
            IdOperateurCm = idOperateurCm,
        };
    }
}
