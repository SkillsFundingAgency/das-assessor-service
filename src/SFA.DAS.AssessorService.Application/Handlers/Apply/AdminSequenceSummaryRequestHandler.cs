using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class AdminSequenceSummaryRequestHandler : IRequestHandler<AdminSequenceSummaryRequest, List<SequenceSummary>>
    {
        private readonly IApplyRepository _applyRepository;

        public AdminSequenceSummaryRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<List<SequenceSummary>> Handle(AdminSequenceSummaryRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetSequences(request.WorkflowId);
            
            var sequenceSummaries = sequences.Select(s => new SequenceSummary(s)).ToList();
            
            return sequenceSummaries;
        }
    }
}