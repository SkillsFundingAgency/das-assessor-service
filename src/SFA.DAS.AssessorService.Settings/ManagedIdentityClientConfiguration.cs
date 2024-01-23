namespace SFA.DAS.AssessorService.Settings
{
    public class ManagedIdentityClientConfiguration : IManagedIdentityClientConfiguration
    {
         public string IdentifierUri { get; set; }

         public string ApiBaseUrl { get; set; }
    }
}
