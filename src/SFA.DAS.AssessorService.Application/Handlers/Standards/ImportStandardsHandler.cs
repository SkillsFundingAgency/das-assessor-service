using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class ImportStandardsHandler : IRequestHandler<ImportStandardsRequest>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOuterApiService outerApiService;
        private readonly IStandardService standardService;
        private readonly ILogger<ImportStandardsHandler> logger;

        public ImportStandardsHandler(IUnitOfWork unitOfWork, IOuterApiService outerApiService, IStandardService standardService, ILogger<ImportStandardsHandler> logger)
        {
            this.unitOfWork = unitOfWork;
            this.outerApiService = outerApiService;
            this.standardService = standardService;
            this.logger = logger;
        }

        public async Task<Unit> Handle(ImportStandardsRequest request, CancellationToken cancellationToken)
        {
            var getAllStandardsTask = outerApiService.GetAllStandards();
            var getActiveStandardsTask = outerApiService.GetActiveStandards();
            await Task.WhenAll(getAllStandardsTask, getActiveStandardsTask);

            var allStandards = getAllStandardsTask.Result;
            var activeStandardUIds = getActiveStandardsTask.Result.Select(s => s.StandardUId);

            if (allStandards.Any() == false)
            {
                logger.LogWarning("Outer API did not return any standards");
                return Unit.Value;
            }

            var allStandardDetails = await outerApiService.GetAllStandardDetails(allStandards.Select(s => s.StandardUId));

            var activeStandardDetails = allStandardDetails.Where(s => activeStandardUIds.Contains(s.StandardUId)).ToList();

            try
            {
                unitOfWork.Begin();

                await standardService.LoadStandards(allStandardDetails);

                await standardService.UpsertStandardCollations(activeStandardDetails);

                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Importing of Standards could not be completed");
                unitOfWork.Rollback();
                throw;
            }

            return Unit.Value;
        }
    }
}
