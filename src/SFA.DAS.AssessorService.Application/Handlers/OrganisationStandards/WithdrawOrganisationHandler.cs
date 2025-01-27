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
    public class WithdrawOrganisationHandler : IRequestHandler<WithdrawOrganisationRequest, Unit>
    {
        private readonly IEpaOrganisationValidator _validator;
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        private readonly IApplyRepository _applyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawOrganisationHandler(IEpaOrganisationValidator validator, IOrganisationStandardRepository organisationStandardRepository, IApplyRepository applyRepository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _organisationStandardRepository = organisationStandardRepository;
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
                
                throw new ValidationException(message);
            }

            try
            {
                _unitOfWork.Begin();

                await _organisationStandardRepository.WithdrawOrganisation(request.EndPointAssessorOrganisationId, request.WithdrawalDate);

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
