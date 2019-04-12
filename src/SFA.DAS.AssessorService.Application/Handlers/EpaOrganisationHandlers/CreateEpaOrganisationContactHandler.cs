using System;
using System.Linq;
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
    public class CreateEpaOrganisationContactHandler : IRequestHandler<CreateEpaOrganisationContactRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationContactHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;

        public CreateEpaOrganisationContactHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ISpecialCharacterCleanserService cleanser, ILogger<CreateEpaOrganisationContactHandler> logger, IEpaOrganisationIdGenerator organisationIdGenerator)
        {
            _registerRepository = registerRepository;
            _validator = validator;
            _cleanser = cleanser;
            _logger = logger;
            _organisationIdGenerator = organisationIdGenerator;
        }

        public async Task<string> Handle(CreateEpaOrganisationContactRequest request, CancellationToken cancellationToken)
        {

            ProcessRequestFieldsForSpecialCharacters(request);

            var validationResponse = _validator.ValidatorCreateEpaOrganisationContactRequest(request);

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

            var newUsername = request.Email;

            var contact = MapOrganisationContactRequestToContact(request, newUsername);
            return await _registerRepository.CreateEpaOrganisationContact(contact);
        }

        private EpaContact MapOrganisationContactRequestToContact(CreateEpaOrganisationContactRequest request, string newUsername)
        {
            return new EpaContact
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId,
                Id = Guid.NewGuid(),
                PhoneNumber = request.PhoneNumber,
                Username = newUsername,
                FamilyName = request.LastName,
                GivenNames = request.FirstName,
                SigninType = "AsLogin",
                SigninId = null,
                Status="New"
            };
        }

        private void ProcessRequestFieldsForSpecialCharacters(CreateEpaOrganisationContactRequest request)
        {
            request.DisplayName = _cleanser.CleanseStringForSpecialCharacters(request.DisplayName);
            request.FirstName = _cleanser.CleanseStringForSpecialCharacters(request.FirstName);
            request.LastName = _cleanser.CleanseStringForSpecialCharacters(request.LastName);
            request.Email = _cleanser.CleanseStringForSpecialCharacters(request.Email);
            request.PhoneNumber = _cleanser.CleanseStringForSpecialCharacters(request.PhoneNumber);
        }    
    }
}
