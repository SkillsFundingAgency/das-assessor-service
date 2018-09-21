using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sections
{
    public class CreateSectionRequestHandler : IRequestHandler<CreateSectionRequest, Section>
    {
        private readonly IApplyRepository _applyRepository;

        public CreateSectionRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Section> Handle(CreateSectionRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var thisSequence = sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);

            if (thisSequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            var sections = thisSequence.Sections;

            if (sections.Exists(s => s.SectionId == request.Section.SectionId))
            {
                throw new BadRequestException("SectionId already exists.");
            }
            
            if (request.Section.Order == null)
            {
                SetOrderToMaxPlusOne(request, sections);
            }
            else
            {
                ReorderSequencesToMakeRoomForNewOne(request, sections);
            }
            
            sections.Insert(request.Section.Order.Value, request.Section);
            var workflowJson = JsonConvert.SerializeObject(sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);

            return request.Section;
        }

        private static void ReorderSequencesToMakeRoomForNewOne(CreateSectionRequest request, List<Section> sections)
        {
            sections.Where(s => s.Order >= request.Section.Order).ToList().ForEach(s => s.Order++);
        }

        private static void SetOrderToMaxPlusOne(CreateSectionRequest request, List<Section> sections)
        {
            request.Section.Order = sections.Max(s => s.Order).Value + 1;
        }
    }
}