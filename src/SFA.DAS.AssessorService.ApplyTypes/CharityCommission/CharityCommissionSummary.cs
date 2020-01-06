using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes.CharityCommission
{
    public class CharityCommissionSummary
    {
        public string CharityName { get; set; }
        public string CharityNumber { get; set; }
        public DateTime? IncorporatedOn { get; set; }

        public List<TrusteeInformation> Trustees { get; set; }
        public bool TrusteeManualEntryRequired { get; set; }
    }

    public class TrusteeInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
