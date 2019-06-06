using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class StandardCollation
    {
        public int Id { get; set; }
        public int StandardId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public string StandardData { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateRemoved { get; set; }

        public bool? IsLive { get; set; }
    }
}
