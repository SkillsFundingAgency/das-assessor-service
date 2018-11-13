using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class GetLearnerDetailsRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public int? StandardCode { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}
