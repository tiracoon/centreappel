using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Enum;
using Microsoft.AspNetCore.Components;

namespace CentreAppel.Web.Components.Pages.SuiviCampagnes
{
    public partial class SuiviCampagnes() : LocalizedPage, IDisposable
    {
        private static readonly TimeSpan IntervalleRafraichissement = TimeSpan.FromSeconds(5000);

        [Inject]
        private ICampagneService CampagneService { get; set; } = default!;

        [Inject]
        private ILogger<SuiviCampagnes> Logger { get; set; } = default!;

        private List<CampagneEnCours> CampagnesEnCours { get; set; } = [];
        private List<LigneCampagneEnCours> LignesCampagne { get; set; } = [];

        private CampagneEnCours? CampagneSelectionnee { get; set; }
        private LigneCampagneEnCours? LigneSelectionnee { get; set; }
        private long? IdLCampagnePopupOuverte { get; set; }
        private ModeOuverturePopup ModePopup { get; set; }

        private PeriodicTimer? timerRafraichissement;
        private string? MessageProchainContact { get; set; }

        protected override async Task OnParametersSetAsync() => await LoadAsync();

        protected override void OnInitialized()
        {
            timerRafraichissement = new PeriodicTimer(IntervalleRafraichissement);
            _ = RafraichirPeriodiquementAsync();
        }

        // Rafraîchit les données en tâche de fond sans jamais superposer deux rafraîchissements
        // (le tick suivant n'est attendu qu'une fois le précédent terminé) et sans perturber la
        // sélection en cours — LoadLignesCampagneSelectionnee conserve la ligne sélectionnée par
        // Id plutôt que de revenir systématiquement à la première.
        private async Task RafraichirPeriodiquementAsync()
        {
            while (await timerRafraichissement!.WaitForNextTickAsync())
            {
                await InvokeAsync(async () =>
                {
                    await LoadAsync();
                    StateHasChanged();
                });
            }
        }

        public void Dispose() => timerRafraichissement?.Dispose();

        private async Task LoadAsync()
        {
            var idOperateur = await GetIdOperateurConnecteAsync();
            if (idOperateur == null) return;

            CampagnesEnCours = await CampagneService.GetCampagnesEnCoursAsync(idOperateur.Value, CancellationToken.None) ?? [];

            await LoadLignesCampagneSelectionnee();
        }

        private async Task LoadLignesCampagneSelectionnee()
        {
            if (CampagnesEnCours?.Count == 0) return;

            CampagneSelectionnee = CampagneSelectionnee ?? CampagnesEnCours!.First();

            var idLigneSelectionnee = LigneSelectionnee?.IdLCampagne;

            LignesCampagne = await CampagneService.GetLigneCampagneEnCoursAsync(CampagneSelectionnee.IdCampagne, CancellationToken.None) ?? [];

            // Conserve la ligne déjà sélectionnée si elle existe toujours (rafraîchissement périodique
            // ou après enregistrement d'une action) ; sinon revient à la première (nouvelle campagne).
            LigneSelectionnee = LignesCampagne?.FirstOrDefault(l => l.IdLCampagne == idLigneSelectionnee)
                ?? LignesCampagne?.FirstOrDefault();
        }

        private async Task SelectCampagneAsync(CampagneEnCours campagne)
        {
            if (campagne == CampagneSelectionnee) return;

            Logger.LogInformation("Opérateur {IdOperateur} sélectionne la campagne {IdCampagne}", await GetIdOperateurConnecteAsync(), campagne.IdCampagne);

            CampagneSelectionnee = campagne;
            await LoadLignesCampagneSelectionnee();
        }

        private async Task SelectLigne(LigneCampagneEnCours? ligne)
        {
            Logger.LogInformation("Opérateur {IdOperateur} sélectionne la ligne {IdLCampagne}", await GetIdOperateurConnecteAsync(), ligne?.IdLCampagne);

            LigneSelectionnee = ligne;
        }

        // Rien dans la spec n'interdit formellement une nouvelle action sur une ligne déjà "Ne plus
        // contacter" (§3.4 dit même l'inverse : une nouvelle action est toujours possible) - mais le
        // déroulement insère le client dans CLIENTS_HORS_CONTACT (§2.3), donc le recontacter via
        // Relance n'a pas de sens métier. Bouton désactivé + ligne signalée en attendant un arbitrage.
        private static bool LigneNePlusContacter(LigneCampagneEnCours ligne) => ligne.DeroulementCode == CodesDeroulement.NePlusContacter;

        private string? ClasseLigne(LigneCampagneEnCours ligne)
        {
            var classes = new List<string>();
            if (ligne == LigneSelectionnee) classes.Add("selectionnee");
            if (LigneNePlusContacter(ligne)) classes.Add("ne-plus-contacter");
            return classes.Count == 0 ? null : string.Join(' ', classes);
        }

        private async Task OnProchainContactAsync()
        {
            if (CampagneSelectionnee is null) return;

            Logger.LogInformation("Opérateur {IdOperateur} clique sur Prochain contact pour la campagne {IdCampagne}", await GetIdOperateurConnecteAsync(), CampagneSelectionnee.IdCampagne);

            var ligne = await CampagneService.AcquireProchainContactAsync(CampagneSelectionnee.IdCampagne, CancellationToken.None);
            if (ligne is null)
            {
                MessageProchainContact = Localizer["SuiviCampagnes_ProchainContactAucun"];
                return;
            }

            // Affectation directe (pas de passage par SelectLigne/OpenPopupLigneSelectionneeAsync,
            // qui liraient encore l'ancienne LigneSelectionnee avant sa mise à jour) : évite un état
            // intermédiaire où la popup s'ouvrirait un instant sur l'ancienne ligne sélectionnée.
            MessageProchainContact = null;
            ModePopup = ModeOuverturePopup.ProchainContact;
            LigneSelectionnee = LignesCampagne.FirstOrDefault(l => l.IdLCampagne == ligne);
            IdLCampagnePopupOuverte = ligne;
        }

        private async Task OnRelanceAsync()
        {
            Logger.LogInformation("Opérateur {IdOperateur} clique sur Relance pour la ligne {IdLCampagne}", await GetIdOperateurConnecteAsync(), LigneSelectionnee?.IdLCampagne);
            ModePopup = ModeOuverturePopup.Relance;
            await OpenPopupLigneSelectionneeAsync();
        }

        private async Task OnHistoriqueAsync()
        {
            Logger.LogInformation("Opérateur {IdOperateur} clique sur Historique pour la ligne {IdLCampagne}", await GetIdOperateurConnecteAsync(), LigneSelectionnee?.IdLCampagne);
            ModePopup = ModeOuverturePopup.Historique;
            await OpenPopupLigneSelectionneeAsync();
        }

        private async Task OnPopupClosedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await LoadAsync();
        }

        private async Task OnPopupSavedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await LoadAsync();
        }

        private async Task OpenPopupLigneSelectionneeAsync()
        {
            if (LigneSelectionnee is null) return;

            IdLCampagnePopupOuverte = LigneSelectionnee.IdLCampagne;
        }

        // Configuration déclarative des colonnes (libellé + accès à la valeur) plutôt que figée dans le markup,
        // comme demandé par la spec UX. Les colonnes issues de la popup de saisie d'action (hors scope actuel)
        // n'ont pas encore de source de données et restent vides.
        private sealed record ColonneLigne(string Libelle, Func<LigneCampagneEnCours, string?> Valeur);

        // Propriété calculée (pas de champ static) car les colonnes Type de contact/Déroulement/
        // Intérêt/Canal doivent résoudre leur libellé via Localizer, qui est une dépendance
        // d'instance — voir Application/services.md pour l'origine du pattern Code + .resx.
#pragma warning disable CS8604 // IStringLocalizer n'annote pas son indexeur comme non-nullable, mais il ne retourne jamais null.
        private ColonneLigne[] ColonnesLigne =>
        [
            new(Localizer["SuiviCampagnes_ColonneSoc"], l => l.CodeSoc),
            new(Localizer["SuiviCampagnes_ColonneNumcli"], l => l.NumCli?.ToString("0")),
            new(Localizer["SuiviCampagnes_ColonneRfm"], l => l.Rfm),
            new(Localizer["SuiviCampagnes_ColonneRaisonSociale"], l => l.RaisonSociale),
            new(Localizer["SuiviCampagnes_ColonneSousActivite"], l => l.SousActivite),
            new(Localizer["SuiviCampagnes_ColonneCaHt"], l => l.CaHt?.ToString("N2")),
            new(Localizer["SuiviCampagnes_ColonneDateDernierAchat"], l => l.DateDernierAchat?.ToString("dd/MM/yyyy")),
            new(Localizer["SuiviCampagnes_ColonneCorrespondant"], l => l.Correspondant),
            new(Localizer["SuiviCampagnes_ColonneTelephone"], l => l.Telephone),
            new(Localizer["SuiviCampagnes_ColonneEmail"], l => l.Email),
            new(Localizer["SuiviCampagnes_ColonneMagasinAffilie"], l => l.MagasinAffilie),
            new(Localizer["SuiviCampagnes_ColonneDateHeureContact"], l => l.DateHeureContact?.ToString("dd/MM/yyyy")),
            new(Localizer["SuiviCampagnes_ColonneTypeContact"], l => l.TypeContactCode is null ? null : Localizer[$"TypeContact_{l.TypeContactCode}"]),
            new(Localizer["SuiviCampagnes_ColonneDeroulement"], l => l.DeroulementCode is null ? null : Localizer[$"Deroulement_{l.DeroulementCode}"]),
            new(Localizer["SuiviCampagnes_ColonneDateRelance"], l => l.DateRelance?.ToString("dd/MM/yyyy")),
            new(Localizer["SuiviCampagnes_ColonneInteretClient"], l => l.InteretClientCode is null ? null : Localizer[$"Interet_{l.InteretClientCode}"]),
            new(Localizer["SuiviCampagnes_ColonneCanalAchat"], l => l.CanalAchatCode is null ? null : Localizer[$"CanalAchat_{l.CanalAchatCode}"]),
            new(Localizer["SuiviCampagnes_ColonneCommentaire"], l => l.Commentaire),
            new(Localizer["SuiviCampagnes_ColonneOperateurEnCours"], l => l.NomOperateurEnCours),
        ];
#pragma warning restore CS8604
    }
}
