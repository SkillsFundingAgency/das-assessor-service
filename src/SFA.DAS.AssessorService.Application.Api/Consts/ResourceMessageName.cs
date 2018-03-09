namespace SFA.DAS.AssessorService.Application.Api.Consts
{
    public static class ResourceMessageName
    {      
        public const string InvalidUkprn = "InvalidUKPRN";            
        public const string AlreadyExists = "AlreadyExists";
        public const string DoesNotExist = "DoesNotExist";
        public const string MustBeDefined = "MustBeDefined";

        public const string NoAssesmentProviderFound = "NoAssesmentProviderFound";

        //public const string DisplayNameMustBeDefined = "DisplayNameMustBeDefined";
        //public const string EMailMustBeDefined = "EMailMustBeDefined";
        //public const string UserNameMustBeDefined = "UserNameMustBeDefined";
        public const string MaxLengthError = "MaxLengthError";
        public const string HaveExistingOrganisation = "HaveExistingOrganisation";
    }
}