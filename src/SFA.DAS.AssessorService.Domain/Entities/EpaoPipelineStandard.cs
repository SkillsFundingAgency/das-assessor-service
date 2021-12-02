using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EpaoPipelineStandard
    {
        public string Title { get; set; }
        public int Pipeline { get; set; }
        public DateTime EstimateDate { get; set; }
        public int TotalRows { get; set; }

        public string StdCode { get; set; }
        public string Version { get; set; }

        public string UKPRN { get; set; }
        public string ProviderName { get; set; }
    }

    public class EpaoPipelineStandardExtract : EpaoPipelineStandard
    {
        public int ProviderUkPrn { get; set; }
    }
}
