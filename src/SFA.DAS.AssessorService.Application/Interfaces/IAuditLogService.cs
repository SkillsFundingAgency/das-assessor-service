using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Auditing;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<List<AuditChange>> GetEpaOrganisationChanges(string organisationId, EpaOrganisation organisation);
        Task<List<AuditChange>> GetEpaOrganisationPrimaryContactChanges(string organisationId, EpaContact primaryContactId);

        Task WriteChangesToAuditLog(string organisationId, string updatedBy, List<AuditChange> auditChanges);
    }
}
