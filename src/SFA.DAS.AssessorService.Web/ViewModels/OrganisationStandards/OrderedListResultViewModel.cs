using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels.OrganisationStandards
{
    public class OrderedListResultViewModel
    {
        public PaginatedList<EpaoPipelineStandardsResponse> Response { get; set; }

        [Display(Name = "Standard")]
        public string SelectedStandard { get; set; }
        public List<PipelineFilterItem> StandardFilter { get; set; }

        [Display(Name = "Training provider")]
        public string SelectedProvider { get; set; }
        public List<PipelineFilterItem> ProviderFilter { get; set; }

        [Display(Name = "Estimated EPA date")]
        public string SelectedEPADate { get; set; }
        public List<PipelineFilterItem> EPADateFilter { get; set; }

        public bool FilterApplied { get; set; }

        public string OrderedBy { get; set; }
        public string OrderDirection { get; set; }

        public class PipelineFilterItem
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
    }
}
