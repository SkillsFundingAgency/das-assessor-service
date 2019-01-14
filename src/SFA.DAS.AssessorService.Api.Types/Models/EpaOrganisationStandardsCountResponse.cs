using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationStandardsCountResponse
    {

        public EpaOrganisationStandardsCountResponse(int count)
        {
            Count = count;
        }

        public int Count { get; set; }
    }
}
