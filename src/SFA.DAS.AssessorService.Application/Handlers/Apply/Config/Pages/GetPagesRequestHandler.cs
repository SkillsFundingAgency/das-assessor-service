using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Pages
{
    public class GetPagesRequestHandler : IRequestHandler<GetPagesRequest, List<Page>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetPagesRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<List<Page>> Handle(GetPagesRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var sequence = sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);

            if (sequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }

            var section = sequence.Sections.FirstOrDefault(s => s.SectionId == request.SectionId);
            
            if (section == null)
            {
                throw new BadRequestException($"Section {request.SectionId} does not exist in Sequence {request.SequenceId}");
            }

            return section.Pages;
        }
    }
}