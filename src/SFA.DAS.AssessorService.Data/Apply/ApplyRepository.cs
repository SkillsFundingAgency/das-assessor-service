using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Apply
{
    public class ApplyRepository : Repository, IApplyRepository
    {        
        private readonly ILogger<ApplyRepository> _logger;

        public ApplyRepository(IUnitOfWork unitOfWork, ILogger<ApplyRepository> logger)
            : base(unitOfWork)
        {
            _logger = logger;

            SqlMapper.AddTypeHandler(typeof(ApplyData), new ApplyDataHandler());
            SqlMapper.AddTypeHandler(typeof(FinancialGrade), new FinancialGradeHandler());
            SqlMapper.AddTypeHandler(typeof(FinancialEvidence), new FinancialEvidenceHandler());
        }

        public async Task<Domain.Entities.Apply> GetApply(Guid applicationId)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<Domain.Entities.Apply>(
                @"SELECT * FROM Apply WHERE Id = @applicationId", 
                param: new { applicationId },
                transaction: _unitOfWork.Transaction);
        }

        public async Task<ApplySummary> GetApplication(Guid applicationId, Guid? userId)
        {
            var query = @"SELECT 
                            a.Id, a.ApplicationId, a.OrganisationId, a.ApplicationStatus, a.ReviewStatus, 
                            a.ApplyData, a.FinancialReviewStatus, a.FinancialGrade, 
                            a.StandardCode, a.CreatedBy, a.UpdatedBy, a.DeletedBy, 
                            o.EndPointAssessorName, c1.DisplayName [CreatedByName] , c1.Email [CreatedByEmail]
                          FROM Contacts c
                            INNER JOIN Apply a ON a.OrganisationId = c.OrganisationId
                            INNER JOIN Organisations o ON a.OrganisationId = o.Id
                            INNER JOIN Contacts c1 ON c1.Id = a.CreatedBy
                          WHERE 
                            a.Id = @applicationId
                            AND (c.Id = @userId OR @userId IS NULL)
                          GROUP BY 
	                        a.Id, a.ApplicationId, a.OrganisationId, a.ApplicationStatus, a.ReviewStatus, 
	                        a.ApplyData, a.FinancialReviewStatus, a.FinancialGrade, 
	                        a.StandardCode, a.CreatedAt, a.CreatedBy, a.UpdatedAt, a.UpdatedBy, a.DeletedAt, a.DeletedBy, 
	                        o.EndPointAssessorName, c1.DisplayName, c1.Email";

            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<ApplySummary>(
                sql: query,
                param: new { applicationId, userId },
                transaction: _unitOfWork.Transaction);
        }

        public async Task<List<ApplySummary>> GetApplications(Guid userId, int[] sequenceNos)
        {
            var query = @"SELECT 
                            a.Id, a.ApplicationId, a.OrganisationId, a.ApplicationStatus, a.ReviewStatus, 
                            a.ApplyData, a.FinancialReviewStatus, a.FinancialGrade, 
                            a.StandardCode, a.CreatedBy, a.UpdatedBy, a.DeletedBy, 
                            o.EndPointAssessorName, c1.DisplayName [CreatedByName] , c1.Email [CreatedByEmail] 
                         FROM Contacts c
                            INNER JOIN Apply a ON a.OrganisationId = c.OrganisationId
                            INNER JOIN Organisations o ON a.OrganisationId = o.Id
                            INNER JOIN Contacts c1 ON c1.Id = a.CreatedBy
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, NotRequired BIT) sequence
                         WHERE c.Id = @userId
                            AND sequence.SequenceNo IN @sequenceNos AND sequence.NotRequired = 0
                         GROUP BY 
                            a.Id, a.ApplicationId, a.OrganisationId, a.ApplicationStatus, a.ReviewStatus, 
                            a.ApplyData, a.FinancialReviewStatus, a.FinancialGrade, 
                            a.StandardCode, a.CreatedAt, a.CreatedBy, a.UpdatedAt, a.UpdatedBy, a.DeletedAt, a.DeletedBy, 
                            o.EndPointAssessorName, c1.DisplayName, c1.Email";

            return (await _unitOfWork.Connection.QueryAsync<ApplySummary>(
                sql: query,
                param: new { userId, sequenceNos },
                transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task<List<ApplySummary>> GetOrganisationApplications(Guid userId)
        {
            return await GetApplications(userId, new int[] { ApplyConst.ORGANISATION_SEQUENCE_NO });
        }

        public async Task<List<ApplySummary>> GetStandardApplications(Guid userId)
        {
            return await GetApplications(userId, new int[] { ApplyConst.STANDARD_SEQUENCE_NO });
        }

        public async Task<List<ApplySummary>> GetWithdrawalApplications(Guid userId)
        {
            return await GetApplications(userId, new int[] { ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
        }

        public async Task<List<ApplySummary>> GetOrganisationWithdrawalApplications(Guid userId)
        {
            return await GetApplications(userId, new int[] { ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO });
        }

        public async Task<List<ApplySummary>> GetStandardWithdrawalApplications(Guid userId)
        {
            return await GetApplications(userId, new int[] { ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
        }

        public async Task<Guid> CreateApplication(Domain.Entities.Apply apply)
        {
            return await _unitOfWork.Connection.QuerySingleAsync<Guid>(
                @"INSERT INTO Apply (ApplicationId, OrganisationId ,ApplicationStatus, ApplyData, StandardCode, ReviewStatus, FinancialReviewStatus, CreatedBy, CreatedAt)
                  OUTPUT INSERTED.[Id] 
                  VALUES (@ApplicationId, @OrganisationId, @ApplicationStatus, @ApplyData, @StandardCode, @ReviewStatus, @FinancialReviewStatus, @CreatedBy, GETUTCDATE())",
                param: new { apply.ApplicationId, apply.OrganisationId, apply.ApplicationStatus, apply.ApplyData, apply.StandardCode, apply.ReviewStatus, apply.FinancialReviewStatus, apply.CreatedBy },
                transaction: _unitOfWork.Transaction);
        }

        public async Task<bool> CanSubmitApplication(Guid applicationId)
        {
            // Prevent submission if Organisation Sequence is required and another user has submitted theirs
            var otherAppsInProgress = await _unitOfWork.Connection.QueryAsync<Domain.Entities.Apply>(
                $@"SELECT a.*
                    FROM Apply a
                    INNER JOIN Organisations o ON o.Id = a.OrganisationId
                    INNER JOIN Contacts con ON a.OrganisationId = con.OrganisationID
                    CROSS APPLY OPENJSON(a.ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, NotRequired BIT, Status VARCHAR(20)) sequence
                    WHERE a.OrganisationId = (SELECT OrganisationId FROM Apply WHERE Id = @applicationId)
                    AND a.CreatedBy <> (SELECT CreatedBy FROM Apply WHERE Id = @applicationId)
                    AND a.ApplicationStatus NOT IN (@applicationStatusApproved, @applicationStatusApprovedDeclined)
                    AND sequence.NotRequired = 0 AND sequence.SequenceNo = {ApplyConst.ORGANISATION_SEQUENCE_NO}
                    AND sequence.Status IN (@applicationSequenceStatusSubmitted, @applicationSequenceStatusResubmitted)",
                param: new
                {
                    applicationId,
                    applicationStatusApproved = ApplicationStatus.Approved,
                    applicationStatusApprovedDeclined = ApplicationStatus.Declined,
                    applicationSequenceStatusSubmitted = ApplicationSequenceStatus.Submitted,
                    applicationSequenceStatusResubmitted = ApplicationSequenceStatus.Resubmitted
                },
                transaction: _unitOfWork.Transaction);

            return !otherAppsInProgress.Any();
        }

        public async Task SubmitApplicationSequence(Domain.Entities.Apply apply)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"UPDATE Apply
                  SET  ApplicationStatus = @ApplicationStatus, ApplyData = @ApplyData, StandardCode = @StandardCode, ReviewStatus = @ReviewStatus, FinancialReviewStatus = @FinancialReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                  WHERE  (Apply.Id = @Id)",
                param: new { apply.ApplicationStatus, apply.ApplyData, apply.StandardCode, apply.ReviewStatus, apply.FinancialReviewStatus, apply.Id, apply.UpdatedBy},
                transaction: _unitOfWork.Transaction);
        }

        public async Task<bool> UpdateStandardData(Guid id, int standardCode, string referenceNumber, string standardName, List<string> versions, string applicationType)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;

            if(application != null && applyData != null)
            {
                application.StandardCode = standardCode;
                application.StandardReference = referenceNumber;
                application.ApplicationType = applicationType;

                if (applyData.Apply == null)
                {
                    applyData.Apply = new ApplyTypes.Apply();
                }

                applyData.Apply.StandardCode = standardCode;
                applyData.Apply.StandardReference = referenceNumber;
                applyData.Apply.StandardName = standardName;
                applyData.Apply.Versions = versions;

                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE Apply
                      SET  ApplyData = @ApplyData, StandardCode = @StandardCode, StandardReference = @standardReference, ApplicationType = @applicationType
                      WHERE  Id = @Id",
                    param: new { application.Id, application.ApplyData, application.StandardCode, application.StandardReference, application.ApplicationType },
                    transaction: _unitOfWork.Transaction);
                
                return true;
            }

            return false;
        }

        public async Task<bool> ResetApplicatonToStage1(Guid id, Guid userId)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO);
            var section = sequence?.Sections.SingleOrDefault(sec => sec.SectionNo == ApplyConst.STANDARD_DETAILS_SECTION_NO);

            if (application != null && sequence != null && section != null)
            {
                application.ApplicationStatus = ApplicationStatus.InProgress;
                application.ReviewStatus = ApplicationReviewStatus.Approved;
                application.StandardCode = null;
                application.UpdatedAt = DateTime.UtcNow;
                application.UpdatedBy = userId.ToString();

                section.Status = ApplicationSectionStatus.Draft;
                section.ReviewStartDate = null;
                section.ReviewedBy = null;
                section.EvaluatedDate = null;
                section.EvaluatedBy = null;

                sequence.Status = ApplicationSequenceStatus.Draft;
                sequence.ApprovedDate = null;
                sequence.ApprovedBy = null;

                if (applyData.Apply == null)
                {
                    applyData.Apply = new ApplyTypes.Apply();
                }

                applyData.Apply.StandardCode = null;
                applyData.Apply.StandardReference = null;
                applyData.Apply.StandardName = null;

                applyData.Apply.ResetStandardSubmissions();

                await _unitOfWork.Connection.ExecuteAsync(
                    "UPDATE " +
                    "   Apply " + 
                    "SET " +
                    "   ApplyData = @ApplyData, " + 
                    "   ApplicationStatus = @ApplicationStatus, " +
                    "   ReviewStatus = @ReviewStatus, " +
                    "   StandardCode = @StandardCode, " +
                    "   UpdatedAt = @UpdatedAt, " + 
                    "   UpdatedBy = @UpdatedBy " +
                    "WHERE " +
                    "   Id = @Id",
                    param: new { 
                        application.Id, 
                        application.ApplyData, 
                        application.ApplicationStatus, 
                        application.ReviewStatus, 
                        application.StandardCode, 
                        application.UpdatedAt, 
                        application.UpdatedBy 
                    },
                    transaction: _unitOfWork.Transaction);

                return true;
            }

            return false;
        }

        public async Task StartFinancialReview(Guid id, string reviewer)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == ApplyConst.FINANCIAL_SEQUENCE_NO);
            var section = sequence?.Sections.SingleOrDefault(sec => sec.SectionNo == ApplyConst.FINANCIAL_DETAILS_SECTION_NO);

            if (application != null && section != null && sequence?.IsActive == true && application.FinancialReviewStatus == FinancialReviewStatus.New)
            {
                application.FinancialReviewStatus = FinancialReviewStatus.InProgress;
                application.UpdatedBy = reviewer;
                application.UpdatedAt = DateTime.UtcNow;

                section.Status = ApplicationSectionStatus.InProgress;
                section.ReviewedBy = reviewer;
                section.ReviewStartDate = DateTime.UtcNow;

                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE Apply
                      SET  ApplyData = @ApplyData, FinancialReviewStatus = @FinancialReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                      WHERE Apply.Id = @Id",
                    param:  new { application.Id, application.ApplyData, application.FinancialReviewStatus, application.UpdatedBy },
                    transaction: _unitOfWork.Transaction);
            }
        }

        public async Task ReturnFinancialReview(Guid id, FinancialGrade financialGrade)
        {
            if (financialGrade != null)
            {
                var application = await GetApply(id);
                var applyData = application?.ApplyData;
                var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == ApplyConst.FINANCIAL_SEQUENCE_NO);
                var section = sequence?.Sections.SingleOrDefault(sec => sec.SectionNo == ApplyConst.FINANCIAL_DETAILS_SECTION_NO);

                if (application != null && section != null && sequence?.IsActive == true && application.FinancialReviewStatus == FinancialReviewStatus.InProgress)
                {
                    var financialReviewStatus = (financialGrade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate) ? FinancialReviewStatus.Rejected : FinancialReviewStatus.Graded;

                    application.FinancialReviewStatus = financialReviewStatus;
                    application.FinancialGrade = financialGrade;
                    application.UpdatedBy = financialGrade.GradedBy;
                    application.UpdatedAt = DateTime.UtcNow;

                    section.Status = ApplicationSectionStatus.Graded;
                    section.ReviewedBy = financialGrade.GradedBy;
                    section.ReviewStartDate = DateTime.UtcNow;

                    await _unitOfWork.Connection.ExecuteAsync(
                        @"UPDATE Apply
                          SET  ApplyData = @ApplyData, FinancialGrade = @FinancialGrade, FinancialReviewStatus = @FinancialReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE()  
                          WHERE Apply.Id = @Id",
                        param: new { application.Id, application.ApplyData, application.FinancialGrade, application.FinancialReviewStatus, application.UpdatedBy },
                        transaction: _unitOfWork.Transaction);
                }
            }
            else
            {
                _logger.LogError("FinancialGrade is null therefore failed to update Apply table.");
            }
        }

        public async Task StartApplicationSectionReview(Guid id, int sequenceNo, int sectionNo, string reviewer)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == sequenceNo);
            var section = sequence?.Sections.SingleOrDefault(sec => sec.SectionNo == sectionNo);

            if (application != null && section != null && sequence?.IsActive == true && section.Status != ApplicationSectionStatus.Evaluated)
            {
                application.ReviewStatus = ApplicationReviewStatus.InProgress;
                application.UpdatedBy = reviewer;
                application.UpdatedAt = DateTime.UtcNow;

                section.Status = ApplicationSectionStatus.InProgress;
                section.ReviewedBy = reviewer;
                section.ReviewStartDate = DateTime.UtcNow;

                await _unitOfWork.Connection.ExecuteAsync(@"UPDATE Apply
                    SET  ApplyData = @ApplyData, ReviewStatus = @ReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                    WHERE Apply.Id = @Id",
                    param: new { application.Id, application.ApplyData, application.ReviewStatus, application.UpdatedBy },
                    transaction: _unitOfWork.Transaction);
            }
        }

        public async Task EvaluateApplicationSection(Guid id, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == sequenceNo);
            var section = sequence?.Sections.SingleOrDefault(sec => sec.SectionNo == sectionNo);

            if (application != null && section != null && sequence?.IsActive == true)
            {
                application.UpdatedBy = evaluatedBy;
                application.UpdatedAt = DateTime.UtcNow;

                if (isSectionComplete)
                {
                    section.Status = ApplicationSectionStatus.Evaluated;
                    section.EvaluatedDate = DateTime.UtcNow;
                    section.EvaluatedBy = evaluatedBy;

                    if(section.ReviewedBy != section.EvaluatedBy)
                    {
                        // Note: If it's a different person who has evaluated from the person who started the review then update the review information!
                        section.ReviewStartDate = section.EvaluatedDate;
                        section.ReviewedBy = section.EvaluatedBy;
                    }
                }
                else if (sequence.SequenceNo == ApplyConst.FINANCIAL_SEQUENCE_NO && section.SectionNo == ApplyConst.FINANCIAL_DETAILS_SECTION_NO)
                {
                    section.Status = ApplicationSectionStatus.Graded;
                    section.EvaluatedDate = null;
                    section.EvaluatedBy = null;

                    if(application.FinancialGrade != null)
                    {
                        section.ReviewStartDate = application.FinancialGrade.GradedDateTime;
                        section.ReviewedBy = application.FinancialGrade.GradedBy;
                    }
                }
                else
                {
                    section.Status = ApplicationSectionStatus.InProgress;
                    section.EvaluatedDate = null;
                    section.EvaluatedBy = null;
                }

                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE Apply
                      SET  ApplyData = @ApplyData, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                      WHERE  (Apply.Id = @Id)",
                    param: new { application.Id, application.ApplyData, application.UpdatedBy },
                    transaction: _unitOfWork.Transaction);
            }
        }

        public async Task ReturnApplicationSequence(Guid id, int sequenceNo, string sequenceStatus, string returnedBy)
        {
            var application = await GetApply(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == sequenceNo);
            var nextSequence = applyData?.Sequences.Where(seq => seq.SequenceNo > sequenceNo && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (application != null && applyData != null && sequence !=null)
            { 
                application.UpdatedBy = returnedBy;
                application.UpdatedAt = DateTime.UtcNow;
                sequence.Status = sequenceStatus;
                sequence.ApprovedBy = returnedBy;
                sequence.ApprovedDate = DateTime.UtcNow;

                switch (sequenceStatus)
                {
                    case ApplicationSequenceStatus.FeedbackAdded:
                        application.ReviewStatus = ApplicationReviewStatus.HasFeedback;
                        application.ApplicationStatus = ApplicationStatus.FeedbackAdded;
                        if (sequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO)
                        {
                            applyData.Apply.InitSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.OrganisationWithdrawalSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        else if(sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardWithdrawalSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        break;
                    case ApplicationSequenceStatus.Declined:
                        application.ReviewStatus = ApplicationReviewStatus.Declined;
                        application.ApplicationStatus = ApplicationStatus.Declined;
                        if (sequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO)
                        {
                            applyData.Apply.InitSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.OrganisationWithdrawalSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardWithdrawalSubmissionClosedDate = DateTime.UtcNow;
                        }
                        break;
                    case ApplicationSequenceStatus.Approved:
                        application.ReviewStatus = ApplicationReviewStatus.Approved;
                        if (sequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO)
                        {
                            applyData.Apply.InitSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.OrganisationWithdrawalSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO)
                        {
                            applyData.Apply.StandardWithdrawalSubmissionClosedDate = DateTime.UtcNow;
                        }

                        if (nextSequence != null)
                        {
                            sequence.IsActive = false;
                            nextSequence.IsActive = true;
                            application.ApplicationStatus = ApplicationStatus.InProgress;
                        }
                        else
                        {
                            application.ApplicationStatus = ApplicationStatus.Approved;

                            // Delete any related applications if this one was an initial application
                            // (i.e all sequences are required, section 1 & 2 are required, hence not on EPAO Register)
                            var sequenceOneSections = applyData.Sequences.Where(seq => seq.SequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO).SelectMany(seq => seq.Sections);
                            var initialSections = sequenceOneSections.Where(sec => sec.SectionNo == ApplyConst.ORGANISATION_SEQUENCE_NO || sec.SectionNo == ApplyConst.STANDARD_SEQUENCE_NO);

                            bool initialSectionsRequired = initialSections.All(sec => !sec.NotRequired);
                            bool allSequencesRequired = applyData.Sequences.All(seq => !seq.NotRequired);

                            if (allSequencesRequired && initialSectionsRequired)
                            {
                                await RejectAllRelatedApplications(application.Id, application.UpdatedBy);
                            }
                        }
                        break;
                }

                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE Apply
                      SET  ApplicationStatus = @ApplicationStatus, ReviewStatus = @ReviewStatus, ApplyData = @ApplyData, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                      WHERE  (Apply.Id = @Id)",
                    param: new { application.Id, application.ApplicationStatus, application.ReviewStatus, application.ApplyData, application.UpdatedBy },
                    transaction: _unitOfWork.Transaction);
            }
        }

        private async Task RejectAllRelatedApplications(Guid applicationId, string deletedBy)
        {
            var inProgressRelatedApplications = await _unitOfWork.Connection.QueryAsync<Domain.Entities.Apply>(
                @"SELECT * FROM Apply a
                    WHERE a.OrganisationId = (SELECT OrganisationId FROM Apply WHERE Id = @applicationId)
                    AND a.Id <> @applicationId
                    AND a.ApplicationStatus NOT IN (@approvedStatus, @rejectedStatus)",
                param: new { applicationId, approvedStatus = ApplicationStatus.Approved, rejectedStatus = ApplicationStatus.Declined },
                transaction: _unitOfWork.Transaction);

            foreach (var application in inProgressRelatedApplications)
            {
                application.ApplicationStatus = ApplicationStatus.Declined;
                application.ReviewStatus = ApplicationReviewStatus.Deleted;
                application.DeletedBy = deletedBy;

                foreach (var sequence in application.ApplyData?.Sequences)
                {
                    sequence.IsActive = false;
                    sequence.Status = ApplicationSequenceStatus.Declined;
                }

                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE Apply
                        SET  ApplicationStatus = @ApplicationStatus, ReviewStatus = @ReviewStatus, ApplyData = @ApplyData, DeletedBy = @UpdatedBy, DeletedAt = GETUTCDATE() 
                        WHERE  (Apply.Id = @Id)",
                    param: new { application.Id, application.ApplicationStatus, application.ReviewStatus, application.ApplyData, application.DeletedBy },
                    transaction: _unitOfWork.Transaction);
            }
        }

        public async Task<int> GetNextAppReferenceSequence()
        {
            return (await _unitOfWork.Connection.QueryAsync<int>(@"SELECT NEXT VALUE FOR AppRefSequence")).FirstOrDefault();
        }

        public async Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts()
        {
            var @params = new DynamicParameters();          
            @params.Add("includedNewApplicationSequenceStatus", GetApplicationSequenceStatus(ApplicationReviewStatus.New));
            @params.Add("includedInProgressApplicationSequenceStatus", GetApplicationSequenceStatus(ApplicationReviewStatus.InProgress));
            @params.Add("includedHasFeedbackApplicationSequenceStatus", GetApplicationSequenceStatus(ApplicationReviewStatus.HasFeedback));
            @params.Add("includedApprovedApplicationSequenceStatus", GetApplicationSequenceStatus(ApplicationReviewStatus.Approved));
            @params.Add("excludedApplicationStatus", string.Join("|", new List<string> { ApplicationStatus.Declined }));
            @params.Add("excludedReviewStatus", string.Join("|", new List<string> { ApplicationReviewStatus.Deleted }));
            @params.Add("includedNewReviewStatus", ApplicationReviewStatus.New);
            @params.Add("includedInProgressReviewStatus", ApplicationReviewStatus.InProgress);
            @params.Add("includedHasFeedbackReviewStatus", ApplicationReviewStatus.HasFeedback);
            @params.Add("includedApprovedReviewStatus", ApplicationReviewStatus.Approved);

            var reviewStatusCountResults = (await _unitOfWork.Connection.QueryMultipleAsync(
                "Apply_Get_ReviewStatusCounts",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var organisationReviewStatusCounts = reviewStatusCountResults.Read<ReviewStatusCount>().ToList();
            var standardReviewStatusCounts = reviewStatusCountResults.Read<ReviewStatusCount>().ToList();
            var withdrawalReviewStatusCounts = reviewStatusCountResults.Read<ReviewStatusCount>().ToList();

            var applicationReviewStatusCounts = new ApplicationReviewStatusCounts
            {
                OrganisationApplicationsNew = organisationReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.New)?.Total ?? 0,
                OrganisationApplicationsInProgress = organisationReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.InProgress)?.Total ?? 0,
                OrganisationApplicationsHasFeedback = organisationReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.HasFeedback)?.Total ?? 0,
                OrganisationApplicationsApproved = organisationReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.Approved)?.Total ?? 0,
                StandardApplicationsNew = standardReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.New)?.Total ?? 0,
                StandardApplicationsInProgress = standardReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.InProgress)?.Total ?? 0,
                StandardApplicationsHasFeedback = standardReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.HasFeedback)?.Total ?? 0,
                StandardApplicationsApproved = standardReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.Approved)?.Total ?? 0,
                WithdrawalApplicationsNew = withdrawalReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.New)?.Total ?? 0,
                WithdrawalApplicationsInProgress = withdrawalReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.InProgress)?.Total ?? 0,
                WithdrawalApplicationsHasFeedback = withdrawalReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.HasFeedback)?.Total ?? 0,
                WithdrawalApplicationsApproved = withdrawalReviewStatusCounts?.FirstOrDefault(p => p.ReviewStatus == ApplicationReviewStatus.Approved)?.Total ?? 0,
            };

            return applicationReviewStatusCounts;
        }

        public async Task<ApplicationsResult> GetOrganisationApplications(string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex)
        {
            return await GetApplications(new[] { ApplyConst.ORGANISATION_SEQUENCE_NO }, null, reviewStatus, sortColumn, sortAscending, pageSize, pageIndex);
        }

        public async Task<ApplicationsResult> GetStandardApplications(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex)
        {
            return await GetApplications(new[] { ApplyConst.STANDARD_SEQUENCE_NO }, organisationId, reviewStatus, sortColumn, sortAscending, pageSize, pageIndex);
        }

        public async Task<ApplicationsResult> GetWithdrawalApplications(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex)
        {
            return await GetApplications(new[] { ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO, ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO }, organisationId, reviewStatus, sortColumn, sortAscending, pageSize, pageIndex);
        }

        private async Task<ApplicationsResult> GetApplications(int[] sequenceNos, string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex)
        {
            var @params = new DynamicParameters();
            @params.Add("sequenceNos", string.Join("|", sequenceNos));
            @params.Add("organisationId", organisationId);
            @params.Add("includedApplicationSequenceStatus", GetApplicationSequenceStatus(reviewStatus));
            @params.Add("excludedApplicationStatus", string.Join("|", new List<string> { ApplicationStatus.Declined }));
            @params.Add("excludedReviewStatus", string.Join("|", new List<string> { ApplicationReviewStatus.Deleted }));
            @params.Add("includedReviewStatus", string.Join("|", reviewStatus));
            @params.Add("sortColumn", sortColumn);
            @params.Add("sortAscending", sortAscending);
            @params.Add("pageSize", pageSize);
            @params.Add("pageIndex", pageIndex);
            @params.Add("totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _unitOfWork.Connection.QueryAsync<ApplicationSummaryItem>(
                "Apply_List_Applications",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            var result = new ApplicationsResult
            {
                PageOfResults = results?.ToList() ?? new List<ApplicationSummaryItem>(),
                TotalCount = @params.Get<int?>("totalCount") ?? 0
            };

            return result;
        }

        private string GetApplicationSequenceStatus(string reviewStatus)
        {
            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    return string.Join("|", new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted });
                case ApplicationReviewStatus.InProgress:
                    return string.Join("|", new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted });
                case ApplicationReviewStatus.HasFeedback:
                    return string.Join("|", new List<string> { ApplicationSequenceStatus.FeedbackAdded, ApplicationSequenceStatus.Resubmitted });
                case ApplicationReviewStatus.Approved:
                    return string.Join("|", new List<string> { ApplicationSequenceStatus.Approved });
            }

            return string.Empty;
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            return (await _unitOfWork.Connection.QueryAsync<FinancialApplicationSummaryItem>(
                $@"SELECT
                    ap1.Id AS ApplicationId,
                    sequence.SequenceNo AS SequenceNo,
                    section.SectionNo AS SectionNo, 
                    org.EndPointAssessorName AS OrganisationName, 
                    apply.SubmittedDate AS SubmittedDate,
                    apply.SubmissionCount AS SubmissionCount,
                    ap1.ApplicationStatus AS ApplicationStatus,
                    ap1.ReviewStatus AS ReviewStatus,
                    ap1.FinancialReviewStatus AS FinancialStatus
                FROM Apply ap1
                INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                    CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Sections NVARCHAR(MAX) AS JSON) sequence
	                CROSS APPLY OPENJSON(sequence.Sections) WITH (SectionNo INT) section
	                CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionsCount') apply
                WHERE sequence.SequenceNo = {ApplyConst.FINANCIAL_SEQUENCE_NO} AND section.SectionNo = {ApplyConst.FINANCIAL_DETAILS_SECTION_NO} AND sequence.IsActive = 1
                    AND ap1.FinancialReviewStatus IN (@financialReviewStatusNew, @financialReviewStatusInProgress)
                    AND ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted)
                    AND ap1.DeletedAt IS NULL",
                param: new
                {
                    financialReviewStatusNew = FinancialReviewStatus.New,
                    financialReviewStatusInProgress = FinancialReviewStatus.InProgress,
                    applicationStatusSubmitted = ApplicationStatus.Submitted,
                    applicationStatusResubmitted = ApplicationStatus.Resubmitted,
                },
                transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return (await _unitOfWork.Connection.QueryAsync<FinancialApplicationSummaryItem>(
                $@"SELECT
                    ap1.Id AS ApplicationId,
                    sequence.SequenceNo AS SequenceNo,
                    section.SectionNo AS SectionNo, 
                    org.EndPointAssessorName AS OrganisationName,
                    apply.SubmittedDate AS SubmittedDate,
                    apply.SubmissionCount AS SubmissionCount,
                    ISNULL(section.FeedbackDate, JSON_VALUE(ap1.FinancialGrade, '$.GradedDateTime')) As FeedbackAddedDate,
                    ap1.ApplicationStatus AS ApplicationStatus,
                    ap1.ReviewStatus AS ReviewStatus,
                    ap1.FinancialReviewStatus AS FinancialStatus,
	                ap1.FinancialGrade As Grade
                FROM Apply ap1
                INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                    CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Sections NVARCHAR(MAX) AS JSON) sequence
                    CROSS APPLY OPENJSON(sequence.Sections) WITH (SectionNo INT, FeedbackDate VARCHAR(30) '$.Feedback.FeedbackDate') section
                    CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionsCount') apply
                WHERE sequence.SequenceNo = {ApplyConst.FINANCIAL_SEQUENCE_NO} AND section.SectionNo = {ApplyConst.FINANCIAL_DETAILS_SECTION_NO} AND sequence.IsActive = 1
                    AND ap1.FinancialReviewStatus = @financialReviewStatusRejected
                    AND ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted, @applicationStatusFeedbackAdded)
                    AND ap1.DeletedAt IS NULL",
                param: new
                {
                    financialReviewStatusRejected = FinancialReviewStatus.Rejected,
                    applicationStatusSubmitted = ApplicationStatus.Submitted,
                    applicationStatusResubmitted = ApplicationStatus.Resubmitted,
                    applicationStatusFeedbackAdded = ApplicationStatus.FeedbackAdded,
                },
                transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            return (await _unitOfWork.Connection.QueryAsync<FinancialApplicationSummaryItem>(
                $@"SELECT
                    ap1.Id AS ApplicationId,
                    sequence.SequenceNo AS SequenceNo,
                    section.SectionNo AS SectionNo, 
                    org.EndPointAssessorName AS OrganisationName,
                    apply.ClosedDate AS ClosedDate,
                    apply.SubmissionCount AS SubmissionCount,
                    ap1.ApplicationStatus AS ApplicationStatus,
                    ap1.ReviewStatus AS ReviewStatus,
                    ap1.FinancialReviewStatus AS FinancialStatus,
	                ap1.FinancialGrade As Grade
                FROM Apply ap1
                INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                    CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, Sections NVARCHAR(MAX) AS JSON) sequence
                    CROSS APPLY OPENJSON(sequence.Sections) WITH (SectionNo INT, NotRequired BIT) section
                    CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (ClosedDate VARCHAR(30) '$.InitSubmissionClosedDate', SubmissionCount INT '$.InitSubmissionsCount') apply
                WHERE sequence.SequenceNo = {ApplyConst.FINANCIAL_SEQUENCE_NO} AND section.SectionNo = {ApplyConst.FINANCIAL_DETAILS_SECTION_NO} AND section.NotRequired = 0
                    AND ap1.FinancialReviewStatus IN (@financialReviewStatusGraded, @financialReviewStatusApproved) -- NOTE: Not showing Exempt
                    AND ap1.DeletedAt IS NULL",
                param: new
                {
                    financialReviewStatusGraded = FinancialReviewStatus.Graded,
                    financialReviewStatusApproved = FinancialReviewStatus.Approved                            
                },
                transaction: _unitOfWork.Transaction)).ToList();   
        }
    }
}
