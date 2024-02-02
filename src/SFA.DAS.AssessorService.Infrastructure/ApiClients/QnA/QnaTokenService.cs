using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA
{
    public class QnaTokenService : TokenService, IQnaTokenService
    {
        public QnaTokenService(IClientConfiguration clientConfiguration, ILogger<TokenService> logger) : base(clientConfiguration, logger)
        {
        }
    }

    public interface IQnaTokenService : ITokenService { }
}
