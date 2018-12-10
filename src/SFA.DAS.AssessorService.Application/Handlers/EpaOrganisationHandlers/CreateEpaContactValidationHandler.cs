using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaContactValidationHandler : IRequestHandler<CreateEpaContactValidationRequest, ValidationResponse>
    {
        private readonly ILogger<CreateEpaContactValidationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaContactValidationHandler(IEpaOrganisationValidator validator,
            ILogger<CreateEpaContactValidationHandler> logger)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(CreateEpaContactValidationRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.ValidatorCreateEpaOrganisationContactRequest(new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = request.OrganisationId,
                DisplayName = request.Name,
                Email = request.Email,
                PhoneNumber = request.Phone
            } );

            return await Task.FromResult(result);
        }
    }
}