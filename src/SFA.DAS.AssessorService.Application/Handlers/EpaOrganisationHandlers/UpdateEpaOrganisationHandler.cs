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
    public class UpdateEpaOrganisationHandler : IRequestHandler<UpdateEpaOrganisationRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;
        
        public UpdateEpaOrganisationHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<UpdateEpaOrganisationHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _cleanser = cleanser;
            _validator = validator;
        }

        public async Task<string> Handle(UpdateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            ProcessRequestFieldsForSpecialCharacters(request);
            var validationResponse = _validator.ValidatorUpdateEpaOrganisationRequest(request);
         
            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.NotFound.ToString()))
                {
                    throw new NotFound(message);
                }

                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }

                throw new Exception(message);
            }

            var organisation = MapOrganisationRequestToOrganisation(request);

           return await _registerRepository.UpdateEpaOrganisation(organisation);
        }

        private void ProcessRequestFieldsForSpecialCharacters(UpdateEpaOrganisationRequest request)
        {       
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId?.Trim());  
            request.Name = _cleanser.CleanseStringForSpecialCharacters(request.Name?.Trim());  
            request.LegalName = _cleanser.CleanseStringForSpecialCharacters(request.LegalName?.Trim());
            request.WebsiteLink = _cleanser.CleanseStringForSpecialCharacters(request.WebsiteLink?.Trim());
            request.Address1 = _cleanser.CleanseStringForSpecialCharacters(request.Address1?.Trim());
            request.Address2 = _cleanser.CleanseStringForSpecialCharacters(request.Address2?.Trim());
            request.Address3 = _cleanser.CleanseStringForSpecialCharacters(request.Address3?.Trim());
            request.Address4 = _cleanser.CleanseStringForSpecialCharacters(request.Address4?.Trim());
            request.Postcode = _cleanser.CleanseStringForSpecialCharacters(request.Postcode?.Trim());
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(UpdateEpaOrganisationRequest request)
        {
            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = request.OrganisationId.Trim(),
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
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
