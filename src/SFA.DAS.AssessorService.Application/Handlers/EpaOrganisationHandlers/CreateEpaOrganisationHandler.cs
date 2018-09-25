using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationHandler : IRequestHandler<CreateEpaOrganisationRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;

        public CreateEpaOrganisationHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, IEpaOrganisationIdGenerator orgIdGenerator, ILogger<CreateEpaOrganisationHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _organisationIdGenerator = orgIdGenerator;
            _validator = validator;
        }

        public async Task<string> Handle(CreateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();
            
            errorDetails.Append(_validator.CheckOrganisationName(request.Name));
            errorDetails.Append(_validator.CheckOrganisationTypeIsNullOrExists(request.OrganisationTypeId));
            errorDetails.Append(_validator.CheckUkprnIsValid(request.Ukprn));
            errorDetails.Append(_validator.CheckOrganisationNameNotUsed(request.Name));
            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new BadRequestException(errorDetails.ToString());
            }
            
            errorDetails.Append(_validator.CheckIfOrganisationUkprnExists(request.Ukprn));
            ThrowAlreadyExistsExceptionIfErrorPresent(errorDetails);

            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
                throw new Exception("A valid organisation Id could not be generated");

            var organisation = MapOrganisationRequestToOrganisation(request, newOrganisationId);
            return await _registerRepository.CreateEpaOrganisation(organisation);
        }

        private void ThrowAlreadyExistsExceptionIfErrorPresent(StringBuilder errorDetails)
        {
            if (errorDetails.Length == 0) return;
            _logger.LogError(errorDetails.ToString());
            throw new AlreadyExistsException(errorDetails.ToString());
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(CreateEpaOrganisationRequest request, string newOrganisationId)
        {
            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = newOrganisationId,
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData
                {
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    LegalName = request.LegalName,
                    Postcode = request.Postcode,
                    WebsiteLink = request.WebsiteLink
                }
            };

            return organisation;
        }
    }
}
