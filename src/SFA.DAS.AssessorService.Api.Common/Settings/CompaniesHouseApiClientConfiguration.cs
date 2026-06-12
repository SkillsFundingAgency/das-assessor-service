namespace SFA.DAS.AssessorService.Api.Common
{
    public class CompaniesHouseApiClientConfiguration : ICompaniesHouseApiClientConfiguration
    {
        public string ApiKey { get; set; }

        public string ApiBaseAddress { get; set; }
    }
}
