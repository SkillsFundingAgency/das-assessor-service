using SFA.DAS.AssessorService.Api.Common.Settings;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IExternalApiConfiguration
    {
        AssessorApiClientConfiguration AssessorApiAuthentication { get; set; }

        AssessorApiClientConfiguration SandboxAssessorApiAuthentication { get; set; }
    }
}