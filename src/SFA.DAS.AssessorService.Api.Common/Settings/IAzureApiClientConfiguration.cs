namespace SFA.DAS.AssessorService.Api.Common
{
    public interface IAzureApiClientConfiguration
    {
        string ApiBaseAddress { get; set; }

        string Id { get; set; }

        string Key { get; set; }

        string ProductId { get; set; }

        string GroupId { get; set; }
    }
}
