using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Data.Entitites
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
