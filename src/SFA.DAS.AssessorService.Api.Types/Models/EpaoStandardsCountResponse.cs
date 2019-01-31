using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaoStandardsCountResponse
    {

        public EpaoStandardsCountResponse(int count)
        {
            Count = count;
        }

        public int Count { get; set; }
    }
}
