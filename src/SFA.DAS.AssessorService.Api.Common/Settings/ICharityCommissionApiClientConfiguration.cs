namespace SFA.DAS.AssessorService.Api.Common
{
    public interface ICharityCommissionApiClientConfiguration
    {
        string ApiBaseAddress { get; set; }

        string ApiKey { get; set; }
    }
}