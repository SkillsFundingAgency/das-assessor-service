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
            page.Answers = new List<Answer>();
            
            foreach (var answer in request.Answers)
            {
                var questionIdDb = page.Questions.SingleOrDefault(q => q.QuestionId == answer.QuestionId);

                if (questionIdDb == null)
                {
                    throw new BadRequestException($"{answer.QuestionId} is not an question on this page.");
                }

                if (questionIdDb.Input.Type == "Checkbox" && answer.Value == "on")
                {
                    answer.Value = "Yes";
                }
                
                page.Answers.Add(answer);
            }

            page.Complete = true;

            MarkSequenceAsCompleteIfAllPagesComplete(sequence, workflow);
            
            var workflowJson = JsonConvert.SerializeObject(workflow);

            await _connection.ExecuteAsync("UPDATE UserWorkflows SET Workflow = @Workflow WHERE UserId = @UserId",
                new {request.UserId, Workflow = workflowJson});

            return page;
        }

        private static void MarkSequenceAsCompleteIfAllPagesComplete(Sequence sequence,
            List<Sequence> workflow)
        {
            sequence.Complete = sequence.Sections.SelectMany(s => s.Pages).All(p => p.Complete);

            if (!sequence.Complete) return;
            
            var nextSequence = sequence.NextSequence;
            if (nextSequence.Condition != null)
            {
                var answers = sequence.Sections.SelectMany(s => s.Pages).SelectMany(p => p.Answers).ToList();
                if (answers.Any(a => a.QuestionId == nextSequence.Condition.QuestionId && a.Value == nextSequence.Condition.MustEqual))
                {
                    workflow.Single(w => w.SequenceId == sequence.NextSequence.NextSequenceId).Active = true;
                }
            }
            else
            {
                workflow.Single(w => w.SequenceId == sequence.NextSequence.NextSequenceId).Active = true;
            }
        }
    }

}