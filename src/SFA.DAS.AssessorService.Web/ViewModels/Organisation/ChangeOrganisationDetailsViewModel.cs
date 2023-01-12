using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeOrganisationDetailsViewModel
    {
        public List<ContactResponse> Contacts { get; internal set; }
        public string ActionChoice { get; set; }
    }
}
