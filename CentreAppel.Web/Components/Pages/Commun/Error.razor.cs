using System.Diagnostics;

namespace CentreAppel.Web.Components.Pages.Commun
{
    public partial class Error : LocalizedPage
    {
        private string? RequestId { get; set; }
        private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        protected override void OnInitialized() =>
            RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
    }

}
