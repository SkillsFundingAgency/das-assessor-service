namespace SFA.DAS.AssessorService.Web.Services
{
    public interface ICache
    {
        string GetString(string key);
        void SetString(string key, string value);
    }
}