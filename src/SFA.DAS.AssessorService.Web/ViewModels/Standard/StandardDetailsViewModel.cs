using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class StandardDetailsViewModel
    {
        public StandardVersion SelectedStandard { get; set; }
        
        public List<StandardVersion> AllVersions { get; set; }
        
        public List<StandardVersion> ApprovedVersions { get; set; }

        public string GetStatusText(StandardVersion standardVersion)
        {
            if (IsApproved(standardVersion))
            {
                return "Approved";
            }
            else
            {
                return "You can opt in";
            }
        }

        public string GetActionText(StandardVersion standardVersion)
        {
            if (IsApproved(standardVersion))
            {
                return "Opt out of standard version";
            }
            else
            {
                return "Opt into standard version";
            }
        }

        public string GetActionRoute(StandardVersion standardVersion)
        {
            if (IsApproved(standardVersion))
            {
                return StandardController.OptOutStandardVersionRouteGet;
            }
            else
            {
                return StandardController.OptInStandardVersionRouteGet;
            }
        }

        private bool IsApproved(StandardVersion standardVersion)
        {
            return ApprovedVersions.Exists(p => p.Version == standardVersion.Version);
        }
    }
}
