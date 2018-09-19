using System;
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
using SFA.DAS.AssessorService.Application.Handlers.Apply.Validation;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class UpdatePageHandler : IRequestHandler<UpdatePageRequest, UpdatePageResult>
    {
        private readonly IDbConnection _connection;
        private IValidatorFactory _validatorFactory;

        public UpdatePageHandler(IDbConnection connection, IValidatorFactory validatorFactory)
        {
            _connection = connection;
            _validatorFactory = validatorFactory;
        }

        public async Task<UpdatePageResult> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId",
                    request);

            var workflow = JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);

            var sequence = workflow.Single(w => w.Sections.Any(s => s.Pages.Any(p => p.PageId == request.PageId)));
            var section = sequence.Sections.Single(s => s.Pages.Any(p => p.PageId == request.PageId));

            if (!sequence.Active)
            {
                throw new BadRequestException("Sequence not active");
            }

            var page = section.Pages.Single(p => p.PageId == request.PageId);
            page.Answers = new List<Answer>();


            var validationPassed = true;
            var validationErrors = new List<KeyValuePair<string, string>>();

            foreach (var question in page.Questions)
            {
                var answer = request.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                
                var validators = _validatorFactory.Build(question);
                foreach (var validator in validators)
                {
                    var errors = validator.Validate(question, answer);
                
                    if (errors.Any())
                    {
                        validationPassed = false;
                        validationErrors.AddRange(errors);
                    }
                    else
                    {
                        if (question.Input.Type == "Checkbox" && answer.Value == "on")
                        {
                            answer.Value = "Yes";
                        }
                    }
                    page.Answers.Add(answer);
                }
                
            }
//            
//            foreach (var answer in request.Answers)
//            {
//                var questionIdDb = page.Questions.SingleOrDefault(q => q.QuestionId == answer.QuestionId);
//
//                
//
//                
//            }

            if (validationPassed)
            {
                page.Complete = true;

                MarkSequenceAsCompleteIfAllPagesComplete(sequence, workflow);
                var workflowJson = JsonConvert.SerializeObject(workflow);

                await _connection.ExecuteAsync("UPDATE UserWorkflows SET Workflow = @Workflow WHERE UserId = @UserId",
                    new {request.UserId, Workflow = workflowJson});
                return new UpdatePageResult {Page = page, ValidationPassed = validationPassed};
            }
            else
            {
                return new UpdatePageResult {Page = page, ValidationPassed = validationPassed, ValidationErrors = validationErrors};
            }
            
            
        }

        private static void MarkSequenceAsCompleteIfAllPagesComplete(Sequence sequence, List<Sequence> workflow)
        {
            sequence.Complete = sequence.Sections.SelectMany(s => s.Pages).All(p => p.Complete);

            if (!sequence.Complete) return;

            var nextSequences = sequence.NextSequences;
            foreach (var nextSequence in nextSequences)
            {
                if (nextSequence.Condition != null)
                {
                    var answers = sequence.Sections.SelectMany(s => s.Pages).SelectMany(p => p.Answers).ToList();
                    if (answers.Any(a =>
                        a.QuestionId == nextSequence.Condition.QuestionId &&
                        a.Value == nextSequence.Condition.MustEqual))
                    {
                        workflow.Single(w => w.SequenceId == nextSequence.NextSequenceId).Active = true;
                    }
                }
                else
                {
                    workflow.Single(w => w.SequenceId == nextSequence.NextSequenceId).Active = true;
                }
            }
        }
    }
}