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
            bool result;

            switch (request.Type.ToLower())
            {
                case "phonenumber":
                    result = _validationService.CheckPhoneNumberIsValue(request.Value);
                    break;
                case "email":
                    result = _validationService.CheckEmailIsValid(request.Value);
                    break;
                case "websitelink":
                    result = _validationService.CheckWebsiteLinkIsValid(request.Value);
                    break;
                case "notempty":
                    result = _validationService.IsNotEmpty(request.Value);
                    break;
                case "ukprn":
                    result = _validationService.UkprnIsValid(request.Value);
                    break;
                case "uln":
                    result = _validationService.UlnIsValid(request.Value);
                    break;
                case "minimumlength":
                    if (!int.TryParse(request.MatchCriteria, out int validationMatchValue))
                    {
                        throw new Exception($"Validation Match Value [{request.MatchCriteria}] cannot be mapped to an minimum length integer");
                    }
                    else
                    {
                        result = _validationService.IsMinimumLengthOrMore(request.Value, validationMatchValue);
                        break;
                    }
                case "maximumlength":
                    if (!int.TryParse(request.MatchCriteria, out int validationMatchVal))
                    {
                        throw new Exception($"Validation Match Value [{request.MatchCriteria}] cannot be mapped to an maximum length integer");
                    }
                    else
                    {
                        result = _validationService.IsMaximumLengthOrLess(request.Value, validationMatchVal);
                        break;
                    }
                case "validdate":
                    result = _validationService.DateIsValid(request.Value);
                    break;
                case "dateistodayorinfuture":
                    result = _validationService.DateIsTodayOrInFuture(request.Value);
                    break;
                case "dateistodayorinpast":
                    result = _validationService.DateIsTodayOrInPast(request.Value);
                    break;
                case "organisationid":
                    result = _validationService.OrganisationIdIsValid(request.Value);
                    break;
                case "companynumber":
                    result = _validationService.CompanyNumberIsValid(request.Value);
                    break;
                case "charitynumber":
                    result = _validationService.CharityNumberIsValid(request.Value);
                    break;
                default:
                    throw new Exception("Type not recognised");
            }

            return await Task.FromResult(result);
        }
    }

   

}
