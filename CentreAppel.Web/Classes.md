### Ce fichier sert de guide pour la construction des classes

Les classes dans le dossier DTO ne doivent pas etre suffixées Dto exemple:CampagneEnCours et pas CampagneEnCoursDto
Les classes dans le dossier DTO doivent refleter ce qui va etre affiché et/ou nécesaire au traitement de l'action utilisateur

Exemple:
public class CampagneEnCours 
{
    public long IdCampagne { get; set; }
    public required string Nom { get; set; }
    public DateOnly DateCampagne { get; set; }
    public required string Description { get; set; }
    public required string Statut { get; set; }
}

### Id
Les Id doivent etre explicite sans abréviation
Exemple:

public class CampagneOperateur
{
    public long IdCampagneOperateur { get; set; }
}

et pas:
public class CampagneOperateur
{
    public long IdCampOp { get; set; }
}