using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class AssessorValidationService: IAssessorValidationService
    {
        private readonly IRegisterValidationRepository _registerValidationRepository;

        public AssessorValidationService(IRegisterValidationRepository registerValidationRepository)
        {
            _registerValidationRepository = registerValidationRepository;
        }

        public async Task<bool> CheckIfContactDetailsAlreadyPresentInSystem(string displayName, string email, string phone, Guid? contactId)
        {
            return await _registerValidationRepository.ContactDetailsAlreadyExist(displayName, email, phone, contactId);
        }

        public async Task<bool> IsCharityNumberTaken(string charityNumber)
        {
            return string.IsNullOrWhiteSpace(charityNumber)
                ? false
                : await _registerValidationRepository.EpaOrganisationExistsWithCharityNumber(charityNumber);
        }

        public async Task<bool> IsCompanyNumberTaken(string companyNumber)
        {
            return string.IsNullOrWhiteSpace(companyNumber)
                ? false
                : await _registerValidationRepository.EpaOrganisationExistsWithCompanyNumber(companyNumber);
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? false
                : await _registerValidationRepository.EmailAlreadyPresent(email);
        }

        public async Task<bool> IsOrganisationNameTaken(string organisationName)
        {
            return string.IsNullOrWhiteSpace(organisationName)
                ? false
                : await _registerValidationRepository.EpaOrganisationAlreadyUsingName(organisationName, string.Empty);
        }

        public async Task<bool> IsOrganisationUkprnTaken(long ukprn)
        {
            return await _registerValidationRepository.EpaOrganisationExistsWithUkprn(ukprn);
        }
    }
}
