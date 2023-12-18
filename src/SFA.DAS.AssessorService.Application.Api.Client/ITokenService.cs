using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }

    public interface IAssessorTokenService : ITokenService { }
    public interface IQnATokenService : ITokenService { }
    public interface IRoatpTokenService : ITokenService { }
    public interface IReferenceDataTokenService : ITokenService { }
}