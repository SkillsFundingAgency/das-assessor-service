using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class PageRequestHandler : IRequestHandler<PageRequest, Page>
    {
        private readonly IApplyRepository _applyRepository;

        public PageRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Page> Handle(PageRequest request, CancellationToken cancellationToken)
        {
            var workflow = await _applyRepository.GetSequences(request.UserId);
            
            var sequence = workflow.Single(w => w.Sections.Any(s => s.Pages.Any(p => p.PageId == request.PageId)));
            var section = sequence.Sections.Single(s => s.Pages.Any(p => p.PageId == request.PageId));

            if (!sequence.Active)
            {
                throw new UnauthorisedException("Sequence not active");
            }

            var page = section.Pages.Single(p => p.PageId == request.PageId);
            
            return page;
        }
    }
}