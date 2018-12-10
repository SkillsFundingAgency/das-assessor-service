using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationContactValidationHandler: IRequestHandler<UpdateEpaOrganisationContactValidationRequest, ValidationResponse>
    {
        private readonly ILogger<UpdateEpaOrganisationContactValidationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        public UpdateEpaOrganisationContactValidationHandler(IEpaOrganisationValidator validator, ILogger<UpdateEpaOrganisationContactValidationHandler> logger)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(UpdateEpaOrganisationContactValidationRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.ValidatorUpdateEpaOrganisationContactRequest(new UpdateEpaOrganisationContactRequest
            {
                ContactId = request.ContactId,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            } );

            return await Task.FromResult(result);
        }
    }
}