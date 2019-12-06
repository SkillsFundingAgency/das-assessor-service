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
                return "IgnoreUlnDummyValue";

            var learner = await _ilrRepository.Get(request.Uln.Value, request.StdCode.Value);

            if (learner == null)
            {
                return CreateIlrRecord(request);
            }
            else
            {
                if (int.Parse(request.Source) < int.Parse(learner.Source))
                {
                    return "IgnoreSourcePriorToCurrentSource";
                }
                if (int.Parse(request.Source) > int.Parse(learner.Source))
                {
                    return UpdateIlrRecord(request, false);
                }
                else
                {
                    if (request.Ukprn == learner.UkPrn)
                    {
                        if (request.LearnActEndDate != null && request.LearnStartDate == request.LearnActEndDate)
                            return "IgnoreLearnActEndDateSameAsLearnStartDate";

                        if (request.PlannedEndDate == learner.PlannedEndDate && request.LearnStartDate == learner.LearnStartDate)
                            return UpdateIlrRecord(request, true, learner);

                        if (request.LearnStartDate > learner.LearnStartDate)
                            return UpdateIlrRecord(request, false);

                        // if the request.LearnStartDate < current.LearnStartDate or PlannedEndDate
                        return "IgnoreOutOfDate";
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

                        return "IgnoreOutOfDate";
                    }
                }
            }
        }
        private string CreateIlrRecord(ImportLearnerDetailRequest request)
        {
            _logger.LogInformation("Handling Import Learner Detail Request - Create Ilr");

            _ilrRepository.Create(request.Source, request.Ukprn.Value, request.Uln.Value, request.StdCode.Value, request.FundingModel, request.GivenNames, request.FamilyName,
                request.EpaOrgId, request.LearnStartDate, request.PlannedEndDate, request.CompletionStatus, request.LearnRefNumber, request.DelLocPostCode,
                request.LearnActEndDate, request.WithdrawReason, request.Outcome, request.AchDate, request.OutGrade);

            return "CreatedLearnerDetail";
        }

        private string UpdateIlrRecord(ImportLearnerDetailRequest request, bool isUpdate, Ilr currentLearner = null)
        {
            _logger.LogInformation("Handling Import Learner Detail Request - Update Ilr");

            // for an update to certain fields if the request is null then the currrent value will 
            // retained, otherwise the request value will be used
            _ilrRepository.Update(request.Source, request.Ukprn.Value, request.Uln.Value, request.StdCode.Value, request.FundingModel, request.GivenNames, request.FamilyName,
                isUpdate ? request.EpaOrgId ?? currentLearner.EpaOrgId : request.EpaOrgId,
                request.LearnStartDate, request.PlannedEndDate, request.CompletionStatus, request.LearnRefNumber, request.DelLocPostCode,
                isUpdate ? request.LearnActEndDate ?? currentLearner.LearnActEndDate : request.LearnActEndDate, 
                isUpdate ? request.WithdrawReason ?? currentLearner.WithdrawReason : request.WithdrawReason, 
                isUpdate ? request.Outcome ?? currentLearner.Outcome : request.Outcome, 
                isUpdate ? request.AchDate ?? currentLearner.AchDate : request.AchDate, 
                isUpdate ? request.OutGrade ?? currentLearner.OutGrade : request.OutGrade);

            return $"{(isUpdate ? "Updated" : "Replaced")}LearnerDetail";
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