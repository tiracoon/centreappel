# Spécifications à préciser

Points relevés en cours de développement où la spec (technique ou UX) ne tranche pas clairement un comportement. Une décision provisoire est prise pour pouvoir avancer, documentée ici avec son contexte — à faire arbitrer officiellement plus tard et reporter dans les specs techniques/UX une fois validé (cf. le pattern déjà utilisé en §10 de [Spécifications techniques fusionnées.md](Spécifications%20techniques%20fusionnées.md)).

---

## 2026-09-03 — Blocage du bouton Relance sur une ligne « Ne plus contacter »

**Constat** : rien dans la spec n'interdit explicitement de créer une nouvelle action (bouton Relance) sur une ligne dont la dernière action a pour déroulement « Ne plus contacter ». Le §3.4 (Exclusivité d'écriture) dit même l'inverse : un conseiller peut à tout moment créer une nouvelle action sur la ligne, sans restriction énoncée liée au déroulement précédent. Le mécanisme `CLIENTS_HORS_CONTACT` (§2.3, §3.6) est explicitement un filtre **à l'import** (empêche le client de réapparaître dans une *future* campagne), pas un blocage de la ligne déjà présente dans la campagne en cours.

**Décision provisoire retenue** : le bouton Relance est désactivé quand la ligne sélectionnée a pour dernier déroulement « Ne plus contacter », et la ligne est surlignée en rouge très pâle dans la grille (`rgba(220, 53, 69, 0.08)`). Implémenté dans `SuiviCampagnes.razor` / `.razor.cs` (`LigneNePlusContacter`, `ClasseLigne`).

**Sections concernées** : §2.3, §3.3, §3.4, §3.6, §10.2 (Spécifications techniques fusionnées).

**À trancher officiellement** : faut-il permettre de recontacter un client « Ne plus contacter » via Relance dans certains cas (ex. réservé à l'admin, cohérent avec « réversible uniquement par l'admin » au §3.3), ou bloquer totalement le bouton pour tous les rôles comme fait actuellement ?
