using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
