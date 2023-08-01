using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class StandardVersionApplicationViewModel
    {
        public Guid Id { get; set; }

        public string StandardReference { get; set; }

        public List<StandardVersionApplication> Results { get; set; }

        public StandardVersionApplication SelectedStandard { get; set; }
    }
}
