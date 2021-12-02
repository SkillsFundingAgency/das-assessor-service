using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoPipelineStandardsFiltersResponse
    {
        public IEnumerable<EpaoPipelineStandardFilter> StandardFilterItems { get; set; }
        public IEnumerable<EpaoPipelineStandardFilter> ProviderFilterItems { get; set; }
        public IEnumerable<EpaoPipelineStandardFilter> EPADateFilterItems { get; set; }
    }
}
