using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationStandardHandler : IRequestHandler<CreateEpaOrganisationStandardRequest, string>
    {
        private readonly IMediator _mediator;
        private readonly IRegisterRepository _registerRepository;
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        private readonly ILogger<CreateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public CreateEpaOrganisationStandardHandler(
            IMediator mediator,
            IRegisterRepository registerRepository, 
            IOrganisationStandardRepository organisationStandardRepository,
            IEpaOrganisationValidator validator, 
            ILogger<CreateEpaOrganisationStandardHandler> logger, 
            ISpecialCharacterCleanserService cleanser)
        {
            _mediator = mediator;
            _registerRepository = registerRepository;
            _organisationStandardRepository = organisationStandardRepository;
            _logger = logger;
            _cleanser = cleanser;
            _validator = validator;
        }

        public async Task<string> Handle(CreateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            ProcessRequestFieldsForSpecialCharacters(request);
            var validationResponse = _validator.ValidatorCreateEpaOrganisationStandardRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x =>  x.ValidationStatusCode == ValidationStatusCode.BadRequest))
                {     
                    throw new BadRequestException(message);
                }
                
                if (validationResponse.Errors.Any(x =>  x.ValidationStatusCode == ValidationStatusCode.NotFound))
                {
                    throw new NotFoundException(message);
                }

                if (validationResponse.Errors.Any(x => x.ValidationStatusCode == ValidationStatusCode.AlreadyExists))
                {
                    throw new AlreadyExistsException(message);
                }

                throw new Exception(message);
            }

            var standardExists = (await _organisationStandardRepository.GetOrganisationStandardByOrganisationIdAndStandardReference(
                request.OrganisationId, request.StandardReference) != null);

            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);
            var deliveryAreas = !(request.DeliveryAreas?.Any() ?? false) ? await GetDeliveryAreas() : request.DeliveryAreas;

            if (standardExists) 
            {
                return await _registerRepository.UpdateEpaOrganisationStandardAndOrganisationStandardVersions(organisationStandard, deliveryAreas);
            }
            else
            {
                return await _registerRepository.CreateEpaOrganisationStandard(organisationStandard, deliveryAreas);
            }
        }

        private void ProcessRequestFieldsForSpecialCharacters(CreateEpaOrganisationStandardRequest request)
        {
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId?.Trim());
            request.Comments = _cleanser.CleanseStringForSpecialCharacters(request.Comments?.Trim());
            request.ContactId = request.ContactId?.Trim();
        }
        
        private static  EpaOrganisationStandard MapOrganisationStandardRequestToOrganisationStandard(CreateEpaOrganisationStandardRequest request)
        {
            Guid? contactId = null;
            if (Guid.TryParse(request.ContactId, out var contactIdGuid))
                contactId = contactIdGuid;

            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                StandardReference = request.StandardReference,
                StandardVersions = request.StandardVersions,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateStandardApprovedOnRegister = request.DateStandardApprovedOnRegister,
                Comments = request.Comments,
                ContactId = contactId,
                OrganisationStandardData = new OrganisationStandardData { DeliveryAreasComments = request.DeliveryAreasComments}
            };
            return organisationStandard;
        }

        private async Task<List<int>> GetDeliveryAreas()
        {
            var areas = await _mediator.Send(new GetDeliveryAreasRequest());
            return areas.Select(a => a.Id).ToList();
        }
    }
}
