using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Apply
{
    public class ApplyRepository : IApplyRepository
    {
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<ApplyRepository> _logger;

        public ApplyRepository(IWebConfiguration configuration, ILogger<ApplyRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;

            SqlMapper.AddTypeHandler(typeof(ApplyData), new ApplyDataHandler());
            SqlMapper.AddTypeHandler(typeof(FinancialGrade), new FinancialGradeHandler());
            SqlMapper.AddTypeHandler(typeof(FinancialEvidence), new FinancialEvidenceHandler());
        }

        public async Task<List<Domain.Entities.Apply>> GetUserApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Apply>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Apply a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId AND a.CreatedBy = @userId", new { userId })).ToList();
            }
        }

        public async Task<List<Domain.Entities.Apply>> GetOrganisationApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Apply>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Apply a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId", new { userId })).ToList();
            }
        }

        public async Task<Domain.Entities.Apply> GetApplication(Guid applicationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var application = await connection.QuerySingleOrDefaultAsync<Domain.Entities.Apply>(@"SELECT * FROM Apply WHERE Id = @applicationId", new { applicationId });

                return application;
            }
        }

        public async Task<Guid> CreateApplication(Domain.Entities.Apply apply)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return await connection.QuerySingleAsync<Guid>(
                    @"INSERT INTO Apply (ApplicationId, OrganisationId,ApplicationStatus,ApplyData, ReviewStatus,FinancialReviewStatus, CreatedAt, CreatedBy)
                                        OUTPUT INSERTED.[Id] 
                                        VALUES (@ApplicationId, @OrganisationId,@ApplicationStatus,@ApplyData,@ReviewStatus,@FinancialReviewStatus,GETUTCDATE(), @CreatedBy)",
                    new { apply.ApplicationId, apply.OrganisationId, apply.ApplicationStatus,apply.FinancialReviewStatus,
                        apply.ApplyData, apply.ReviewStatus, apply.CreatedBy });
            }
        }

        public async Task SubmitApplicationSequence(Domain.Entities.Apply apply)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
              var result =  await connection.ExecuteAsync(@"UPDATE Apply
                                                SET  ApplicationStatus = @ApplicationStatus, ApplyData = @ApplyData, ReviewStatus = @ReviewStatus, FinancialReviewStatus = @FinancialReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                                                WHERE  (Apply.Id = @Id)",
                  new { apply.ApplicationStatus, apply.ApplyData, apply.ReviewStatus, apply.FinancialReviewStatus, apply.Id, apply.UpdatedBy});
            }
        }

        public async Task UpdateInitialStandardData(UpdateInitialStandardDataRequest standardRequest)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var result = await connection.ExecuteAsync(@"UPDATE Apply
                                                SET  ApplyData = JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(ApplyData,'$.Apply.StandardReference',@ReferenceNumber),'$.Apply.StandardCode',@StandardCode),'$.Apply.StandardName',@StandardName)
                                                WHERE  Id = @Id",
                    new { standardRequest.StandardCode,standardRequest.ReferenceNumber, standardRequest.StandardName, standardRequest.Id });
            }
        }

        public async Task StartApplicationReview(Guid id, int sequenceNo)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                await connection.ExecuteAsync(@"UPDATE Apply 
                                                SET ReviewStatus = @reviewStatusInProgress
                                                WHERE Id = @id",
                    new { id, sequenceNo, reviewStatusInProgress = ApplicationReviewStatus.InProgress }) ;
            }
        }

        public async Task StartFinancialReview(Guid id)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var financialReviewStatus = FinancialReviewStatus.InProgress;
                var finanicalSectionStatus = ApplicationSectionStatus.InProgress;

                await connection.ExecuteAsync(@"UPDATE Apply 
                                                SET FinancialReviewStatus = @financialReviewStatus
                                                    ApplyData = JSON_MODIFY(ApplyData, '$.Sequences[0].Sections[2].Status', @finanicalSectionStatus)
                                                WHERE Id = @id",
                    new { id, financialReviewStatus, finanicalSectionStatus });
            }
        }

        public async Task UpdateApplicationFinancialGrade(Guid id, FinancialGrade financialGrade)
        {
            if (financialGrade != null)
            {
                using (var connection = new SqlConnection(_configuration.SqlConnectionString))
                {
                    var financialReviewStatus = (financialGrade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate) ? FinancialReviewStatus.Rejected : FinancialReviewStatus.Approved;
                    var finanicalSectionStatus = ApplicationSectionStatus.Graded;

                    await connection.ExecuteAsync(@"UPDATE Apply
                                                    SET FinancialGrade = @financialGrade, FinancialReviewStatus = @financialReviewStatus,
                                                        ApplyData = JSON_MODIFY(ApplyData, '$.Sequences[0].Sections[2].Status', @finanicalSectionStatus)
                                                    WHERE Id = @id",
                        new { id, financialGrade, financialReviewStatus, finanicalSectionStatus });
                }
            }
            else
            {
                _logger.LogError("FinancialGrade is null therefore failed to update Apply table.");
            }
        }

        public async Task UpdateApplicationSectionStatus(Guid id, string sequenceNo, string sectionNo, string status)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var result = await connection.ExecuteAsync(@"UPDATE Apply SET ApplyData = 
                                                JSON_MODIFY(ApplyData, '$.Sequences['+@sequenceNo+'].Sections['+@sectionNo+'].Status',@status) 
                                                WHERE Id = @id",
                    new { sequenceNo, sectionNo,status, id });
            }
        }

        public async Task UpdateApplicationSequenceStatus(Guid id, int sequenceNo, string sequenceStatus, string updatedBy)
        {
            var application = await GetApplication(id);
            var applyData = application?.ApplyData;
            var sequence = applyData?.Sequences.SingleOrDefault(seq => seq.SequenceNo == sequenceNo);
            var nextSequence = applyData?.Sequences.Where(seq => seq.SequenceNo > sequenceNo && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (application != null && applyData != null && sequence !=null)
            { 
                application.UpdatedBy = updatedBy;
                sequence.Status = sequenceStatus;

                switch (sequenceStatus)
                {
                    case ApplicationSequenceStatus.FeedbackAdded:
                        application.ReviewStatus = ApplicationReviewStatus.HasFeedback;
                        application.ApplicationStatus = ApplicationStatus.FeedbackAdded;
                        if (sequenceNo == 1)
                        {
                            applyData.Apply.InitSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == 2)
                        {
                            applyData.Apply.StandardSubmissionFeedbackAddedDate = DateTime.UtcNow;
                        }
                        break;
                    case ApplicationSequenceStatus.Declined:
                        application.ReviewStatus = ApplicationReviewStatus.Declined;
                        application.ApplicationStatus = ApplicationStatus.Declined;
                        if (sequenceNo == 1)
                        {
                            applyData.Apply.InitSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == 2)
                        {
                            applyData.Apply.StandardSubmissionClosedDate = DateTime.UtcNow;
                        }
                        break;
                    case ApplicationSequenceStatus.Approved:
                        application.ReviewStatus = ApplicationReviewStatus.Approved;
                        if (sequenceNo == 1)
                        {
                            applyData.Apply.InitSubmissionClosedDate = DateTime.UtcNow;
                        }
                        else if (sequenceNo == 2)
                        {
                            applyData.Apply.StandardSubmissionClosedDate = DateTime.UtcNow;
                        }

                        if(nextSequence != null)
                        {
                            sequence.IsActive = false;
                            nextSequence.IsActive = true;
                            application.ApplicationStatus = ApplicationStatus.InProgress;
                        }
                        else
                        {
                            application.ApplicationStatus = ApplicationStatus.Approved;

                            // Delete any related applications if this one was an initial application
                            // (i.e all sequences are required, and thus, not on EPAO Register)
                            if (applyData.Sequences.All(seq => !seq.NotRequired) && !application.Organisation.OrganisationData.RoEPAOApproved)
                            {
                                await RejectAllRelatedApplications(application.Id, application.UpdatedBy);
                            }
                        }
                        break;
                }

                using (var connection = new SqlConnection(_configuration.SqlConnectionString))
                {
                    await connection.ExecuteAsync(@"UPDATE Apply
                                                    SET  ApplicationStatus = @ApplicationStatus, ReviewStatus = @ReviewStatus, ApplyData = @ApplyData, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                                                    WHERE  (Apply.Id = @Id)",
                                                    new { application.Id, application.ApplicationStatus, application.ReviewStatus, application.ApplyData, application.UpdatedBy });
                }
            }
        }

        private async Task RejectAllRelatedApplications(Guid applicationId, string deletedBy)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var inProgressRelatedApplications = await connection.QueryAsync<Domain.Entities.Apply>(@"SELECT * FROM Apply a
                                                                                                         WHERE a.ApplyingOrganisationId = (SELECT ApplyingOrganisationId FROM Applications WHERE Applications.Id = @applicationId)
                                                                                                         AND a.Id <> @applicationId
                                                                                                         AND a.ApplicationStatus NOT IN (@approvedStatus, @rejectedStatus)",
                                                                                                         new { applicationId, approvedStatus = ApplicationStatus.Approved, rejectedStatus = ApplicationStatus.Declined });

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

                    await connection.ExecuteAsync(@"UPDATE Apply
                                                    SET  ApplicationStatus = @ApplicationStatus, ReviewStatus = @ReviewStatus, ApplyData = @ApplyData, DeletedBy = @UpdatedBy, DeletedAt = GETUTCDATE() 
                                                    WHERE  (Apply.Id = @Id)",
                                                    new { application.Id, application.ApplicationStatus, application.ReviewStatus, application.ApplyData, application.DeletedBy });
                }
            }
        }

        public async Task<int> GetNextAppReferenceSequence()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<int>(@"SELECT NEXT VALUE FOR AppRefSequence")).FirstOrDefault();

            }
        }

        public async Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<ApplicationSummaryItem>(
                        @"SELECT
                            org.EndPointAssessorName AS OrganisationName,
                            ap1.Id AS ApplicationId,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 1 ELSE 2 END AS SequenceNo,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 'Midpoint' ELSE 'Standard' END AS ApplicationType,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardName') AS StandardName,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardCode') AS StandardCode,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate'),JSON_VALUE(ap1.Applydata,'$.Apply.LatestInitSubmissionDate')) AS SubmittedDate,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionCount')) AS SubmissionCount,
                            ap1.FinancialReviewStatus AS FinancialStatus,
                            JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade,
                            ap1.ApplicationStatus AS CurrentStatus,
                            ap1.ReviewStatus AS ReviewStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                        WHERE ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted)
                        AND @SequenceNo = CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 1 ELSE 2 END",
                        new
                        {
                            SequenceNo = sequenceNo,
                            applicationStatusSubmitted = ApplicationStatus.Submitted,
                            applicationStatusResubmitted = ApplicationStatus.Resubmitted,
                        })).ToList();
            }
        }

        public async Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<ApplicationSummaryItem>(
                        @"SELECT
                            org.EndPointAssessorName AS OrganisationName,
                            ap1.Id AS ApplicationId,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 1 ELSE 2 END AS SequenceNo,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 'Midpoint' ELSE 'Standard' END AS ApplicationType,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardName') AS StandardName,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardCode') AS StandardCode,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate'),JSON_VALUE(ap1.Applydata,'$.Apply.LatestInitSubmissionDate')) AS SubmittedDate,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionCount')) AS SubmissionCount,
                            ap1.FinancialReviewStatus AS FinancialStatus,
                            JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade,
                            ap1.ApplicationStatus AS CurrentStatus,
                            ap1.ReviewStatus AS ReviewStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                        WHERE ap1.ApplicationStatus IN (@applicationStatusFeedbackAdded)",
                        new
                        {
                            applicationStatusFeedbackAdded = ApplicationStatus.FeedbackAdded,
                        })).ToList();
            }
        }

        public async Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<ApplicationSummaryItem>(
                        @"SELECT
                            org.EndPointAssessorName AS OrganisationName,
                            ap1.Id AS ApplicationId,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 1 ELSE 2 END AS SequenceNo,
                            CASE WHEN JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate') IS NULL THEN 'Midpoint' ELSE 'Standard' END AS ApplicationType,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardName') AS StandardName,
                            JSON_VALUE(ap1.Applydata,'$.Apply.StandardCode') AS StandardCode,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.LatestStandardSubmissionDate'),JSON_VALUE(ap1.Applydata,'$.Apply.LatestInitSubmissionDate')) AS SubmittedDate,
                            ISNULL(JSON_VALUE(ap1.Applydata,'$.Apply.StandardSubmissionsCount'),JSON_VALUE(ap1.Applydata,'$.Apply.InitSubmissionCount')) AS SubmissionCount,
                            ap1.FinancialReviewStatus AS FinancialStatus,
                            JSON_VALUE(ap1.FinancialGrade,'$.SelectedGrade') AS FinancialGrade,
                            ap1.ApplicationStatus AS CurrentStatus,
                            ap1.ReviewStatus AS ReviewStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                        WHERE ap1.ApplicationStatus IN (@applicationStatusApproved, @applicationStatusDeclined)",
                        new
                        {
                            applicationStatusApproved = ApplicationStatus.Approved,
                            applicationStatusDeclined = ApplicationStatus.Declined,
                        })).ToList();
            }
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<FinancialApplicationSummaryItem>(
                        @"SELECT
                           org.EndPointAssessorName AS OrganisationName,
                           ap1.Id AS Id,
	                       sequence.SequenceNo AS SequenceNo,
                           section.SectionNo AS SectionNo, 
                           apply.SubmittedDate AS SubmittedDate,
                           apply.SubmissionCount AS SubmissionCount, 
	                       CASE WHEN (ap1.FinancialReviewStatus = @financialReviewStatusInProgress) THEN @applicationStatusInProgress
                                WHEN (ap1.ApplicationStatus = @applicationStatusResubmitted) THEN @applicationStatusResubmitted
                                WHEN (ap1.ApplicationStatus = @applicationStatusSubmitted) THEN @applicationStatusSubmitted
                                ELSE section.Status
                           END As CurrentStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20)) section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND sequence.IsActive = 1
                            AND ap1.FinancialReviewStatus IN (@financialReviewStatusNew, @financialReviewStatusInProgress)
                            AND ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted)",
                        new
                        {
                            financialReviewStatusNew = FinancialReviewStatus.New,
                            financialReviewStatusInProgress = FinancialReviewStatus.InProgress,
                            applicationStatusInProgress = ApplicationStatus.InProgress,
                            applicationStatusSubmitted = ApplicationStatus.Submitted,
                            applicationStatusResubmitted = ApplicationStatus.Resubmitted,
                        })).ToList();
            }
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<FinancialApplicationSummaryItem>(
                        @"SELECT org.EndPointAssessorName AS OrganisationName,
                           ap1.Id AS Id,
	                       sequence.SequenceNo AS SequenceNo,
                           section.SectionNo AS SectionNo, 
	                       ap1.FinancialGrade As Grade,
	                       ISNULL(section.FeedbackDate, JSON_VALUE(ap1.FinancialGrade, '$.GradedDateTime')) As FeedbackAddedDate,
                           apply.SubmittedDate AS SubmittedDate,
                           apply.SubmissionCount AS SubmissionCount,
	                       ap1.FinancialReviewStatus AS CurrentStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20), FeedbackDate VARCHAR(30) '$.Feedback.FeedbackDate') section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND sequence.IsActive = 1
                            AND ap1.FinancialReviewStatus = @financialReviewStatusRejected
                            AND ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted)",
                        new
                        {
                            financialReviewStatusRejected = FinancialReviewStatus.Rejected,
                            applicationStatusSubmitted = ApplicationStatus.Submitted,
                            applicationStatusResubmitted = ApplicationStatus.Resubmitted,
                        })).ToList();
            }
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection
                    .QueryAsync<FinancialApplicationSummaryItem>(
                        @"SELECT org.EndPointAssessorName AS OrganisationName,
                           ap1.Id AS Id,
	                       sequence.SequenceNo AS SequenceNo,
                           section.SectionNo AS SectionNo, 
	                       ap1.FinancialGrade As Grade,
                           apply.ClosedDate AS ClosedDate,
                           apply.SubmissionCount AS SubmissionCount,
	                       CASE WHEN (sequence.Status = @applicationSequenceStatusApproved) THEN @applicationSequenceStatusApproved
                                WHEN (sequence.Status = @applicationSequenceStatusRejected) THEN @applicationSequenceStatusDeclined
                                ELSE ap1.FinancialReviewStatus
	                       END As CurrentStatus
                        FROM Apply ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20), NotRequired BIT) section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (ClosedDate VARCHAR(30) '$.InitSubmissionClosedDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND section.NotRequired = 0
                            AND ap1.FinancialReviewStatus = @financialReviewStatusApproved -- NOTE: Not showing Exempt",
                        new
                        {
                            financialReviewStatusApproved = FinancialReviewStatus.Approved,
                            applicationSequenceStatusApproved = ApplicationSequenceStatus.Approved,
                            applicationSequenceStatusDeclined = ApplicationSequenceStatus.Declined                            
                        })).ToList();
            }
        }

    }
}
