namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiSettings
    {
        string TokenEncodingKey { get; set; }
        string ApiBaseAddress { get; set; }
    }
}