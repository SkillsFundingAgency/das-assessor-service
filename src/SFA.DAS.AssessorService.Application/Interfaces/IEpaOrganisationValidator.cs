using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationIdIsPresentAndValid(string organisationId);
        string CheckOrganisationNameIsPresentAndValid(string name);
        string CheckIfOrganisationAlreadyExists(string organisationId);
        string CheckIfOrganisationUkprnExists(long? ukprn);
        string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore);
        string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId);
        string CheckOrganisationTypeExists(int? organisationTypeId);
        string CheckOrganisationExists(string organisationId); 
        string CheckUkprnIsValid(long? ukprn);
        string CheckIfOrganisationStandardAlreadyExists(string organisationId, int standardCode);
        string CheckOrganisationNameNotExists(string name);
        string CheckOrganisationNameNotExistsExcludingOrganisation(string name, string excludingOrganisationId);
        string CheckContactIdExists(string contactId);
        string CheckSearchStringForStandardsIsValid(string searchstring);

        string CheckIfContactIdIsValid(string contactId, string organisationId);
        string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode);
        string CheckDisplayNameIsPresentAndValid(string displayName);
        string CheckEmailPresentAndValid(string email);
        string CheckEmailNotExistsExcludingContact(string email, string excludingContactId);
        string CheckIfDeliveryAreasAreValid(List<int> DeliveryAreas);

        string CheckOrganisationStandardEffectiveFromIsEntered(DateTime? effectiveFrom);
        string CheckContactDetailsNotExists(string firstName, string lastName, string email, string phone,
            string contactId);

        string CheckOrganisationStandardFromDateIsWithinStandardDateRanges(DateTime? effectiveFrom,
            DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo, DateTime? lastDateForNewStarts);

        string CheckEffectiveFromIsOnOrBeforeEffectiveTo(DateTime? effectiveFrom, DateTime? effectiveTo);
        string CheckOrganisationStandardToDateIsWithinStandardDateRanges(DateTime? effectiveTo,
            DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo);

        string CheckAddressDetailsForOrganisation(string address1, string address2, string address3, string address4);
        string CheckPostcodeIsPresentForOrganisation(string postcode);

        string CheckUkprnForOrganisation(long? ukprn);
        string CheckContactCountForOrganisation(int? numberOfContacts);

        string CheckCompanyNumberIsMissingOrValid(string companyNumber);
        string CheckIfOrganisationCompanyNumberExists(string organisationIdToExclude, string companyNumber);
        string CheckIfOrganisationCompanyNumberExists(string companyNumber);
        string CheckCharityNumberIsValid(string charityNumber);
        string CheckIfOrganisationCharityNumberExists(string organisationIdToExclude, string charityNumber);
        string CheckIfOrganisationCharityNumberExists(string charityNumber);

        ValidationResponse ValidatorCreateEpaOrganisationRequest(CreateEpaOrganisationRequest request);
        ValidationResponse ValidatorCreateEpaOrganisationContactRequest(CreateEpaOrganisationContactRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationContactRequest(UpdateEpaOrganisationContactRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationRequest(UpdateEpaOrganisationRequest request);
        ValidationResponse ValidatorCreateEpaOrganisationStandardRequest(CreateEpaOrganisationStandardRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationStandardRequest(UpdateEpaOrganisationStandardRequest request);
        Task<ValidationResponse> ValidatorUpdateOrganisationStandardVersionRequest(UpdateOrganisationStandardVersionRequest request);
        ValidationResponse ValidatorWithdrawOrganisationRequest(WithdrawOrganisationRequest request);
        ValidationResponse ValidatorWithdrawStandardRequest(WithdrawStandardRequest request);
        ValidationResponse ValidatorSearchStandardsRequest(SearchStandardsValidationRequest request);

    }
}
