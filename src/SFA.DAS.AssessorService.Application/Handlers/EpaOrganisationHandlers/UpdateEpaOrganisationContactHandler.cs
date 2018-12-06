using System;
using System.Collections.Generic;
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
    public class UpdateEpaOrganisationContactHandler : IRequestHandler<UpdateEpaOrganisationContactRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateEpaOrganisationContactHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public UpdateEpaOrganisationContactHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ISpecialCharacterCleanserService cleanser, ILogger<UpdateEpaOrganisationContactHandler> logger)
        {
            _registerRepository = registerRepository;
            _validator = validator;
            _cleanser = cleanser;
            _logger = logger;
        }
        public async Task<string> Handle(UpdateEpaOrganisationContactRequest request, CancellationToken cancellationToken)
        {
            ProcessRequestFieldsForSpecialCharacters(request);
            var validationResponse = _validator.ValidatorUpdateEpaOrganisationContactRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.AlreadyExists.ToString()))
                {
                    throw new AlreadyExistsException(message);
                }
                throw new Exception(message);
            }
            var contact = MapOrganisationContactRequestToContact(request);
            return await _registerRepository.UpdateEpaOrganisationContact(contact, request.ActionChoice);
        }

        private static EpaContact MapOrganisationContactRequestToContact(UpdateEpaOrganisationContactRequest request)
        {
            return new EpaContact
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                Id = Guid.Parse(request.ContactId),
                PhoneNumber = request.PhoneNumber
            };
        }

        private void ProcessRequestFieldsForSpecialCharacters(UpdateEpaOrganisationContactRequest request)
        {
            request.DisplayName = _cleanser.CleanseStringForSpecialCharacters(request.DisplayName);
            request.Email = _cleanser.CleanseStringForSpecialCharacters(request.Email);
            request.PhoneNumber = _cleanser.CleanseStringForSpecialCharacters(request.PhoneNumber);
        }
    }
}
