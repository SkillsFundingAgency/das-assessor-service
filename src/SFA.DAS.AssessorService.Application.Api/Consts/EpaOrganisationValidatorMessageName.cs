﻿namespace SFA.DAS.AssessorService.Application.Api.Consts
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
        public const string OrganisationCompanyNumberNotValid = "OrganisationCompanyNumberNotValid";
        public const string OrganisationCharityNumberNotValid = "OrganisationCharityNumberNotValid";
        public const string OrganisationCompanyNumberAlreadyUsed = "OrganisationCompanyNumberAlreadyUsed";
        public const string OrganisationCharityNumberAlreadyUsed = "OrganisationCharityNumberAlreadyUsed";
        public const string OrganisationTypeIsInvalid = "OrganisationTypeIsInvalid";
        public const string AnotherOrganisationUsingTheUkprn = "AnotherOrganisationUsingTheUkprn";
        public const string UkprnIsInvalid = "UkprnIsInvalid";
        public const string ContactIdInvalidForOrganisationId = "ContactIdInvalidForOrganisationId";
        public const string OrganisationStandardAlreadyExists = "This organisation/standard already exists";
        public const string OrganisationStandardVersionAlreadyExists = "One or more versions already exist";
        public const string StandardNotFound = "StandardNotFound";
        public const string OrganisationStandardDoesNotExist = "OrganisationStandardDoesNotExist";
        public const string EmailIsMissing = "EmailIsMissing";
        public const string EmailAlreadyPresentInAnotherOrganisation = "EmailAlreadyPresentInAnotherOrganisation";
        public const string EmailAlreadyPresentInCurrentOrganisation = "This email is being used by another contact";
        public const string ContactIdIsRequired = "ContactIdIsRequired";   
        public const string DisplayNameTooShort = "DisplayNameTooShort";
        public const string FirstNameTooShort = "First Name Too Short";
        public const string LastNameTooShort = "Last Name Is Too Short";
        public const string DisplayNameIsMissing = "DisplayNameIsMissing";
        public const string LastNameIsMissing = "Last Name Is Missing";
        public const string FirstNameIsMissing = "First Name Is Missing";
        public const string ContactIdDoesntExist = "ContactIdDoesntExist";
        public const string EmailIsIncorrectFormat = "EmailIsIncorrectFormat";
        public const string ErrorMessageOrganisationNameAlreadyPresent = "ErrorMessageOrganisationNameAlreadyPresent";
        public const string NoDeliveryAreasPresent = "NoDeliveryAreasPresent";
        public const string DeliveryAreaNotValid = "DeliveryAreaNotValid";
        public const string SearchStandardsTooShort = "SearchStandardsTooShort";
        public const string OrganisationStandardIdIsRequired = "OrganisationStandardIdIsRequired";
        public const string UpdatedByIsMissing = "Updated By Is Missing";
        public const string ApplicationIdIsMissing = "Application Id is Missing";
        public const string RecognitionNumberAlreadyInUse = "The Ofqual Recognition Number you have entered is already assigned to another organisation";
        public const string RecognitionNumberNotFound = "The Ofqual Recognition Number you have entered does not match Ofqual records";

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
