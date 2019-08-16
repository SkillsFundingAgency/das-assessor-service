using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApplyRepository
    {
        Task<List<Domain.Entities.Application>> GetUserApplications(Guid userId);
        Task<List<Domain.Entities.Application>> GetOrganisationApplications(Guid userId);
        Task<Domain.Entities.Application> GetApplication(Guid applicationId);
        Task<Guid> CreateApplication(string applicationData, Guid applyingOrganisationId, Guid applyingApplicationId);
    }
}
