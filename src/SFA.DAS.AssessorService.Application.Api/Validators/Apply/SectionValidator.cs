using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Apply
{
    public class SectionValidator : AbstractValidator<Section>
    {
        public SectionValidator(IDbConnection dbConnection)
        {
            RuleFor(r => r.SectionId).NotEmpty().WithMessage("SectionId must not be empty");
            RuleFor(r => r.Title).NotEmpty().WithMessage("Title must not be empty");
        }
    }
}