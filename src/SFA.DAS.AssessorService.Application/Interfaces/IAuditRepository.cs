using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.AssessorService.Application.Auditing;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAuditRepository
    {
        Task CreateAudit(Guid organisationId, string updatedBy, List<AuditChange> auditChanges);
    }
}
