using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Validation
{
    public class ValidationHandler : IRequestHandler<ValidationRequest, bool>
    {
        private readonly IValidationService _validationService;

        public ValidationHandler(IValidationService validationService)
        {
            _validationService = validationService;
        }

        public async Task<bool> Handle(ValidationRequest request, CancellationToken cancellationToken)
        {
            switch (request.ValidationType.ToLower())
            {
                case "email":
                    return _validationService.CheckEmailIsValid(request.ValidationString);
                case "notempty":
                    return _validationService.IsNotEmpty(request.ValidationString);
                case "ukprn":
                    return _validationService.UkprnIsValid(request.ValidationString);
            }

            throw new Exception("Type not recognised");
        }
    }

   

}
