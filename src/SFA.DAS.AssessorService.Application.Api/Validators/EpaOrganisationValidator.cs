using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterRepository _registerRepository;

        public string ErrorMessageNoOrganisationId { get; } = "There is no organisation Id; ";
        public string ErrorMessageOrganisationIdTooLong { get; } = "The length of the organisation Id is too long; ";
        public string ErrorMessageOrganisationNameEmpty { get; } = "There is no organisation name; ";
        public string ErrorMessageOrganisationIdAlreadyUsed { get; } = "There is already an entry for this organisation Id; ";
        public string ErrorMessageUkprnAlreadyUsed { get; } = "There is already an organisation with this ukrpn; ";

        public string ErrorMessageOrganisationTypeIsInvalid { get; } = "There is no organisation type with this Id; ";

        public EpaOrganisationValidator( IRegisterRepository registerRepository)
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
    }
}
