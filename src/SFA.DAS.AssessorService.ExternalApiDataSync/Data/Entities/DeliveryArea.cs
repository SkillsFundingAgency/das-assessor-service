using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class DeliveryArea
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public int Ordering { get; set; }
    }
}
