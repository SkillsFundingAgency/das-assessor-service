namespace SFA.DAS.AssessorService.Api.Common
{
    public interface ICompaniesHouseApiClientConfiguration
    {
        string ApiBaseAddress { get; set; }

        string ApiKey { get; set; }
    }
}