namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiAuthentication
    {
        string TenantId { get; set; }
        string Audience { get; set; }
    }
}