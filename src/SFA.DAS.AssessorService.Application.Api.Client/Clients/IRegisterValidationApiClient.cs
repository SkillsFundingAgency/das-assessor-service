using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IRegisterValidationApiClient
    {
        Task<ValidationResponse> CreateEpaContactValidate(CreateEpaContactValidationRequest request);
        Task<ValidationResponse> CreateOrganisationStandardValidate(CreateEpaOrganisationStandardValidationRequest request);
        Task<ValidationResponse> CreateOrganisationValidate(CreateEpaOrganisationValidationRequest request);
        Task<ValidationResponse> UpdateOrganisationValidate(UpdateEpaOrganisationValidationRequest request);
    }
}