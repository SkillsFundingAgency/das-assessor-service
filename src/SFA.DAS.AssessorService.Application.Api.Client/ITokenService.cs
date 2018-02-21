namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public interface ITokenService
    {
        string GetJwt(string userKey);
    }
}