using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationStandardValidationHandler : IRequestHandler<CreateEpaOrganisationStandardValidationRequest, ValidationResponse>
    {
        private readonly ILogger<CreateEpaOrganisationStandardValidationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaOrganisationStandardValidationHandler(ILogger<CreateEpaOrganisationStandardValidationHandler> logger, IEpaOrganisationValidator validator)
        {
            _logger = logger;
            _validator = validator;
        }


        public async Task<ValidationResponse> Handle(CreateEpaOrganisationStandardValidationRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.ValidatorCreateEpaOrganisationStandardRequest(new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveTo = request.EffectiveTo,
                EffectiveFrom = request.EffectiveFrom,
                ContactId = request.ContactId,
                DeliveryAreas = request.DeliveryAreas
            });

            return await Task.FromResult(result);
        }
    }
}
