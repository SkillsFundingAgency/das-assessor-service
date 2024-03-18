namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiAuthentication
    {
        string Tenant { get; set; }
        string Audiences { get; set; }
    }
}