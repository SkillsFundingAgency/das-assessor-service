namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{
    public interface IHeaderInfo
    {
        int Ukprn { get; set; }
        string Username { get; set; }
    }
}