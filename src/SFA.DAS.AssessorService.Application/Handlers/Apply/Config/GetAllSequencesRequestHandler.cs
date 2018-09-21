using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config
{
    public class GetAllSequencesRequestHandler : IRequestHandler<GetAllSequencesRequest, List<SequenceSummary>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetAllSequencesRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<List<SequenceSummary>> Handle(GetAllSequencesRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var sequenceSummaries = sequences.Select(s => new SequenceSummary(s)).ToList();   
            
            return sequenceSummaries;
        }
    }
}