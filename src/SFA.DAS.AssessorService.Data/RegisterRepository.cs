using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository

    {
        public async Task<EpaOrganisation> CreateEpaOrganisation(EpaOrganisation organisation)
        {
            return null;
        } 
    }
}
