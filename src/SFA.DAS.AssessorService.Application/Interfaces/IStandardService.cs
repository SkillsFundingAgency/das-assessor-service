using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<StandardCollation>> GetAllStandards();

        Task<StandardCollation> GetStandard(int standardId);
        Task<StandardCollation> GetStandard(string referenceNumber);

        Task<IEnumerable<IfaStandard>> GetIfaStandards();
        Task<IEnumerable<StandardCollation>> GatherAllApprovedStandardDetails(List<IfaStandard> approvedIfaStandards);
        IEnumerable<StandardNonApprovedCollation> GatherAllNonApprovedStandardDetails(List<IfaStandard> nonApprovedIfaStandards);

        Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId);
        Task<IEnumerable<StandardOptions>> GetStandardOptions();
    }
}