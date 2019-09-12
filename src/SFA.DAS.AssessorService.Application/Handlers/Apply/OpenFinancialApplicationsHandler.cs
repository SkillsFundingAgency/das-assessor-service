using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class OpenFinancialApplicationsHandler : IRequestHandler<OpenFinancialApplicationsRequest, List<FinancialApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;

        public OpenFinancialApplicationsHandler(IApplyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FinancialApplicationSummaryItem>> Handle(OpenFinancialApplicationsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetOpenFinancialApplications();
        }
    }
}
