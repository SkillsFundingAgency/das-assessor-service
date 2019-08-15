using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public interface IAuditFilter
    {
        string FilterAuditDiff(string propertyChanged);
    }
}
