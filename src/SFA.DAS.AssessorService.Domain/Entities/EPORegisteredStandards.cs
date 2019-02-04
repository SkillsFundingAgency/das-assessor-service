using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EPORegisteredStandards
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public int Level { get; set; }
    }
}
