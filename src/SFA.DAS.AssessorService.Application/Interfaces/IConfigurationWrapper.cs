namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IConfigurationWrapper
    {
        string DbConnectionString { get; }
        string AssessmentOrgsUrl { get; }
        string GitPassword { get; }
        string GitUserName { get; }
    }
}