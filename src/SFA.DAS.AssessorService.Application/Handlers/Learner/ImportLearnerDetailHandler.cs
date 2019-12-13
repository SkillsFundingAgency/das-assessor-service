using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Learner
{
    public class ImportLearnerDetailHandler : IRequestHandler<ImportLearnerDetailRequest, ImportLearnerDetailResponse>
    {
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<ImportLearnerDetailHandler> _logger;

        public ImportLearnerDetailHandler(IIlrRepository ilrRepository, ICertificateRepository certificateRepository, ILogger<ImportLearnerDetailHandler> logger)
        {
            _ilrRepository = ilrRepository;
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<ImportLearnerDetailResponse> Handle(ImportLearnerDetailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Import Learner Detail Request");

            return CheckMissingMandatoryFields(request) ?? new ImportLearnerDetailResponse
            {
                Result = await HandleRequest(request, cancellationToken)
            };
        }

        private async Task<string> HandleRequest(ImportLearnerDetailRequest request, CancellationToken cancellationToken)
        {
            if (request.Uln == 9999999999 || request.Uln == 1000000000)
            {
                return "IgnoreUlnDummyValue";
            }

            var learner = await _ilrRepository.Get(request.Uln.Value, request.StdCode.Value);
            if (learner != null)
            {
                return await HandleExistingLearnerRequest(learner, request, cancellationToken);
            }
            
            return CreateIlrRecord(request);
        }

        private async Task<string> HandleExistingLearnerRequest(Ilr learner, ImportLearnerDetailRequest request, CancellationToken cancellationToken)
        {
            // the source represents an academic year which should be compared as a number
            var requestSource = int.Parse(request.Source);
            var learnerSource = int.Parse(learner.Source);

            if (requestSource < learnerSource)
            {
                return "IgnoreSourcePriorToCurrentSource";
            }
            else if (requestSource > learnerSource)
            {
                return UpdateIlrRecord(request, false);
            }
            
            return await HandleSameSourceRequest(learner, request, cancellationToken);
        }

        private async Task<string> HandleSameSourceRequest(Ilr learner, ImportLearnerDetailRequest request, CancellationToken cancellationToken)
        {
            if (request.Ukprn == learner.UkPrn)
            {
                if (request.LearnActEndDate != null && request.LearnStartDate == request.LearnActEndDate)
                    return "IgnoreLearnActEndDateSameAsLearnStartDate";

                if (request.PlannedEndDate == learner.PlannedEndDate && request.LearnStartDate == learner.LearnStartDate)
                    return UpdateIlrRecord(request, true, learner);

                if (request.LearnStartDate > learner.LearnStartDate)
                    return UpdateIlrRecord(request, false);
            }
            else
            {
                var certificate = await _certificateRepository.GetCertificate(request.Uln.Value, request.StdCode.Value);

                if (certificate != null)
                    return "IgnoreUkprnChangedButCertficateAlreadyExists";

                if (request.FundingModel == 99 && learner.FundingModel != 99)
                    return "IgnoreFundingModelChangedTo99WhenPrevioulsyNot99";

                if (request.LearnActEndDate == null && learner.LearnActEndDate != null)
                    return UpdateIlrRecord(request, false);

                if (request.LearnActEndDate != null && request.PlannedEndDate > learner.PlannedEndDate)
                    return UpdateIlrRecord(request, false);

                if (request.LearnStartDate > learner.LearnStartDate)
                    return UpdateIlrRecord(request, false);    
            }

            return "IgnoreOutOfDate";
        }

        private string CreateIlrRecord(ImportLearnerDetailRequest request)
        {
            _logger.LogInformation("Handling Import Learner Detail Request - Create Ilr");

            _ilrRepository.Create(new Ilr
            {
                Source = request.Source,
                UkPrn = request.Ukprn.Value,
                Uln = request.Uln.Value,
                StdCode = request.StdCode.Value,
                FundingModel = request.FundingModel,
                GivenNames = request.GivenNames,
                FamilyName = request.FamilyName,
                EpaOrgId = request.EpaOrgId,
                LearnStartDate = request.LearnStartDate.Value,
                PlannedEndDate = request.PlannedEndDate,
                CompletionStatus = request.CompletionStatus,
                LearnRefNumber = request.LearnRefNumber,
                DelLocPostCode = request.DelLocPostCode,
                LearnActEndDate = request.LearnActEndDate,
                WithdrawReason = request.WithdrawReason,
                Outcome = request.Outcome,
                AchDate = request.AchDate,
                OutGrade = request.OutGrade
            });

            return "CreatedLearnerDetail";
        }

        private string UpdateIlrRecord(ImportLearnerDetailRequest request, bool isUpdate, Ilr currentLearner = null)
        {
            _logger.LogInformation("Handling Import Learner Detail Request - Update Ilr");

            // for an update to certain fields if the request is null then the currrent value will be
            // retained, otherwise the request value will be used
            _ilrRepository.Update(new Ilr
            {
                Source = request.Source,
                UkPrn = request.Ukprn.Value,
                Uln = request.Uln.Value,
                StdCode = request.StdCode.Value,
                FundingModel = request.FundingModel,
                GivenNames = request.GivenNames,
                FamilyName = request.FamilyName,
                EpaOrgId = RetainCurrentValueForNullUpdate(currentLearner?.EpaOrgId, request.EpaOrgId, isUpdate),
                LearnStartDate = request.LearnStartDate.Value,
                PlannedEndDate = request.PlannedEndDate,
                CompletionStatus = request.CompletionStatus,
                LearnRefNumber = request.LearnRefNumber,
                DelLocPostCode = request.DelLocPostCode,
                LearnActEndDate = RetainCurrentValueForNullUpdate(currentLearner?.LearnActEndDate, request.LearnActEndDate, isUpdate),
                WithdrawReason = RetainCurrentValueForNullUpdate(currentLearner?.WithdrawReason, request.WithdrawReason, isUpdate),
                Outcome = RetainCurrentValueForNullUpdate(currentLearner?.Outcome, request.Outcome, isUpdate),
                AchDate = RetainCurrentValueForNullUpdate(currentLearner?.AchDate, request.AchDate, isUpdate),
                OutGrade = RetainCurrentValueForNullUpdate(currentLearner?.OutGrade, request.OutGrade, isUpdate)
            });

            return $"{(isUpdate ? "Updated" : "Replaced")}LearnerDetail";
        }

        private T RetainCurrentValueForNullUpdate<T>(T currentValue, T newValue, bool isUpdate)
        {
            return isUpdate && newValue == null ? currentValue : newValue;
        }

        private ImportLearnerDetailResponse CheckMissingMandatoryFields(ImportLearnerDetailRequest request)
        {
            _logger.LogInformation("Handling Import Learner Detail Request - Checking for missing mandatory fields");

            var response = new ImportLearnerDetailResponse
            {
                Result = "ErrorMissingMandatoryField",
                Errors = new List<string>()
            };

            AddMissingMandatoryFieldError(response, request.Source, nameof(request.Source));
            AddMissingMandatoryFieldError(response, request.Ukprn, nameof(request.Ukprn));
            AddMissingMandatoryFieldError(response, request.Uln, nameof(request.Uln));
            AddMissingMandatoryFieldError(response, request.StdCode, nameof(request.StdCode));
            AddMissingMandatoryFieldError(response, request.FundingModel, nameof(request.FundingModel));
            AddMissingMandatoryFieldError(response, request.GivenNames, nameof(request.GivenNames));
            AddMissingMandatoryFieldError(response, request.FamilyName, nameof(request.FamilyName));
            AddMissingMandatoryFieldError(response, request.LearnStartDate, nameof(request.LearnStartDate));
            AddMissingMandatoryFieldError(response, request.PlannedEndDate, nameof(request.PlannedEndDate));
            AddMissingMandatoryFieldError(response, request.CompletionStatus, nameof(request.CompletionStatus));
            AddMissingMandatoryFieldError(response, request.LearnRefNumber, nameof(request.LearnRefNumber));
            AddMissingMandatoryFieldError(response, request.DelLocPostCode, nameof(request.DelLocPostCode));

            return response.Errors.Count > 0 ? response : null;
        }

        private void AddMissingMandatoryFieldError<T>(ImportLearnerDetailResponse response, T fieldValue, string fieldName)
        {
            if(fieldValue == null)
            {
                response.Errors.Add($"Missing mandatory field {fieldName}.");
            }
        }
    }
}