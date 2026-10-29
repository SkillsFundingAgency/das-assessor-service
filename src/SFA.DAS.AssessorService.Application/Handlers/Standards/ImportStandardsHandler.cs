using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class ImportStandardsHandler : IRequestHandler<ImportStandardsRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOuterApiService outerApiService;
        private readonly IStandardImportService standardImportService;
        private readonly ILogger<ImportStandardsHandler> logger;

        public ImportStandardsHandler(IUnitOfWork unitOfWork, IOuterApiService outerApiService, IStandardImportService standardImportService, ILogger<ImportStandardsHandler> logger)
        {
            this.unitOfWork = unitOfWork;
            this.outerApiService = outerApiService;
            this.standardImportService = standardImportService;
            this.logger = logger;
        }

        public async Task<Unit> Handle(ImportStandardsRequest request, CancellationToken cancellationToken)
        {
            var allStandards = await outerApiService.GetAllStandards();
            if (!allStandards.Any())
            {
                logger.LogWarning("Outer API did not return any standards");
                return Unit.Value;
            }

            try
            {
                unitOfWork.Begin();

                await standardImportService.PrepareImport();

                await standardImportService.StageStandards(allStandards);

                await standardImportService.StageOptions(allStandards);

                await standardImportService.MergeStandardsFromStaging();

                unitOfWork.Commit();

                logger.LogInformation("Standards import completed.");
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
