using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.DataSync
{
    public class RebuildExternalApiSandboxHandler : IRequestHandler<RebuildExternalApiSandboxRequest>
    {
        private readonly ILogger<RebuildExternalApiSandboxHandler> _logger;
        private readonly IWebConfiguration _config;
        private readonly ISandboxDbRepository _sandboxDbRepository;

        public RebuildExternalApiSandboxHandler(ILogger<RebuildExternalApiSandboxHandler> logger, IWebConfiguration config, ISandboxDbRepository sandboxDbRepository)
        {
            _logger = logger;
            _config = config;
            _sandboxDbRepository = sandboxDbRepository;
        }

        public async Task<Unit> Handle(RebuildExternalApiSandboxRequest request, CancellationToken cancellationToken)
        {
            await ExecuteDataSync();
            return Unit.Value;
        }

        private async Task ExecuteDataSync()
        {
            _logger.LogInformation("External Api Data Sync Function Started");
            _logger.LogInformation($"Process Environment = {EnvironmentVariableTarget.Process}");

            if (_config.ExternalApiDataSyncEnabled)
            {
                await _sandboxDbRepository.RebuildExternalApiSandbox();
            }
            else
            {
                _logger.LogInformation("External Api Data Sync is disabled at this time");
            }

            await Task.CompletedTask;
        }
    }
}

