using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class ApplyStandardSearchViewModel
    {
        public Guid Id { get; set; }
        public string Search { get; set; }

        public List<StandardVersion> Results { get; set; }
    }
}
