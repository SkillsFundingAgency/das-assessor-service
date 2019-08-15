using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Auditing;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAuditLogService
    {
        List<AuditLogDiff> Compare<T>(T previous, T current) where T : IAuditFilter;
    }
}
