namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public interface IClientApiAuthentication
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Domain { get; set; }
        string Instance { get; set; }
        string ResourceId { get; set; }
        string TenantId { get; set; }
        string ApiBaseAddress { get; set; }
    }
}