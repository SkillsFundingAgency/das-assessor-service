using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Auditing
{
    public class AuditChange
    {
        public string ProperyChanged { get; set; }
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }
        public string FieldChanged { get; set; }
    }
}
