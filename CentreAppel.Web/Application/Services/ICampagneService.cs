using CentreAppel.Web.Application.Models;

namespace CentreAppel.Web.Application.Services;

public interface ICampagneService
{
    Task<List<CampagneEnCours>?> GetCampagnesEnCoursAsync(long idOperateur, CancellationToken cancellationToken);
    Task<List<LigneCampagneEnCours>?> GetLigneCampagneEnCoursAsync(long idCampagne, CancellationToken cancellationToken);
    Task<List<CommentaireCampagne>> GetCommentairesCampagneAsync(long idCampagne, CancellationToken cancellationToken);
    Task<LigneCampagnePopup?> GetLigneCampagnePopupAsync(long idLCampagne, CancellationToken cancellationToken);
    Task<long?> AcquireProchainContactAsync(long idCampagne, CancellationToken cancellationToken);
    Task<List<HistoriqueAction>> GetHistoriqueAsync(long idLCampagne, CancellationToken cancellationToken);

    Task SaveActionAsync(SaisieAction saisie, string? commentaireLibre, CancellationToken cancellationToken);
}
