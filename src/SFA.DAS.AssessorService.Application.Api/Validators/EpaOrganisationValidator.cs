using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterQueryRepository _registerRepository;

        public string ErrorMessageNoOrganisationId { get; } = "There is no organisation Id; ";
        public string ErrorMessageOrganisationIdTooLong { get; } = "The length of the organisation Id is too long; ";
        public string ErrorMessageOrganisationNameEmpty { get; } = "There is no organisation name; ";
        public string ErrorMessageOrganisationIdAlreadyUsed { get; } = "There is already an entry for this organisation Id; ";
        public string ErrorMessageUkprnAlreadyUsed { get; } = "There is already an organisation with this ukprn; ";
        public string ErrorMessageOrganisationTypeIsInvalid { get; } = "There is no organisation type with this Id; ";
        public string ErrorMessageAnotherOrganisationUsingTheUkprn { get; } = "The ukprn entered is already used by another organisation; ";
        public string ErrorMessageUkprnIsInvalid { get; } = "The ukprn is not the correct format or length; ";
        public string ErrorMessageOrganisationNotFound { get; } = "There is no organisation for the this organisation Id; ";

        public EpaOrganisationValidator( IRegisterQueryRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }
        
        public string CheckOrganisationId(string organisationId)
        {           
            if (string.IsNullOrEmpty(organisationId) || organisationId.Trim().Length==0)
            {
               return ErrorMessageNoOrganisationId;
            }

            if (organisationId.Trim().Length > 12)
            {
                return ErrorMessageOrganisationIdTooLong;
            }

            return string.Empty;
        }

        public string CheckOrganisationName(string organisationName)
        {
            if (string.IsNullOrEmpty(organisationName) || organisationName.Trim().Length==0)
                return ErrorMessageOrganisationNameEmpty;

            return string.Empty;
        }

        public string CheckIfOrganisationIdExists(string organisationId)
        {
            if (organisationId == null ||
                !_registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result) return string.Empty;
            return ErrorMessageOrganisationIdAlreadyUsed;
        }

        public string CheckIfOrganisationUkprnExists(long? ukprn)
        {
            if (ukprn == null || !_registerRepository.EpaOrganisationExistsWithUkprn(ukprn.Value).Result) return string.Empty;
            return ErrorMessageUkprnAlreadyUsed;
        }

        public string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId)
        {
            if (organisationTypeId == null|| _registerRepository.OrganisationTypeExists(organisationTypeId.Value).Result) return string.Empty;

            return ErrorMessageOrganisationTypeIsInvalid;
        }

        public string CheckUkprnIsValid(long? ukprn)
        {
            if (ukprn == null) return string.Empty;
            var isValid = ukprn >= 10000000 && ukprn <= 99999999;
            return isValid ? string.Empty : ErrorMessageUkprnIsInvalid;
        }

        public string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore)
        {
        if (ukprn == null || !_registerRepository.EpaOrganisationAlreadyUsingUkprn(ukprn.Value, organisationIdToIgnore).Result) return string.Empty;
            return ErrorMessageUkprnAlreadyUsed;
        }

        public string CheckIfOrganisationNotFound(string organisationId)
        {
            return _registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result 
                ? string.Empty : 
                ErrorMessageOrganisationNotFound;
        }
    }
}
