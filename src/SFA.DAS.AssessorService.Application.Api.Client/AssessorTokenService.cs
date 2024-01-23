using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class AssessorTokenService : TokenService, IAssessorTokenService
    {
        public AssessorTokenService(IClientConfiguration clientConfiguration, ILogger<TokenService> logger) : base(clientConfiguration, logger)
        {
        }
    }

    public interface IAssessorTokenService : ITokenService { }
}
