using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class SelectOrChangeContactNameViewModel
    {
        public List<ContactResponse> Contacts { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactName { get; set; }
        public string ActionChoice { get; set; }
    }
}
