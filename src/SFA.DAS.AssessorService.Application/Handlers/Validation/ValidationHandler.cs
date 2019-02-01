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
                case "uln":
                    return _validationService.UlnIsValid(request.ValidationString);
                case "minimumlength":
                    if (!int.TryParse(request.ValidationMatchValue, out int validationMatchValue))
                        throw new Exception($"Validation Match Value [{request.ValidationMatchValue}] cannot be mapped to an minimum length integer");
                    else
                        return _validationService.IsMinimumLengthOrMore(request.ValidationString, validationMatchValue);
                case "maximumlength":
                    if (!int.TryParse(request.ValidationMatchValue, out int validationMatchVal))
                        throw new Exception($"Validation Match Value [{request.ValidationMatchValue}] cannot be mapped to an maximum length integer");
                    else
                        return _validationService.IsMaximumLengthOrLess(request.ValidationString, validationMatchVal);
                case "validdate":
                    return _validationService.DateIsValid(request.ValidationString);
                case "dateistodayorinfuture":
                    return _validationService.DateIsTodayOrInFuture(request.ValidationString);
                case "dateistodayorinpast":
                    return _validationService.DateIsTodayOrInPast(request.ValidationString);
                case "organisationid":
                    return _validationService.OrganisationIdIsValid(request.ValidationString);
                case "companynumber":
                    return _validationService.CompanyNumberIsValid(request.ValidationString);
                case "charitynumber":
                    return _validationService.CharityNumberIsValid(request.ValidationString);
            }

            throw new Exception("Type not recognised");
        }
    }

   

}
