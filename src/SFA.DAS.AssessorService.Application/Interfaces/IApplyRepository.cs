using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApplyRepository
    {
        Task<List<Domain.Entities.Application>> GetUserApplications(Guid userId);
        Task<List<Domain.Entities.Application>> GetOrganisationApplications(Guid userId);
        Task<Domain.Entities.Application> GetApplication(Guid applicationId);
        Task<Guid> CreateApplication(CreateApplicationRequest applicationRequest);
        Task SubmitApplicationSequence(Domain.Entities.Application application);
        Task<int> GetNextAppReferenceSequence();
    }
}
