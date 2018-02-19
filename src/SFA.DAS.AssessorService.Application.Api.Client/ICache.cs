namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public interface ICache
    {
        string GetString(string key);
        void SetString(string key, string value);
    }
}