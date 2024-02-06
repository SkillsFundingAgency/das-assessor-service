namespace SFA.DAS.AssessorService.Api.Common
{
    public interface IAzureApiClientConfiguration
    {
        string Id { get; set; }

        string Key { get; set; }

        string ApiBaseAddress { get; set; }

        string ProductId { get; set; }

        string GroupId { get; set; }

        string RequestBaseAddress { get; set; }
    }
}
