namespace CentreAppel.Web.Enum;

// Détermine, à l'ouverture de PopupSaisieAction, quels champs/boutons sont affichés (cf. §5.3 de
// la spec technique : composant commun à 5 points d'entrée). Un seul mode par point d'entrée -
// Traiter est partagé par Rappels du jour et Recherche client, qui ont le même comportement de
// saisie (pas encore implémentés). Historique seul est en lecture seule (§10.5 : consultation
// uniquement, aucune saisie possible).
public enum ModeOuverturePopup
{
    ProchainContact,
    Relance,
    Historique,
    Traiter,
}
