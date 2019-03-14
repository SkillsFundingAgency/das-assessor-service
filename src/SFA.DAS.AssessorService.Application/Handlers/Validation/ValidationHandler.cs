using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;

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
            switch (request.Type.ToLower())
            {
                case "email":
                    return _validationService.CheckEmailIsValid(request.Value);
                case "notempty":
                    return _validationService.IsNotEmpty(request.Value);
                case "ukprn":
                    return _validationService.UkprnIsValid(request.Value);
                case "uln":
                    return _validationService.UlnIsValid(request.Value);
                case "minimumlength":
                    if (!int.TryParse(request.MatchCriteria, out int validationMatchValue))
                        throw new Exception($"Validation Match Value [{request.MatchCriteria}] cannot be mapped to an minimum length integer");
                    else
                        return _validationService.IsMinimumLengthOrMore(request.Value, validationMatchValue);
                case "maximumlength":
                    if (!int.TryParse(request.MatchCriteria, out int validationMatchVal))
                        throw new Exception($"Validation Match Value [{request.MatchCriteria}] cannot be mapped to an maximum length integer");
                    else
                        return _validationService.IsMaximumLengthOrLess(request.Value, validationMatchVal);
                case "validdate":
                    return _validationService.DateIsValid(request.Value);
                case "dateistodayorinfuture":
                    return _validationService.DateIsTodayOrInFuture(request.Value);
                case "dateistodayorinpast":
                    return _validationService.DateIsTodayOrInPast(request.Value);
                case "organisationid":
                    return _validationService.OrganisationIdIsValid(request.Value);
                case "companynumber":
                    return _validationService.CompanyNumberIsValid(request.Value);
                case "charitynumber":
                    return _validationService.CharityNumberIsValid(request.Value);
            }

            throw new Exception("Type not recognised");
        }
    }

   

}
