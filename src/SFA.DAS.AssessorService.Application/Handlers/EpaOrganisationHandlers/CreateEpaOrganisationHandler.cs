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
    public class CreateEpaOrganisationHandler : IRequestHandler<CreateEpaOrganisationRequest, EpaOrganisation>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaOrganisationHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<CreateEpaOrganisationHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<EpaOrganisation> Handle(CreateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();
            
            errorDetails.Append(_validator.CheckOrganisationId(request.OrganisationId));
            errorDetails.Append(_validator.CheckOrganisationName(request.Name));
            errorDetails.Append(_validator.CheckOrganisationTypeIsNullOrExists(request.OrganisationTypeId));
            errorDetails.Append(_validator.CheckUkprnIsValid(request.Ukprn));
            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new BadRequestException(errorDetails.ToString());
            }

            errorDetails.Append(_validator.CheckIfOrganisationIdExists(request.OrganisationId));
            errorDetails.Append(_validator.CheckIfOrganisationUkprnExists(request.Ukprn));
            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new AlreadyExistsException(errorDetails.ToString());
            }

            var organisation = MapOrganisationRequestToOrganisation(request);

            return await _registerRepository.CreateEpaOrganisation(organisation);
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(CreateEpaOrganisationRequest request)
        {
            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = request.OrganisationId.Trim(),
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
