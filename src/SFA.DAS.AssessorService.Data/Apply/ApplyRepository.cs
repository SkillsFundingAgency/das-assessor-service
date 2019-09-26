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

        public async Task<List<Domain.Entities.Application>> GetUserApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Application>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Applications a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId AND a.CreatedBy = @userId", new { userId })).ToList();
            }
        }

        public async Task<List<Domain.Entities.Application>> GetOrganisationApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Application>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Applications a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId", new { userId })).ToList();
            }
        }

        public async Task<Domain.Entities.Application> GetApplication(Guid applicationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var application = await connection.QuerySingleOrDefaultAsync<Domain.Entities.Application>(@"SELECT * FROM Applications WHERE Id = @applicationId", new { applicationId });

                return application;
            }
        }

        public async Task<Guid> CreateApplication(Domain.Entities.Application application)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return await connection.QuerySingleAsync<Guid>(
                    @"INSERT INTO Applications (ApplicationId, OrganisationId,ApplicationStatus,ApplyData, ReviewStatus,FinancialReviewStatus, CreatedAt, CreatedBy)
                                        OUTPUT INSERTED.[Id] 
                                        VALUES (@ApplicationId, @OrganisationId,@ApplicationStatus,@ApplyData,@ReviewStatus,@FinancialReviewStatus,GETUTCDATE(), @CreatedBy)",
                    new { application.ApplicationId, application.OrganisationId, application.ApplicationStatus,application.FinancialReviewStatus,
                        application.ApplyData, application.ReviewStatus, application.CreatedBy });
            }
        }

        public async Task SubmitApplicationSequence(Domain.Entities.Application application)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
              var result =  await connection.ExecuteAsync(@"UPDATE Applications
                                                SET  ApplicationStatus = @ApplicationStatus, ApplyData = @ApplyData, ReviewStatus = @ReviewStatus, FinancialReviewStatus = @FinancialReviewStatus, UpdatedBy = @UpdatedBy, UpdatedAt = GETUTCDATE() 
                                                WHERE  (Applications.Id = @Id)",
                  new { application.ApplicationStatus, application.ApplyData, application.ReviewStatus, application.FinancialReviewStatus, application.Id, application.UpdatedBy});
            }
        }

        public async Task UpdateInitialStandardData(UpdateInitialStandardDataRequest standardRequest)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var result = await connection.ExecuteAsync(@"UPDATE Applications
                                                SET  ApplyData = JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(ApplyData,'$.Apply.StandardReference',@ReferenceNumber),'$.Apply.StandardCode',@StandardCode),'$.Apply.StandardName',@StandardName)
                                                WHERE  Id = @Id",
                    new { standardRequest.StandardCode,standardRequest.ReferenceNumber, standardRequest.StandardName, standardRequest.Id });
            }
        }

        public async Task UpdateApplicationFinancialGrade(Guid id, FinancialGrade financialGrade)
        {
            if (financialGrade != null)
            {
                using (var connection = new SqlConnection(_configuration.SqlConnectionString))
                {
                    var result = await connection.ExecuteAsync(@"UPDATE Applications
                                                SET FinancialGrade = @financialGrade
                                                WHERE Id = @id",
                        new { id, financialGrade });
                }
            }
            else
            {
                _logger.LogError("FinancialGrade is null therefore failed to update Applications table.");
            }
        }

        public async Task UpdateApplicationSectionStatus(Guid id, string sequenceNo, string sectionNo, string status)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                var result = await connection.ExecuteAsync(@"UPDATE Applications SET ApplyData = 
                                                JSON_MODIFY(ApplyData, '$.Sequences['+@sequenceNo+'].Sections['+@sectionNo+'].Status',@status) 
                                                WHERE Id = @id",
                    new { sequenceNo, sectionNo,status, id });
            }
        }

        public async Task<int> GetNextAppReferenceSequence()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<int>(@"SELECT NEXT VALUE FOR AppRefSequence")).FirstOrDefault();

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
	                       ap1.ApplicationStatus AS CurrentStatus
                        FROM Applications ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20)) section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND sequence.IsActive = 1
                            AND ap1.FinancialReviewStatus = @financialReviewStatusNew -- NOTE: May need to consider Required status. Assumption is that on submit, it will be set to New
                            AND ap1.ApplicationStatus IN (@applicationStatusSubmitted, @applicationStatusResubmitted)",
                        new
                        {
                            financialReviewStatusNew = FinancialReviewStatus.New,
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
	                       ap1.ApplicationStatus AS CurrentStatus
                        FROM Applications ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, IsActive BIT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20), FeedbackDate VARCHAR(30) '$.Feedback.FeedbackDate') section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (SubmittedDate VARCHAR(30) '$.LatestInitSubmissionDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND sequence.IsActive = 1
                            AND ap1.FinancialReviewStatus = @financialReviewStatusRejected -- NOTE: May need to consider if only Graded and Inadequate. Assumption is that if this situation occurs, it will be set to Rejected
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
                                WHEN (sequence.Status = @applicationSequenceStatusRejected) THEN @applicationSequenceStatusRejected
                                ELSE section.Status
	                       END As CurrentStatus
                        FROM Applications ap1
                        INNER JOIN Organisations org ON ap1.OrganisationId = org.Id
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, Status VARCHAR(20)) sequence
                            CROSS APPLY OPENJSON(ApplyData,'$.Sequences[0].Sections') WITH (SectionNo INT, Status VARCHAR(20), NotRequired BIT) section
                            CROSS APPLY OPENJSON(ApplyData,'$.Apply') WITH (ClosedDate VARCHAR(30) '$.InitSubmissionClosedDate', SubmissionCount INT '$.InitSubmissionCount') apply
                        WHERE sequence.SequenceNo = 1 AND section.SectionNo = 3 AND section.NotRequired = 0
                            AND ap1.FinancialReviewStatus = @financialReviewStatusClosed -- NOTE: Not showing Exempt",
                        new
                        {
                            financialReviewStatusClosed = FinancialReviewStatus.Closed,
                            applicationSequenceStatusApproved = ApplicationSequenceStatus.Approved,
                            applicationSequenceStatusRejected = ApplicationSequenceStatus.Rejected                            
                        })).ToList();
            }
        }

    }
}
