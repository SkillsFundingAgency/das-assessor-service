using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Questions
{
    public class QuestionHandlerBase
    {
        private readonly IApplyRepository _applyRepository;
        protected List<Sequence> Sequences;

        public QuestionHandlerBase(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        protected async Task<Page> GetPage(IQuestionRequest request)
        {
            var section = await GetSection(request);

            var page = section.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page == null)
            {
                throw new BadRequestException(
                    $"Page {request.PageId} does not exist in Section {request.SectionId} and Sequence {request.SequenceId}");
            }

            return page;
        }

        protected async Task<Section> GetSection(IQuestionRequest request)
        {
            Sequences = await _applyRepository.GetWorkflowDefinition();
            var sequence = Sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);

            if (sequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }

            var section = sequence.Sections.FirstOrDefault(s => s.SectionId == request.SectionId);

            if (section == null)
            {
                throw new BadRequestException($"Section {request.SectionId} does not exist in Sequence {request.SequenceId}");
            }

            return section;
        }

        protected async Task StoreWorkflow()
        {
            var workflowJson = JsonConvert.SerializeObject(Sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);
        }

        protected static void ReorderSequencesToMakeRoomForNewOne(CreateQuestionRequest request, List<Question> questions)
        {
            questions.Where(s => s.Order >= request.Question.Order).ToList().ForEach(s => s.Order++);
        }

        protected static void SetOrderToMaxPlusOne(CreateQuestionRequest request, List<Question> questions)
        {
            request.Question.Order = questions.Max(s => s.Order).Value + 1;
        }
    }
}