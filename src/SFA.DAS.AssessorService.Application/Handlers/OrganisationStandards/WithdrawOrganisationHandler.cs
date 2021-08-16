using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards
{
    public class WithdrawOrganisationHandler : IRequestHandler<WithdrawOrganisationRequest>
    {
        private readonly IEpaOrganisationValidator _validator;
        private readonly IOrganisationStandardRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawOrganisationHandler(IEpaOrganisationValidator validator, IOrganisationStandardRepository repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(WithdrawOrganisationRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validator.ValidatorWithdrawOrganisationRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");

                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }
                throw new Exception();
            }

            try
            {
                _unitOfWork.Begin();

                await _repository.WithdrawalOrganisation(request.EndPointAssessorOrganisationId, request.WithdrawalDate);

                _unitOfWork.Commit();

                return Unit.Value;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
