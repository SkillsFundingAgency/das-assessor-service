using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationStandardHandler : IRequestHandler<CreateEpaOrganisationStandardRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public CreateEpaOrganisationStandardHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<CreateEpaOrganisationStandardHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerRepository = registerRepository;
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
                    throw new NotFound(message);
                }

                if (validationResponse.Errors.Any(x => x.ValidationStatusCode == ValidationStatusCode.AlreadyExists))
                {
                    throw new AlreadyExistsException(message);
                }

                throw new Exception(message);
            } 

            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);
           
            return await _registerRepository.CreateEpaOrganisationStandard(organisationStandard, request.DeliveryAreas);
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
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateStandardApprovedOnRegister = null,
                Comments = request.Comments,
                ContactId = contactId,
                OrganisationStandardData = new OrganisationStandardData { DeliveryAreasComments = request.DeliveryAreasComments}
            };
            return organisationStandard;
        }
    }
}
