using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class ApplyStandardConfirmViewModel
    {
        public Guid Id { get; set; }

        public string Search { get; set; }

        public string StandardReference { get; set; }

        public List<StandardVersion> Results { get; set; }
        public List<StandardVersion> DistinctResults => GetDistinctResults();

        public StandardVersion SelectedStandard { get; set; }

        public List<string> SelectedVersions { get; set; }

        public bool IsConfirmed { get; set; }

        public string ApplicationStatus { get; set; }

        public DateTime? EarliestVersionEffectiveFrom { get; set; }

        private List<StandardVersion> GetDistinctResults()
        {
            var distinctResults = Results?.DistinctBy(r => r.StandardUId).ToList();
            if (distinctResults != null)
            {
                return distinctResults;
            }
            return Results;
        }
    }
}
