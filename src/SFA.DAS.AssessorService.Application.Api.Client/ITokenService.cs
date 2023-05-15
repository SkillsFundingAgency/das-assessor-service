namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public interface ITokenService
    {
        string GetToken();
    }

    public interface IAssessorTokenService : ITokenService { }
    public interface IQnATokenService : ITokenService { }
    public interface IRoATPTokenService : ITokenService { }
}