using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Questions
{
    public class GetQuestionsRequestHandler : QuestionHandlerBase, IRequestHandler<GetQuestionsRequest, List<Question>>
    {
        public GetQuestionsRequestHandler(IApplyRepository applyRepository) : base(applyRepository){}
        
        public async Task<List<Question>> Handle(GetQuestionsRequest request, CancellationToken cancellationToken)
        {
            var page = await GetPage(request);

            return page.Questions;
        }
    }
}