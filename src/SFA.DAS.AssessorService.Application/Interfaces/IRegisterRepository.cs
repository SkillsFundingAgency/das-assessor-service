using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterRepository
    {
        Task<EpaOrganisation> CreateEpaOrganisation(EpaOrganisation organisation);
        Task<EpaOrganisation> GetEpaOrganisationById(Guid epaOrganisationId);
    }
}
