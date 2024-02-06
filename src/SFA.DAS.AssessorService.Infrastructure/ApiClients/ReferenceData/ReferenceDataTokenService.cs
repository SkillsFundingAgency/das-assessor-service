using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData
{
    public class ReferenceDataTokenService : TokenService, IReferenceDataTokenService
    {
        public ReferenceDataTokenService(IClientConfiguration clientConfiguration, ILogger<TokenService> logger) : base(clientConfiguration, logger)
        {
        }
    }

    public interface IReferenceDataTokenService : ITokenService { }
}
