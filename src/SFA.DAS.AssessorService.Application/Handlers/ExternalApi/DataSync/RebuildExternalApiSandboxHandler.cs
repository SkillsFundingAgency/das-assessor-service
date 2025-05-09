﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.DataSync
{
    public class RebuildExternalApiSandboxHandler : IRequestHandler<RebuildExternalApiSandboxRequest, Unit>
    {
        private readonly ILogger<RebuildExternalApiSandboxHandler> _logger;
        private readonly ISandboxDbRepository _sandboxDbRepository;

        public RebuildExternalApiSandboxHandler(ILogger<RebuildExternalApiSandboxHandler> logger, ISandboxDbRepository sandboxDbRepository)
        {
            _logger = logger;
            _sandboxDbRepository = sandboxDbRepository;
        }

        public async Task<Unit> Handle(RebuildExternalApiSandboxRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rebuild External Api Sandbox Started");
            
            await _sandboxDbRepository.RebuildExternalApiSandbox();
            
            return Unit.Value;
        }
    }
}

