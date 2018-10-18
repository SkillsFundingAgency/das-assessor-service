using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationStandardHandler : IRequestHandler<UpdateEpaOrganisationStandardRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public UpdateEpaOrganisationStandardHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<UpdateEpaOrganisationStandardHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerRepository = registerRepository;
            _validator = validator;
            _logger = logger;
            _cleanser = cleanser;
        }


        public async Task<string> Handle(UpdateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
         
            ProcessRequestFieldsForSpecialCharacters(request);
            var validationResponse = _validator.ValidatorUpdateEpaOrganisationStandardRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {     
                    throw new BadRequestException(message);
                }
                
                throw new Exception(message);
            } 
            
            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);

            return await _registerRepository.UpdateEpaOrganisationStandard(organisationStandard, request.DeliveryAreas);
        }

        private void ProcessRequestFieldsForSpecialCharacters(UpdateEpaOrganisationStandardRequest request)
        {
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId?.Trim());
            request.Comments = _cleanser.CleanseStringForSpecialCharacters(request.Comments?.Trim());
            request.ContactId = request.ContactId?.Trim();
        }

        private static EpaOrganisationStandard MapOrganisationStandardRequestToOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            Guid? contactId = null;
            if (!string.IsNullOrEmpty(request.ContactId) && Guid.TryParse(request.ContactId, out Guid contactIdGuid))
                contactId = contactIdGuid;

            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                Comments = request.Comments,
                ContactId = contactId
            };
            return organisationStandard;
        }
    }
}
