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
            ImportLearnerDetailResponse response = new ImportLearnerDetailResponse
            {
                LearnerDetailResults = new List<ImportLearnerDetailResult>()
            };

            foreach (var importLearnerDetail in request.ImportLearnerDetails)
            {
                _logger.LogDebug($"Handling Import Learner Detail Request Uln:{importLearnerDetail.Uln}, StdCode:{importLearnerDetail.StdCode}");

                response.LearnerDetailResults.Add(
                    CheckMissingMandatoryFields(importLearnerDetail) ?? new ImportLearnerDetailResult
                    {
                        Uln = importLearnerDetail.Uln,
                        StdCode = importLearnerDetail.StdCode,
                        Outcome = await HandleRequest(importLearnerDetail, cancellationToken)
                    });
            }

            return response;
        }

        private async Task<string> HandleRequest(ImportLearnerDetail importLearnerDetail, CancellationToken cancellationToken)
        {
            if (importLearnerDetail.Uln == 9999999999 || importLearnerDetail.Uln == 1000000000)
            {
                return "IgnoreUlnDummyValue";
            }

            var learner = await _ilrRepository.Get(importLearnerDetail.Uln.Value, importLearnerDetail.StdCode.Value);
            if (learner != null)
            {
                return await HandleExistingLearnerRequest(learner, importLearnerDetail, cancellationToken);
            }
            
            return await CreateIlrRecord(importLearnerDetail);
        }

        private async Task<string> HandleExistingLearnerRequest(Ilr learner, ImportLearnerDetail importLearnerDetail, CancellationToken cancellationToken)
        {
            // the source represents an academic year which should be compared as a number
            var requestSource = int.Parse(importLearnerDetail.Source);
            var learnerSource = int.Parse(learner.Source);

            if (requestSource < learnerSource)
            {
                return "IgnoreSourcePriorToCurrentSource";
            }
            else if (requestSource > learnerSource)
            {
                return await UpdateIlrRecord(importLearnerDetail, false);
            }
            
            return await HandleSameSourceRequest(learner, importLearnerDetail, cancellationToken);
        }

        private async Task<string> HandleSameSourceRequest(Ilr learner, ImportLearnerDetail importLearnerDetail, CancellationToken cancellationToken)
        {
            if (importLearnerDetail.Ukprn == learner.UkPrn)
            {
                if (importLearnerDetail.LearnActEndDate != null && importLearnerDetail.LearnStartDate == importLearnerDetail.LearnActEndDate)
                    return "IgnoreLearnActEndDateSameAsLearnStartDate";

                if (importLearnerDetail.PlannedEndDate == learner.PlannedEndDate && importLearnerDetail.LearnStartDate == learner.LearnStartDate)
                    return await UpdateIlrRecord(importLearnerDetail, true, learner);

                if (importLearnerDetail.LearnStartDate > learner.LearnStartDate)
                    return await UpdateIlrRecord(importLearnerDetail, false);
            }
            else
            {
                var certificate = await _certificateRepository.GetCertificate(importLearnerDetail.Uln.Value, importLearnerDetail.StdCode.Value);

                if (certificate != null)
                    return "IgnoreUkprnChangedButCertficateAlreadyExists";

                if (importLearnerDetail.FundingModel == 99 && learner.FundingModel != 99)
                    return "IgnoreFundingModelChangedTo99WhenPrevioulsyNot99";

                if (importLearnerDetail.LearnActEndDate == null && learner.LearnActEndDate != null)
                    return await UpdateIlrRecord(importLearnerDetail, false);

                if (importLearnerDetail.LearnActEndDate != null && importLearnerDetail.PlannedEndDate > learner.PlannedEndDate)
                    return await UpdateIlrRecord(importLearnerDetail, false);

                if (importLearnerDetail.LearnStartDate > learner.LearnStartDate)
                    return await UpdateIlrRecord(importLearnerDetail, false);    
            }

            return "IgnoreOutOfDate";
        }

        private async Task<string> CreateIlrRecord(ImportLearnerDetail importLearnerDetail)
        {
            _logger.LogDebug("Handling Import Learner Detail Request - Create Ilr");

            await _ilrRepository.Create(new Ilr
            {
                Source = importLearnerDetail.Source,
                UkPrn = importLearnerDetail.Ukprn.Value,
                Uln = importLearnerDetail.Uln.Value,
                StdCode = importLearnerDetail.StdCode.Value,
                FundingModel = importLearnerDetail.FundingModel,
                GivenNames = importLearnerDetail.GivenNames,
                FamilyName = importLearnerDetail.FamilyName,
                EpaOrgId = importLearnerDetail.EpaOrgId,
                LearnStartDate = importLearnerDetail.LearnStartDate.Value,
                PlannedEndDate = importLearnerDetail.PlannedEndDate,
                CompletionStatus = importLearnerDetail.CompletionStatus,
                LearnRefNumber = importLearnerDetail.LearnRefNumber,
                DelLocPostCode = importLearnerDetail.DelLocPostCode,
                LearnActEndDate = importLearnerDetail.LearnActEndDate,
                WithdrawReason = importLearnerDetail.WithdrawReason,
                Outcome = importLearnerDetail.Outcome,
                AchDate = importLearnerDetail.AchDate,
                OutGrade = importLearnerDetail.OutGrade
            });

            return "CreatedLearnerDetail";
        }

        private async Task<string> UpdateIlrRecord(ImportLearnerDetail importLearnerDetail, bool isUpdate, Ilr currentLearner = null)
        {
            _logger.LogDebug("Handling Import Learner Detail Request - Update Ilr");

            // for an update to certain fields if the request is null then the currrent value will be
            // retained, otherwise the request value will be used
            await _ilrRepository.Update(new Ilr
            {
                Source = importLearnerDetail.Source,
                UkPrn = importLearnerDetail.Ukprn.Value,
                Uln = importLearnerDetail.Uln.Value,
                StdCode = importLearnerDetail.StdCode.Value,
                FundingModel = importLearnerDetail.FundingModel,
                GivenNames = importLearnerDetail.GivenNames,
                FamilyName = importLearnerDetail.FamilyName,
                EpaOrgId = RetainCurrentValueForNullUpdate(currentLearner?.EpaOrgId, importLearnerDetail.EpaOrgId, isUpdate),
                LearnStartDate = importLearnerDetail.LearnStartDate.Value,
                PlannedEndDate = importLearnerDetail.PlannedEndDate,
                CompletionStatus = importLearnerDetail.CompletionStatus,
                LearnRefNumber = importLearnerDetail.LearnRefNumber,
                DelLocPostCode = importLearnerDetail.DelLocPostCode,
                LearnActEndDate = RetainCurrentValueForNullUpdate(currentLearner?.LearnActEndDate, importLearnerDetail.LearnActEndDate, isUpdate),
                WithdrawReason = RetainCurrentValueForNullUpdate(currentLearner?.WithdrawReason, importLearnerDetail.WithdrawReason, isUpdate),
                Outcome = RetainCurrentValueForNullUpdate(currentLearner?.Outcome, importLearnerDetail.Outcome, isUpdate),
                AchDate = RetainCurrentValueForNullUpdate(currentLearner?.AchDate, importLearnerDetail.AchDate, isUpdate),
                OutGrade = RetainCurrentValueForNullUpdate(currentLearner?.OutGrade, importLearnerDetail.OutGrade, isUpdate)
            });

            return $"{(isUpdate ? "Updated" : "Replaced")}LearnerDetail";
        }

        private T RetainCurrentValueForNullUpdate<T>(T currentValue, T newValue, bool isUpdate)
        {
            return isUpdate && newValue == null ? currentValue : newValue;
        }

        private ImportLearnerDetailResult CheckMissingMandatoryFields(ImportLearnerDetail request)
        {
            _logger.LogDebug("Handling Import Learner Detail Request - Checking for missing mandatory fields");

            var result = new ImportLearnerDetailResult
            {
                Outcome = "ErrorMissingMandatoryField",
                Errors = new List<string>()
            };

            AddMissingMandatoryFieldError(result, request.Source, nameof(request.Source));
            AddMissingMandatoryFieldError(result, request.Ukprn, nameof(request.Ukprn));
            AddMissingMandatoryFieldError(result, request.Uln, nameof(request.Uln));
            AddMissingMandatoryFieldError(result, request.StdCode, nameof(request.StdCode));
            AddMissingMandatoryFieldError(result, request.FundingModel, nameof(request.FundingModel));
            AddMissingMandatoryFieldError(result, request.GivenNames, nameof(request.GivenNames));
            AddMissingMandatoryFieldError(result, request.FamilyName, nameof(request.FamilyName));
            AddMissingMandatoryFieldError(result, request.LearnStartDate, nameof(request.LearnStartDate));
            AddMissingMandatoryFieldError(result, request.PlannedEndDate, nameof(request.PlannedEndDate));
            AddMissingMandatoryFieldError(result, request.CompletionStatus, nameof(request.CompletionStatus));
            AddMissingMandatoryFieldError(result, request.LearnRefNumber, nameof(request.LearnRefNumber));
            AddMissingMandatoryFieldError(result, request.DelLocPostCode, nameof(request.DelLocPostCode));

            return result.Errors.Count > 0 ? result : null;
        }

        private void AddMissingMandatoryFieldError<T>(ImportLearnerDetailResult result, T fieldValue, string fieldName)
        {
            if(fieldValue == null)
            {
                result.Errors.Add($"Missing mandatory field {fieldName}.");
            }
        }
    }
}