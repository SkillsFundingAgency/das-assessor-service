using System.Collections.Generic;
using System.Data;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Apply
{
    public class SequenceValidator : AbstractValidator<Sequence>
    {
        private readonly IDbConnection _dbConnection;

        public SequenceValidator(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            
            RuleFor(r => r.SequenceId).NotEmpty().WithMessage("SequenceID must not be empty");
            RuleFor(r => r.Actor).NotEmpty().WithMessage("Actor must not be empty");
            RuleFor(r => r.Title).NotEmpty().WithMessage("Title must not be empty");
            
            RuleFor(r => r.SequenceId)
                .Custom((sequenceId, context) =>
                {
                    var workflowDefinition = _dbConnection.QuerySingle<WorkflowDefinition>("SELECT * FROM WorkflowDefinitions");
                    var sequences = JsonConvert.DeserializeObject<List<Sequence>>(workflowDefinition.Workflow);

                    if (sequences.Exists(s => s.SequenceId == sequenceId))
                    {
                        context.AddFailure(new ValidationFailure("Contact","SequenceId already exists"));
                    }
                });
        }
    }
}