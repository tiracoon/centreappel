using CentreAppel.Web.Application.Extensions;
using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Enum;
using Microsoft.AspNetCore.Components;

namespace CentreAppel.Web.Components.Pages.Shared
{
    public partial class PopupSaisieAction : LocalizedPage
    {
        [Parameter]
        public long? IdLCampagne { get; set; }

        // Détermine l'affichage (champs saisissables, boutons visibles) - voir LectureSeule ci-dessous.
        [Parameter, EditorRequired]
        public ModeOuverturePopup Mode { get; set; }

        [Inject]
        private ICampagneService CampagneService { get; set; } = default!;

        [Inject]
        private ClientService ClientService { get; set; } = default!;

        [Inject]
        private CanalAchatService CanalAchatService { get; set; } = default!;

        [Inject]
        private DeroulementService DeroulementService { get; set; } = default!;

        [Inject]
        private ILogger<PopupSaisieAction> Logger { get; set; } = default!;

        private LigneCampagnePopup? LigneCampagnePopup;
        private List<HistoriqueAction> Historique = [];
        private List<Deroulement> Deroulements = [];
        private List<TypeContact> TypesContact = [];
        private List<InteretClient> Interets = [];
        private List<CanalAchat> Canaux = [];
        private List<CommentaireCampagne> Commentaires = [];

        private SaisieAction? Saisie;

        private long? idLCampagneCharge;

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        private string? CodeDeroulementSelectionne => Deroulements.FirstOrDefault(d => d.IdDeroulement == Saisie?.IdDeroulement)?.Code;
        private string? CodeInteretSelectionne => Interets.FirstOrDefault(i => i.IdInteret == Saisie?.IdInteret)?.Code;

        private bool InteretSaisissable => CodeDeroulementSelectionne == CodesDeroulement.ContactArgumente;
        private bool DateRelanceSaisissable => CodeDeroulementSelectionne == CodesDeroulement.ARappeler;
        private bool VenteValidee => CodeInteretSelectionne == CodesInteret.VenteValidee;

        // Historique = consultation uniquement (§10.5) : tous les champs de saisie sont désactivés
        // et il n'y a rien à enregistrer (bouton Enregistrer remplacé par Fermer, cf. .razor).
        private bool LectureSeule => Mode == ModeOuverturePopup.Historique;

        // Appelé après tout changement de Déroulement ou d'Intérêt : vide les champs devenus
        // non saisissables. "Ne plus contacter" n'a pas de cas particulier — ce n'est ni
        // Contact argumenté ni À rappeler, donc Intérêt/DateRelance sont déjà vidés par les
        // deux premiers tests, ce qui vide ensuite DateAchat/Canal via VenteValidee devenu faux.
        private void ReinitialiserChampsNonSaisissables()
        {
            if (Saisie is null) return;

            if (!InteretSaisissable)
            {
                Saisie.IdInteret = null;
            }

            if (!DateRelanceSaisissable)
            {
                Saisie.DateRelance = null;
            }

            if (!VenteValidee)
            {
                Saisie.DateAchat = null;
                Saisie.IdCanal = null;
            }
        }

        private void OnDeroulementChange() => ReinitialiserChampsNonSaisissables();

        private void OnInteretChange() => ReinitialiserChampsNonSaisissables();

        private string LibelleDeroulement(Deroulement deroulement) => Localizer[$"Deroulement_{deroulement.Code}"];

        private string LibelleInteret(InteretClient interet) => Localizer[$"Interet_{interet.Code}"];

        private string LibelleTypeContact(TypeContact typeContact) => Localizer[$"TypeContact_{typeContact.Code}"];

        private string LibelleCanalAchat(CanalAchat canal) => Localizer[$"CanalAchat_{canal.Code}"];

        private string LibelleHistoriqueDeroulement(HistoriqueAction action) => Localizer[$"Deroulement_{action.DeroulementCode}"];

        protected override async Task OnParametersSetAsync()
        {
            if (IdLCampagne is null || IdLCampagne == idLCampagneCharge) // a voir....
            {
                return;
            }

            await LoadAsync(IdLCampagne.Value);
        }

        private async Task LoadAsync(long idLCampagne)
        {
            LigneCampagnePopup = await CampagneService.GetLigneCampagnePopupAsync(idLCampagne, CancellationToken.None);
            if (LigneCampagnePopup is null)
            {
                return;
            }

            Historique = await CampagneService.GetHistoriqueAsync(idLCampagne, CancellationToken.None);
            Deroulements = await DeroulementService.GetDeroulementsAsync(CancellationToken.None);
            TypesContact = await ClientService.GetTypesContactAsync(CancellationToken.None);
            Interets = await ClientService.GetInteretsClientAsync(CancellationToken.None);
            Canaux = await CanalAchatService.GetCanauxAchatAsync(CancellationToken.None);
            Commentaires = await CampagneService.GetCommentairesCampagneAsync(LigneCampagnePopup.IdCampagne, CancellationToken.None);

            // Historique (lecture seule) : on montre les valeurs de la dernière action existante,
            // rien à créer. Tous les autres modes (Relance, Prochain contact, Traiter) ouvrent une
            // nouvelle tentative — voir CreerNouvelleSaisieAction.
            // IdOperateur est renseigné au moment de l'enregistrement (SaveAsync), pas ici.
            Saisie = LectureSeule
                ? new SaisieAction
                {
                    IdLCampagne = idLCampagne,
                    IdTypeContact = LigneCampagnePopup.IdTypeContact,
                    IdDeroulement = LigneCampagnePopup.IdDeroulement,
                    IdInteret = LigneCampagnePopup.IdInteret,
                    // DateRelance est stockée en UTC ; Saisie.DateRelance alimente directement le
                    // <input type="datetime-local"> (@bind), qui doit recevoir une heure locale.
                    DateRelance = LigneCampagnePopup.DateRelance?.UtcVersHeureLocale(),
                    DateAchat = LigneCampagnePopup.DateAchat,
                    IdCanal = LigneCampagnePopup.IdCanal,
                    IdCommentaire = LigneCampagnePopup.IdCommentaire,
                }
                : CreerNouvelleSaisieAction(idLCampagne);

            idLCampagneCharge = idLCampagne;
        }

        // TODO à préciser dans les specs : seul Type de contact a un défaut écrit noir sur blanc
        // (§5.3 : "toujours — pré-rempli à Appel"). Pour tous les autres champs (Déroulement,
        // Intérêt, Date de relance, Date d'achat, Canal, Commentaire), rien n'indique de valeur par
        // défaut pour une nouvelle tentative (Relance notamment) — ils restent donc vides ici, pour
        // que l'opérateur choisisse explicitement plutôt que d'hériter par erreur d'une valeur
        // pré-sélectionnée sur un contact qui n'a pas encore eu lieu. À confirmer avec le métier.
        private SaisieAction CreerNouvelleSaisieAction(long idLCampagne)
        {
            return new SaisieAction
            {
                IdLCampagne = idLCampagne,
                IdTypeContact = TypesContact.FirstOrDefault(t => t.Defaut)?.IdTypeContact,
            };
        }

        private async Task SaveAsync()
        {
            if (Saisie is null || Saisie.IdTypeContact is null || Saisie.IdDeroulement is null) return;

            var idOperateur = await GetIdOperateurConnecteAsync();
            if (idOperateur is null) return;

            Saisie.IdOperateur = idOperateur.Value;

            // Saisie.DateRelance vient du <input type="datetime-local"> : heure locale saisie par
            // l'opérateur, à convertir en UTC avant stockage (colonne timestamptz).
            if (Saisie.DateRelance is not null)
            {
                Saisie.DateRelance = Saisie.DateRelance.Value.HeureLocaleVersUtc();
            }

            Logger.LogInformation("Opérateur {IdOperateur} clique sur Enregistrer pour la ligne {IdLCampagne}", idOperateur, Saisie.IdLCampagne);

            await CampagneService.SaveActionAsync(Saisie, commentaireLibre: null, CancellationToken.None);

            await OnSave.InvokeAsync();
        }

        private async Task CancelAsync()
        {
            Logger.LogInformation("Opérateur {IdOperateur} clique sur Annuler pour la ligne {IdLCampagne}", await GetIdOperateurConnecteAsync(), IdLCampagne);
            await OnClose.InvokeAsync();
        }
    }
}
