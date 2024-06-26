﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationValidationHandler : IRequestHandler<CreateEpaOrganisationValidationRequest, ValidationResponse>
    {
        private readonly ILogger<CreateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaOrganisationValidationHandler(IEpaOrganisationValidator validator,
            ILogger<CreateEpaOrganisationHandler> logger)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(CreateEpaOrganisationValidationRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.ValidatorCreateEpaOrganisationRequest(new CreateEpaOrganisationRequest
            {
                Name = request.Name,
                Ukprn = request.Ukprn,
                OrganisationTypeId = request.OrganisationTypeId,
                CompanyNumber = request.CompanyNumber,
                CharityNumber = request.CharityNumber,
                RecognitionNumber = request.RecognitionNumber
            } );

            return await Task.FromResult(result);
        }
    }
}
