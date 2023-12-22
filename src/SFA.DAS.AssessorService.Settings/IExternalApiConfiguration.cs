using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IExternalApiConfiguration
    {
        AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get; set; }

        AzureActiveDirectoryClientConfiguration SandboxAssessorApiAuthentication { get; set; }
    }
}