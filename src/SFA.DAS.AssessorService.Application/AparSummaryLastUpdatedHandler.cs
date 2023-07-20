using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application
{
    public class AparSummaryLastUpdatedHandler
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<AparSummaryLastUpdatedHandler> _logger;

        public AparSummaryLastUpdatedHandler(IRegisterQueryRepository registerQueryRepository, ILogger<AparSummaryLastUpdatedHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<DateTime> Handle()
        {
            _logger.LogInformation("Handling Get Last APAR Summary Update");

            return await _registerQueryRepository.AparSummaryLastUpdated();
        }
    }
}
