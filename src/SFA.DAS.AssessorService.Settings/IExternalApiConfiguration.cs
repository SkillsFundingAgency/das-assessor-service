namespace SFA.DAS.AssessorService.Settings
{
    public interface IExternalApiConfiguration
    {
        ClientApiAuthentication AssessorApiAuthentication { get; set; }

        ClientApiAuthentication SandboxAssessorApiAuthentication { get; set; }
    }
}