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

        // Détermine l'affichage (champs saisissables, boutons visibles) - voir LectureSeule et
        // EnregistrerVisible ci-dessous.
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
        // et il n'y a rien à enregistrer.
        private bool LectureSeule => Mode == ModeOuverturePopup.Historique;
        private bool EnregistrerVisible => Mode != ModeOuverturePopup.Historique;

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

            // Modification : la ligne a déjà une dernière action, on repart de ses valeurs.
            // Création : LigneCampagnePopup.IdDeroulement est null (aucune action existante),
            // Saisie repart vide — sauf Type de contact, toujours pré-rempli sur la valeur par défaut.
            // IdOperateur est renseigné au moment de l'enregistrement (SaveAsync), pas ici.
            Saisie = new SaisieAction
            {
                IdLCampagne = idLCampagne,
                IdTypeContact = LigneCampagnePopup.IdTypeContact ?? TypesContact.FirstOrDefault(t => t.Defaut)?.IdTypeContact,
                IdDeroulement = LigneCampagnePopup.IdDeroulement,
                IdInteret = LigneCampagnePopup.IdInteret,
                DateRelance = LigneCampagnePopup.DateRelance,
                DateAchat = LigneCampagnePopup.DateAchat,
                IdCanal = LigneCampagnePopup.IdCanal,
                IdCommentaire = LigneCampagnePopup.IdCommentaire,
            };

            idLCampagneCharge = idLCampagne;
        }

        private async Task SaveAsync()
        {
            if (Saisie is null || Saisie.IdTypeContact is null || Saisie.IdDeroulement is null) return;

            var idOperateur = await GetIdOperateurConnecteAsync();
            if (idOperateur is null) return;

            Saisie.IdOperateur = idOperateur.Value;

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
