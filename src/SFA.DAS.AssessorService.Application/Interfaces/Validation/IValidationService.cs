namespace SFA.DAS.AssessorService.Application.Interfaces.Validation
{
    public interface IValidationService
    {
        bool CheckPhoneNumberIsValue(string phoneNumberToCheck);
        bool CheckEmailIsValid(string emailToCheck);
        bool CheckWebsiteLinkIsValid(string websiteLinkToCheck);        
        bool IsNotEmpty(string stringToCheck);
        bool UkprnIsNullOrEmptyOrValid(string ukprnToCheck);
        bool UkprnIsValid(string ukprnToCheck, out int ukprn);
        bool UlnIsValid(string ulnToCheck);
        bool IsMinimumLengthOrMore(string stringToCheck, int minimumLength);
        bool IsMaximumLengthOrLess(string stringToCheck, int maximumLength);
        bool DateIsValid(string dateToCheck);
        bool DateIsTodayOrInFuture(string dateToCheck);
        bool DateIsTodayOrInPast(string dateToCheck);
        bool OrganisationIdIsNullOrEmptyOrValid(string organisationIdToCheck);
        bool OrganisationIdIsValid(string organisationIdToCheck);

        bool CompanyNumberIsValid(string companyNumberToCheck);
        bool CharityNumberIsValid(string charityNumberToCheck);
    }
}