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
        public DateTime? EffectiveFrom { get ; set ; }
        public DateTime? EffectiveTo { get ; set ; }
        public DateTime? LastDateForNewStarts { get ; set ; }
        public int Level { get ; set ; }
        public int Duration { get ; set ; }
        public int CurrentFundingCap { get ; set ; }
        public bool IsPublished { get ; set ; }
        public bool IsActiveStandard { get ; set ; }
        public string Uri { get ; set ; }
    }
}