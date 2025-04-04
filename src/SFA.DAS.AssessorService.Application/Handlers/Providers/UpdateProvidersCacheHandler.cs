using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Providers
{
    public class UpdateProvidersCacheHandler : IRequestHandler<UpdateProvidersCacheRequest, Unit>
    {
        private readonly ILogger<UpdateProvidersCacheHandler> _logger;
        private readonly IApprovalsExtractRepository _approvalsExtractRepository;

        public UpdateProvidersCacheHandler(
            ILogger<UpdateProvidersCacheHandler> logger
            , IApprovalsExtractRepository approvalsExtractRepository
            )
        {
            _logger = logger;
            _approvalsExtractRepository = approvalsExtractRepository;
        }

        public async Task<Unit> Handle(UpdateProvidersCacheRequest request, CancellationToken cancellationToken)
        {
            try
            {
                switch(request.UpdateType)
                {
                    case ProvidersCacheUpdateType.InsertProvidersFromApprovalsExtract:
                        await _approvalsExtractRepository.InsertProvidersFromApprovalsExtract();
                        break;
                    default:
                        await _approvalsExtractRepository.RefreshProviders();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Providers cache failed to update successfully.");
                throw;
            }

            return Unit.Value;
        }
    }
}
