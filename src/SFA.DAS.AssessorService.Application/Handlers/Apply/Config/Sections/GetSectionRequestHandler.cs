using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sections
{
    public class GetSectionRequestHandler : IRequestHandler<GetSectionRequest, Section>
    {
        private readonly IApplyRepository _applyRepository;

        public GetSectionRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Section> Handle(GetSectionRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var sequence = sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);
            
            if (sequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            var section = sequence.Sections.FirstOrDefault(s => s.SectionId == request.SectionId);
            
            return section;
        }
    }
}