namespace SFA.DAS.AssessorService.Api.Common
{
    public class CharityCommissionApiClientConfiguration : ICharityCommissionApiClientConfiguration
    {
        public string ApiKey { get; set; }

        public string ApiBaseAddress { get; set; }
    }
}
