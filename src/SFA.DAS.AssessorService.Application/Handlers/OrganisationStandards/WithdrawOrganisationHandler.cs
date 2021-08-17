using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
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
        private readonly IOrganisationStandardRepository _orgStandardRepository;
        private readonly IApplyRepository _applyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawOrganisationHandler(IEpaOrganisationValidator validator, IOrganisationStandardRepository orgStandardRepository, IApplyRepository applyRepository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _orgStandardRepository = orgStandardRepository;
            _applyRepository = applyRepository;
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
                throw new BadRequestException();
            }

            try
            {
                _unitOfWork.Begin();

                await _orgStandardRepository.WithdrawalOrganisation(request.EndPointAssessorOrganisationId, request.WithdrawalDate);

                await _applyRepository.DeclineAllApplicationsForOrgansiation(request.ApplicationId, request.EndPointAssessorOrganisationId, request.UpdatedBy);

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
