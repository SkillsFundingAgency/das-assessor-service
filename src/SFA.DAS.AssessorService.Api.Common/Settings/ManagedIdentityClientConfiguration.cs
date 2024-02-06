namespace SFA.DAS.AssessorService.Api.Common.Settings
{
    public class ManagedIdentityClientConfiguration : IManagedIdentityClientConfiguration
    {
         public string IdentifierUri { get; set; }

         public string ApiBaseUrl { get; set; }
    }
}
