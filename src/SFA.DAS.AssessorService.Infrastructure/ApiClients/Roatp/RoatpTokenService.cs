using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public class RoatpTokenService : TokenService, IRoatpTokenService
    {
        public RoatpTokenService(IClientConfiguration clientConfiguration, ILogger<TokenService> logger) : base(clientConfiguration, logger)
        {
        }
    }

    public interface IRoatpTokenService : ITokenService { }
}
