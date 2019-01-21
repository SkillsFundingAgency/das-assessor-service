namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{
    public class HeaderInfo : IHeaderInfo
    {
        public int Ukprn { get; set; }
        public string Email { get; set; }
    }
}