﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationStandardValidationHandler : IRequestHandler<UpdateEpaOrganisationStandardValidationRequest, ValidationResponse>
    {
        private readonly ILogger<UpdateEpaOrganisationStandardValidationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public UpdateEpaOrganisationStandardValidationHandler(ILogger<UpdateEpaOrganisationStandardValidationHandler> logger, IEpaOrganisationValidator validator)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(UpdateEpaOrganisationStandardValidationRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.ValidatorUpdateEpaOrganisationStandardRequest(new UpdateEpaOrganisationStandardRequest
            {
                OrganisationId = request.OrganisationId,
                OrganisationStandardId = request.OrganisationStandardId,
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
