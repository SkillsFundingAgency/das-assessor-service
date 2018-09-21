using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Pages
{
    public class CreatePageRequestHandler : IRequestHandler<CreatePageRequest, Page>
    {
        private readonly IApplyRepository _applyRepository;

        public CreatePageRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Page> Handle(CreatePageRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var thisSequence = sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);

            if (thisSequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            
            var section = thisSequence.Sections.FirstOrDefault(s => s.SectionId == request.SectionId);
            
            if (section == null)
            {
                throw new BadRequestException($"Section {request.SectionId} does not exist in Sequence {request.SequenceId}");
            }
            
            var pages = section.Pages;
            
            if (pages.Exists(s => s.PageId == request.Page.PageId))
            {
                throw new BadRequestException("PageId already exists.");
            }
            
            if (request.Page.Order == null)
            {
                SetOrderToMaxPlusOne(request, pages);
            }
            else
            {
                ReorderSequencesToMakeRoomForNewOne(request, pages);
            }
            
            pages.Insert(request.Page.Order.Value, request.Page);
            var workflowJson = JsonConvert.SerializeObject(sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);

            return request.Page;
        }

        private static void ReorderSequencesToMakeRoomForNewOne(CreatePageRequest request, List<Page> pages)
        {
            pages.Where(s => s.Order >= request.Page.Order).ToList().ForEach(s => s.Order++);
        }

        private static void SetOrderToMaxPlusOne(CreatePageRequest request, List<Page> pages)
        {
            request.Page.Order = pages.Max(s => s.Order).Value + 1;
        }
    }
}