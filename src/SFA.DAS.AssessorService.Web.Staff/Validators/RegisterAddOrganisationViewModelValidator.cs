using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterAddOrganisationViewModelValidator : AbstractValidator<RegisterAddOrganisationViewModel>
    {
        //private readonly IWebConfiguration _configuration;
        private readonly IStringLocalizer<EpaOrganisationValidator> _localizer;
        private readonly IRegisterQueryRepository _repository;

        public RegisterAddOrganisationViewModelValidator(IStringLocalizer<EpaOrganisationValidator> localizer, IRegisterQueryRepository repository)
        {
           // _configuration = configuration;
            _localizer = localizer;
            _repository = repository;      
            var validator = new EpaOrganisationValidator(_repository, _localizer);
           
           // var res = validator.CheckOrganisationName("");
          
            RuleFor(vm => vm.Name).NotEmpty().WithMessage("needs words");
        }
    }
}
