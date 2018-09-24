using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Questions
{
    public class CreateQuestionRequestHandler : QuestionHandlerBase, IRequestHandler<CreateQuestionRequest, Question>
    {
        public CreateQuestionRequestHandler(IApplyRepository applyRepository) : base(applyRepository) {}
        
        public async Task<Question> Handle(CreateQuestionRequest request, CancellationToken cancellationToken)
        {
            var page = await GetPage(request);
            
            var questions = page.Questions;
            
            if (questions.Exists(s => s.QuestionId == request.Question.QuestionId))
            {
                throw new BadRequestException("QuestionId already exists.");
            }
            
            if (request.Question.Order == null)
            {
                SetOrderToMaxPlusOne(request, questions);
            }
            else
            {
                ReorderSequencesToMakeRoomForNewOne(request, questions);
            }
            
            questions.Insert(request.Question.Order.Value, request.Question);

            await StoreWorkflow();

            return request.Question;
        }
    }
}