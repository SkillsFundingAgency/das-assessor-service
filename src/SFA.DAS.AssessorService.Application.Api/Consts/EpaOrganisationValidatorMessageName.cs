namespace SFA.DAS.AssessorService.Application.Api.Consts
{
    public class EpaOrganisationValidatorMessageName
    {
        public const string NoOrganisationId = "NoOrganisationId";
        public const string OrganisationNotFound = "OrganisationNotFound";
        public const string OrganisationIdTooLong = "OrganisationIdTooLong";
        public const string OrganisationNameEmpty = "OrganisationNameEmpty";
        public const string OrganisationNameTooShort = "OrganisationNameTooShort";
        public const string OrganisationIdAlreadyUsed = "OrganisationIdAlreadyUsed";
        public const string UkprnAlreadyUsed = "UkprnAlreadyUsed";
        public const string OrganisationTypeIsInvalid = "OrganisationTypeIsInvalid";
        public const string AnotherOrganisationUsingTheUkprn = "AnotherOrganisationUsingTheUkprn";
        public const string UkprnIsInvalid = "UkprnIsInvalid";
        public const string ContactIdInvalidForOrganisationId = "ContactIdInvalidForOrganisationId";
        public const string OrganisationStandardAlreadyExists = "This organisation/standard already exists";
        public const string StandardNotFound = "StandardNotFound";
        public const string OrganisationStandardDoesNotExist = "OrganisationStandardDoesNotExist";
        public const string EmailIsMissing = "EmailIsMissing";
        public const string EmailAlreadyPresentInAnotherOrganisation = "EmailAlreadyPresentInAnotherOrganisation";
        public const string ContactIdIsRequired = "ContactIdIsRequired";   
        public const string DisplayNameTooShort = "DisplayNameTooShort";
        public const string DisplayNameIsMissing = "DisplayNameIsMissing";
        public const string ContactIdDoesntExist = "ContactIdDoesntExist";
        public const string EmailIsIncorrectFormat = "EmailIsIncorrectFormat";
        public const string ErrorMessageOrganisationNameAlreadyPresent = "ErrorMessageOrganisationNameAlreadyPresent";
        public const string NoDeliveryAreasPresent = "NoDeliveryAreasPresent";
        public const string DeliveryAreaNotValid = "DeliveryAreaNotValid";
    }
}
