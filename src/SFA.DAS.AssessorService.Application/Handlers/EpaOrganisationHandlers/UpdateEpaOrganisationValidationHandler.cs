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
    public class UpdateEpaOrganisationValidationHandler : IRequestHandler<UpdateEpaOrganisationValidationRequest, ValidationResponse>
    {
        private readonly ILogger<UpdateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public UpdateEpaOrganisationValidationHandler(IEpaOrganisationValidator validator,
            ILogger<UpdateEpaOrganisationHandler> logger)
        {
            _logger = logger;
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(UpdateEpaOrganisationValidationRequest request, CancellationToken cancellationToken)
        {
            return _validator.ValidatorUpdateEpaOrganisationRequest(new UpdateEpaOrganisationRequest
            {
                Name = request.Name,
                Ukprn = request.Ukprn,
                OrganisationTypeId = request.OrganisationTypeId,
                OrganisationId = request.OrganisationId,
                ActionChoice = request.ActionChoice,
                Status = request.Status,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                Address4 = request.Address4,
                Postcode = request.Postcode,
                CompanyNumber = request.CompanyNumber,
                CharityNumber = request.CharityNumber
            });
        }
    }
}
