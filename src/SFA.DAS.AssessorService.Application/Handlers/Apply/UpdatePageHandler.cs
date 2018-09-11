using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class UpdatePageHandler : IRequestHandler<UpdatePageRequest, Page>
    {
        private readonly IDbConnection _connection;

        public UpdatePageHandler(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<Page> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId", request);
            
            var workflow = JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);

            var sequence = workflow.Single(w => w.Sections.Any(s => s.Pages.Any(p => p.PageId == request.PageId)));
            var section = sequence.Sections.Single(s => s.Pages.Any(p => p.PageId == request.PageId));

            if (!sequence.Active)
            {
                throw new BadRequestException("Sequence not active");
            }

            var page = section.Pages.Single(p => p.PageId == request.PageId);

            foreach (var answeredQuestion in request.Questions)
            {
                var questionIdDb = page.Questions.Single(q => q.QuestionId == answeredQuestion.QuestionId);

                foreach (var answeredQuestionOutput in answeredQuestion.Outputs)
                {
                    foreach (var outputValue in answeredQuestionOutput.Values)
                    {
                        if (!questionIdDb.Inputs.Select(v => v.InputId).Contains(outputValue.InputId))
                        {
                            throw new BadRequestException($"{outputValue.InputId} is not an input on this page.");
                        }
                    }
                }
                
                questionIdDb.Outputs = answeredQuestion.Outputs;
                questionIdDb.Complete = true;
            }

            MarkPageAsCompleteIfAllQuestionsComplete(page);

            MarkSequenceAsCompleteIfAllPagesComplete(sequence, section, workflow);
            
            var workflowJson = JsonConvert.SerializeObject(workflow);

            await _connection.ExecuteAsync("UPDATE UserWorkflows SET Workflow = @Workflow WHERE UserId = @UserId",
                new {request.UserId, Workflow = workflowJson});

            return page;
        }

        private static void MarkSequenceAsCompleteIfAllPagesComplete(Sequence sequence, Section section,
            List<Sequence> workflow)
        {
            sequence.Complete = section.Pages.All(p => p.Complete);

            if (sequence.Complete)
            {
                workflow.Single(w => w.SequenceId == sequence.NextSequenceId).Active = true;
            }
        }

        private static void MarkPageAsCompleteIfAllQuestionsComplete(Page page)
        {
            page.Complete = page.Questions.All(q => q.Complete);
        }
    }

}