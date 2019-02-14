using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateOrganisationAndContactHandler : IRequestHandler<CreateOrganisationContactRequest, CreateOrganisationContactResponse>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<CreateOrganisationAndContactHandler> _logger;
        private readonly IEpaOrganisationValidator _organisationValidator;
        // private readonly IEpaOrganisationValidator _validator;
        // private readonly ISpecialCharacterCleanserService _cleanser;


        public CreateOrganisationAndContactHandler(IRegisterRepository registerRepository, IRegisterQueryRepository registerQueryRepository, ILogger<CreateOrganisationAndContactHandler> logger, IEpaOrganisationValidator organisationValidator)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _organisationValidator = organisationValidator;
            _registerQueryRepository = registerQueryRepository;
        }
      
        
        public async Task<CreateOrganisationContactResponse> Handle(CreateOrganisationContactRequest request, CancellationToken cancellationToken)
        {
            var noOrganisationNameMessage = "organisation name missing - no organisation or contact added";
            var organisationNameTooShortMessage = "organisation name too short - no organisation or contact added";
            var organisationNameAlreadyUsedMessage = "organisation name already used - no organisation or contact added";
            var organisationTypeNotIdentifiedMessage = "organisation type not identified so not added to organisation in register";
            var ukprnNotValidMessage = "Ukprn is an invalid format so not added to this organisation in register";
            var ukprnAlreadyUsedMessage =
                "The ukprn is already used by another organisation so not added to this organisation in register";
            var companyNumberNotValidMessage = "Company number is an invalid format so not added to this organisation in register";
            var companyNumberAlreadyUsedMessage =
                "The company number is already used by another organisation so not added to this organisation in register";
            var charityNumberNotValidMessage = "Charity number is an invalid format so not added to this organisation in register";
            var charityNumberAlreadyUsedMessage =
                "The chairity number is already used by another organisation so not added to this organisation in register";

            var contactAdded = false;
            var organisationAdded = false;
            var warningMessages = new List<string>();

            var organisationName = DecideOrganisationName(request.UseTradingName,request.TradingName, request.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(request.OrganisationUkprn, request.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(request.OrganisationType);
            var companyNumber = request.CompanyNumber;
            var charityNumber = request.CharityNumber;

            //var organisationValidation = _organisationValidator.ValidatorCreateEpaOrganisationRequest(new CreateEpaOrganisationRequest...

            // organisation checks

            var organisationNameEmpty = string.IsNullOrEmpty(organisationName);

            // check is is there and is 2 characters or more  MFCMFC
            var organisationNameFormatValid = _organisationValidator.CheckOrganisationName(organisationName) == string.Empty;

            if (organisationNameEmpty)
                warningMessages.Add(noOrganisationNameMessage);

            if (!organisationNameFormatValid)
                warningMessages.Add(organisationNameTooShortMessage);

            var organisationNameNotAlreadyUsed = _organisationValidator.CheckOrganisationNameNotUsed(organisationName) == string.Empty;

            if (!organisationNameNotAlreadyUsed)
                warningMessages.Add(organisationNameAlreadyUsedMessage);

            // If details make adding organisation impossible, then eject here
            if (warningMessages.Count>0)
                return new CreateOrganisationContactResponse(null, false, false, warningMessages);

            if (organisationTypeId == null)
                warningMessages.Add(organisationTypeNotIdentifiedMessage);

            var ukprnValid = _organisationValidator.CheckUkprnIsValid(ukprnAsLong)==string.Empty; // check it's a validly formed ukprn ? use new endpoint?

            if (!ukprnValid)
            {
                ukprnAsLong = null;
                warningMessages.Add(ukprnNotValidMessage);
            }

            var ukprnAlreadyUsed = _organisationValidator.CheckIfOrganisationUkprnExists(ukprnAsLong) != string.Empty;
            if (ukprnAlreadyUsed)
            {
                ukprnAsLong = null;
                warningMessages.Add(ukprnAlreadyUsedMessage);
            }
            
            var companyNumberValid = _organisationValidator.CheckCompanyNumberIsValid(companyNumber) == string.Empty;  // use new service?
            if (!companyNumberValid)
            {
                companyNumber = null;
                warningMessages.Add(companyNumberNotValidMessage);
            }

            var companyNumberAlreadyUsed = _organisationValidator.CheckIfOrganisationCompanyNumberExists(companyNumber) != string.Empty;
            if (!companyNumberAlreadyUsed)
            {
                companyNumber = null;
                warningMessages.Add(companyNumberAlreadyUsedMessage);
            }

            var charityNumberValid = _organisationValidator.CheckCharityNumberIsValid(charityNumber) == string.Empty;  // use new service?
            if (!charityNumberValid)
            {
                charityNumber = null;
                warningMessages.Add(charityNumberNotValidMessage);
            }

            var charityNumberAlreadyUsed = _organisationValidator.CheckIfOrganisationCharityNumberExists(charityNumber) != string.Empty;
            if (charityNumberAlreadyUsed)
            {
                charityNumber = null;
                warningMessages.Add(charityNumberAlreadyUsedMessage);
            }

            // do the organisation insert if name is present and get the new organisation Id

            var organisationId = "ABC"; // this is gathered at the point of adding the organisation;


            // contact checks (as long as an organisation is added)

            //return _organisationValidator.ValidatorCreateEpaOrganisationContactRequest(new CreateEpaOrganisationContactRequest...
            
            // maybe separate the test to change message?
            var emailPresentAndValid = _organisationValidator.CheckIfEmailIsPresentAndInSuitableFormat(request.ContactEmail) == string.Empty;
            var emailAleadyPresentInOtherOrganisation = _organisationValidator.CheckIfEmailAlreadyPresentInAnotherOrganisation(request.ContactEmail, organisationId) != string.Empty;

            var contactDetailsAlreadyPresent = emailPresentAndValid && !emailAleadyPresentInOtherOrganisation && _organisationValidator.CheckIfContactDetailsAlreadyPresentInSystem(request.ContactName, request.ContactEmail, request.ContactPhoneNumber, null)!=string.Empty;

            // redundant I think
            //var organisationNotFound = _organisationValidator.CheckIfOrganisationNotFound(organisationId) == string.Empty;

            // checking if present and 2 characters or more....
            var contactNameValid = _organisationValidator.CheckDisplayName(request.ContactName) == string.Empty;
            

        

            // do the contact update and mark contactAdded is true, if it is done

            return new CreateOrganisationContactResponse(organisationId, organisationAdded, contactAdded, warningMessages);
        }

        private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        {
            var organisationTypes = await _registerQueryRepository.GetOrganisationTypes();
           return organisationTypes.FirstOrDefault(x => string.Equals(x.Type.Replace(" ", ""),
                organisationType.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private static long? GetUkprnFromRequestDetails(string organisationUkprn, string companyUkprn )
        {

            long? ukprnAsLong = null;
            var ukprn = !string.IsNullOrEmpty(organisationUkprn) ? organisationUkprn : companyUkprn;

            if (long.TryParse(ukprn, out long _))
            {
                ukprnAsLong = long.Parse(ukprn);
            }
            return ukprnAsLong;
        }

        private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        {
            return useTradingName && !string.IsNullOrEmpty(tradingName)
                ? tradingName
                : organisationName;
        }
    }
}
