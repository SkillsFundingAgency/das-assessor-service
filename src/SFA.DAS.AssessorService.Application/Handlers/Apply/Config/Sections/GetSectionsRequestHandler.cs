using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sections
{
    public class GetSectionsRequestHandler : IRequestHandler<GetSectionsRequest, List<SectionSummary>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetSectionsRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<List<SectionSummary>> Handle(GetSectionsRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();

            var sequence = sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);

            if (sequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            var sectionSummaries = sequence.Sections.Select(s => new SectionSummary(s)).ToList();
            
            return sectionSummaries;
        }
    }
}