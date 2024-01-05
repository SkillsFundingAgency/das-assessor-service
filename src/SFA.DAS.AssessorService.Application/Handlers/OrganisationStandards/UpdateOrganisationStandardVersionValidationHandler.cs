using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards
{
    public class UpdateOrganisationStandardVersionValidationHandler : IRequestHandler<UpdateEpaOrganisationStandardVersionValidationRequest, ValidationResponse>
    {
        private readonly IEpaOrganisationValidator _validator;

        public UpdateOrganisationStandardVersionValidationHandler(IEpaOrganisationValidator validator)
        {
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(UpdateEpaOrganisationStandardVersionValidationRequest request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidatorUpdateOrganisationStandardVersionRequest(new UpdateOrganisationStandardVersionRequest
            {
                OrganisationStandardId = request.OrganisationStandardId,
                OrganisationStandardVersion = request.OrganisationStandardVersion,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo
            });

            return result;
        }
    }
}
