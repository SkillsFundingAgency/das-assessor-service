using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterRepository
    {
        Task<string> CreateEpaOrganisation(EpaOrganisation organisation);
        Task<string> UpdateEpaOrganisation(EpaOrganisation organisation);

        
    }
}
