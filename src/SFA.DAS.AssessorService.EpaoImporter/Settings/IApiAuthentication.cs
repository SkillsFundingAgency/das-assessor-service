namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public interface IApiAuthentication
    {
        string ClientId { get; set; }
        string Instance { get; set; }
        string Domain { get; set; }
        string TenantId { get; set; }
        string Audience { get; set; }
    }
}