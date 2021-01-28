using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardsListResponse
    {
        public IEnumerable<GetStandardsListItem> Standards { get; set; }
    }

    public class GetStandardsListItem
    {
        public int Id { get ; set ; }
        public string Title { get ; set ; }
        public StandardDates StandardDates { get ; set ; }
        public int Level { get ; set ; }
        public int Duration { get ; set ; }
        public int MaxFunding { get ; set ; }
        public bool IsActiveStandard { get ; set ; }
        
    }

    public class StandardDates
    {
        public DateTime? LastDateStarts { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime EffectiveFrom { get; set; }
    }
}