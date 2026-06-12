namespace SFA.DAS.AssessorService.Api.Common
{
    public interface IOuterApiClientConfiguration
    {
        string BaseUrl { get; set; }

        string Key { get; set; }
    }
}