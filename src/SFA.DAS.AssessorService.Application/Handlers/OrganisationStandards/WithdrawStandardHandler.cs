using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards
{
    public class WithdrawStandardHandler : IRequestHandler<WithdrawStandardRequest>
    {
        private readonly IEpaOrganisationValidator _validator;
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        
        public WithdrawStandardHandler(IEpaOrganisationValidator validator, IOrganisationStandardRepository organisationStandardRepository)
        {
            _validator = validator;
            _organisationStandardRepository = organisationStandardRepository;
        }

        public async Task<Unit> Handle(WithdrawStandardRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validator.ValidatorWithdrawStandardRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");

                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }
                
                throw new ValidationException(message);
            }

            await _organisationStandardRepository.WithdrawStandard(request.EndPointAssessorOrganisationId, request.StandardCode, request.WithdrawalDate);

            return Unit.Value;
        }
    }
}
