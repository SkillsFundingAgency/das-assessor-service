using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Pages
{
    public class GetPageRequestHandler : IRequestHandler<GetPageRequest, Page>
    {
        private readonly IApplyRepository _applyRepository;

        public GetPageRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Page> Handle(GetPageRequest request, CancellationToken cancellationToken)
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

            var page = section.Pages.FirstOrDefault(p => p.PageId == request.PageId);
            
            return page;
        }
    }
}