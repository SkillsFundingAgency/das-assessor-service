namespace SFA.DAS.AssessorService.Settings
{
    public interface IAzureApiAuthentication
    {
        string ApiBaseAddress { get; set; }
        string Id { get; set; }
        string Key { get; set; }
        string ProductId { get; set; }
        string GroupId { get; set; }
        string DefaultPassword { get; set; }
    }
}
