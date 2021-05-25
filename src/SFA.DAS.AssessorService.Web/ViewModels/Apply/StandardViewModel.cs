using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class StandardViewModel
    {
        public Guid Id { get; set; }

        public string StandardToFind { get; set; }

        public int StandardCode { get; set; }

        public List<StandardCollation> Results { get; set; }

        public StandardCollation SelectedStandard { get; set; }
    
        public bool IsConfirmed { get; set; }

        public string ApplicationStatus { get; set; }

        public Dictionary<string, bool> OrganisationHasStandardWithVersions { get; set; }
    }


}
