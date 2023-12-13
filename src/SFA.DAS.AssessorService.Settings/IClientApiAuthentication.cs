namespace SFA.DAS.AssessorService.Settings
{
    public interface IClientApiAuthentication :IManagedIdentityApiAuthentication
    {
        string Instance { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string ResourceId { get; set; }
        string TenantId { get; set; }
        string ApiBaseAddress { get; set; }
    }
}