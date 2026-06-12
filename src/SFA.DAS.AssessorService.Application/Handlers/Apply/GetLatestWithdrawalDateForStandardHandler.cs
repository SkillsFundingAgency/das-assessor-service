using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class GetLatestWithdrawalDateForStandardHandler : IRequestHandler<GetLatestWithdrawalDateForStandardRequest, DateTime?>
    {
        private readonly IApplyRepository _applyRepository;

        public GetLatestWithdrawalDateForStandardHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<DateTime?> Handle(GetLatestWithdrawalDateForStandardRequest request, CancellationToken cancellationToken)
        {
            var result = await _applyRepository.GetLatestWithdrawalDateForStandard(request.OrganisationId, request.StandardCode);
            return result;
        }
    }
}
