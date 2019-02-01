using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationIdIsPresentAndValid(string organisationId);
        string CheckOrganisationName(string name);
        string CheckIfOrganisationAlreadyExists(string organisationId);
        string CheckIfOrganisationUkprnExists(long? ukprn);
        string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore);
        string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId);
        string CheckOrganisationTypeExists(int? organisationTypeId);
        string CheckIfOrganisationNotFound(string organisationId); 
        string CheckUkprnIsValid(long? ukprn);
        Standard GetStandard(int standardCode);
        string CheckIfOrganisationStandardAlreadyExists(string organisationId, int standardCode);
        string CheckOrganisationNameNotUsed(string name);
        string CheckOrganisationNameNotUsedForOtherOrganisations(string name, string organisationIdToIgnore);
        string CheckContactIdExists(string contactId);
        string CheckSearchStringForStandardsIsValid(string searchstring);

        string CheckIfContactIdIsValid(string contactId, string organisationId);
        string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode);
        string CheckDisplayName(string displayName);
        string CheckIfEmailIsPresentAndInSuitableFormat(string email);
        string CheckIfEmailAlreadyPresentInAnotherOrganisation(string email, string organisationId);
        string CheckIfEmailAlreadyPresentInOrganisationNotAssociatedWithContact(string email, string contactId);
        string CheckIfDeliveryAreasAreValid(List<int> DeliveryAreas);

        string CheckOrganisationStandardMakeLiveOrganisationStatus(string organisationStatus, string organisationStandardStatus);
        string CheckOrganisationStandardMakeLiveEffectiveFrom(DateTime? effectiveFrom, string organisationStandardStatus);
        string CheckIfContactDetailsAlreadyPresentInSystem(string displayName, string email, string phone,
            string contactId);

        string CheckOrganisationStandardFromDateIsWithinStandardDateRanges(DateTime? effectiveFrom,
            DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo, DateTime? lastDateForNewStarts);

        string CheckEffectiveFromIsOnOrBeforeEffectiveTo(DateTime? effectiveFrom, DateTime? effectiveTo);
        string CheckOrganisationStandardToDateIsWithinStandardDateRanges(DateTime? effectiveTo,
            DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo);

        string CheckAddressDetailsForOrganisation(string address1, string address2, string address3, string address4);
        string CheckPostcodeIsPresentForOrganisation(string postcode);
        string CheckContactCountForOrganisation(int? numberOfContacts);
        string CheckStandardCountForOrganisation(int? numberOfStandards);

        string CheckCompanyNumberIsValid(string companyNumber);
        string CheckIfOrganisationCompanyNumberExists(string organisationIdToExclude, string companyNumber);
        string CheckCharityNumberIsValid(string charityNumber);
        string CheckIfOrganisationCharityNumberExists(string organisationIdToExclude, string charityNumber);

        ValidationResponse ValidatorCreateEpaOrganisationRequest(CreateEpaOrganisationRequest request);
        ValidationResponse ValidatorCreateEpaOrganisationContactRequest(CreateEpaOrganisationContactRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationContactRequest(UpdateEpaOrganisationContactRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationRequest(UpdateEpaOrganisationRequest request);
        ValidationResponse ValidatorCreateEpaOrganisationStandardRequest(CreateEpaOrganisationStandardRequest request);
        ValidationResponse ValidatorUpdateEpaOrganisationStandardRequest(UpdateEpaOrganisationStandardRequest request);
        ValidationResponse ValidatorSearchStandardsRequest(SearchStandardsValidationRequest request);

    }
}
