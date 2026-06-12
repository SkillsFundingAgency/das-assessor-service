namespace SFA.DAS.AssessorService.Application.Api.Consts
{
    public static class EpaOrganisationValidatorMessageName
    {
        public const string NoOrganisationId = "NoOrganisationId";
        public const string OrganisationNotFound = "OrganisationNotFound";
        public const string OrganisationIdTooLong = "OrganisationIdTooLong";
        public const string OrganisationNameEmpty = "OrganisationNameEmpty";
        public const string OrganisationNameTooShort = "OrganisationNameTooShort";
        public const string OrganisationIdAlreadyUsed = "OrganisationIdAlreadyUsed";
        public const string UkprnAlreadyUsed = "UkprnAlreadyUsed";
        public const string OrganisationCompanyNumberNotValid = "OrganisationCompanyNumberNotValid";
        public const string OrganisationCharityNumberNotValid = "OrganisationCharityNumberNotValid";
        public const string OrganisationCompanyNumberAlreadyUsed = "OrganisationCompanyNumberAlreadyUsed";
        public const string OrganisationCharityNumberAlreadyUsed = "OrganisationCharityNumberAlreadyUsed";
        public const string OrganisationTypeIsInvalid = "OrganisationTypeIsInvalid";
        public const string AnotherOrganisationUsingTheUkprn = "AnotherOrganisationUsingTheUkprn";
        public const string UkprnIsInvalid = "UkprnIsInvalid";
        public const string ContactIdInvalidForOrganisationId = "ContactIdInvalidForOrganisationId";
        public const string OrganisationStandardAlreadyExists = "OrganisationStandardAlreadyExists";
        public const string OrganisationStandardVersionAlreadyExists = "OrganisationStandardVersionAlreadyExists";
        public const string StandardNotFound = "StandardNotFound";
        public const string OrganisationStandardDoesNotExist = "OrganisationStandardDoesNotExist";
        public const string EmailIsMissing = "EmailIsMissing";
        public const string EmailAlreadyExists = "EmailAlreadyExists";
        public const string ContactIdIsRequired = "ContactIdIsRequired";   
        public const string DisplayNameTooShort = "DisplayNameTooShort";
        public const string FirstNameTooShort = "FirstNameTooShort";
        public const string LastNameTooShort = "LastNameTooShort";
        public const string DisplayNameIsMissing = "DisplayNameIsMissing";
        public const string LastNameIsMissing = "LastNameIsMissing";
        public const string FirstNameIsMissing = "FirstNameIsMissing";
        public const string ContactIdDoesntExist = "ContactIdDoesntExist";
        public const string EmailIsIncorrectFormat = "EmailIsIncorrectFormat";
        public const string ErrorMessageOrganisationNameAlreadyPresent = "ErrorMessageOrganisationNameAlreadyPresent";
        public const string NoDeliveryAreasPresent = "NoDeliveryAreasPresent";
        public const string DeliveryAreaNotValid = "DeliveryAreaNotValid";
        public const string SearchStandardsTooShort = "SearchStandardsTooShort";
        public const string OrganisationStandardIdIsRequired = "OrganisationStandardIdIsRequired";
        public const string ApplicationIdIsRequired = "ApplicationIdIsRequired";
        public const string RecognitionNumberAlreadyInUse = "RecognitionNumberAlreadyInUse";
        public const string RecognitionNumberNotFound = "RecognitionNumberNotFound";

        public const string OrganisationStandardEffectiveFromBeforeStandardEffectiveFrom = "OrganisationStandardEffectiveFromBeforeStandardEffectiveFrom";
        public const string OrganisationStandardEffectiveToBeforeStandardEffectiveFrom = "OrganisationStandardEffectiveToBeforeStandardEffectiveFrom";
        public const string OrganisationStandardEffectiveFromAfterEffectiveTo = "OrganisationStandardEffectiveFromAfterEffectiveTo";

        public const string OrganisationStandardVersionEffectiveToBeforeStandardEffectiveFrom = "OrganisationStandardVersionEffectiveToBeforeStandardEffectiveFrom";
        public const string OrganisationStandardVersionEffectiveToAfterStandardEffectiveTo = "OrganisationStandardVersionEffectiveToAfterStandardEffectiveTo";
        public const string OrganisationStandardVersionEffectiveFromAfterEffectiveTo = "OrganisationStandardVersionEffectiveFromAfterEffectiveTo";
        public const string OrganisationStandardVersionEffectiveFromBeforeStandardEffectiveFrom = "OrganisationStandardVersionEffectiveFromBeforeStandardEffectiveFrom";
        public const string OrganisationStandardVersionEffectiveFromAfterStandardEffectiveTo = "OrganisationStandardVersionEffectiveFromAfterStandardEffectiveTo";
        public const string OrganisationStandardVersionEffectiveFromAfterStandardLastDayForNewStarts = "OrganisationStandardVersionEffectiveFromAfterStandardLastDayForNewStarts";
        public const string OrganisationStandardVersionEffectiveToBeforeEffectiveFrom = "OrganisationStandardVersionEffectiveToBeforeEffectiveFrom";

        public const string ContactDetailsAreDuplicates = "ContactDetailsAreDuplicates";
        public const string OrganisationStandardCannotBeUpdatedBecauseOrganisationNotLive = "OrganisationStandardCannotBeUpdatedBecauseOrganisationNotLive";
        public const string OrganisationStandardCannotBeMadeLiveBecauseEffectiveFromNotSet = "OrganisationStandardCannotBeMadeLiveBecauseEffectiveFromNotSet";
        public const string OrganisationStandardCannotBeUpdatedBecauseEffectiveFromNotSet = "OrganisationStandardCannotBeUpdatedBecauseEffectiveFromNotSet";
        public const string OrganisationStandardCannotBeAddedBecauseEffectiveFromNotSet = "OrganisationStandardCannotBeAddedBecauseEffectiveFromNotSet";
        public const string OrganisationTypeIsRequired = "OrganisationTypeIsRequired";
        public const string AddressIsNotEntered = "AddressIsNotEntered";
        public const string PostcodeIsNotEntered = "PostcodeIsNotEntered";
        public const string UkprnIsNotPresent = "UkprnIsNotPresent";
        public const string ContactsAreNotPresent = "ContactsAreNotPresent";
        public const string StandardsAreNotPresent = "StandardsAreNotPresent";
    }
}
