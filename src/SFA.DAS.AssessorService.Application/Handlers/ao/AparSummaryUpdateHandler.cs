using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class AparSummaryUpdateHandler
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<AparSummaryUpdateHandler> _logger;

        public AparSummaryUpdateHandler(IRegisterQueryRepository registerQueryRepository, ILogger<AparSummaryUpdateHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<int?> Handle()
        {
            _logger.LogInformation("Handling Updating APAR Summary");

            return await _registerQueryRepository.AparSummaryUpdate();
        }
    }
}
