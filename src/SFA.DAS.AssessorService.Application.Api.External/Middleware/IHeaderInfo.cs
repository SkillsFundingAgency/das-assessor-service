namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{
    public interface IHeaderInfo
    {
        int Ukprn { get; set; }
        string Email { get; set; }
    }
}