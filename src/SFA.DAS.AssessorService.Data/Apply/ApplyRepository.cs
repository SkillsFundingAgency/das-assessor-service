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

        public async Task<Guid> CreateApplication(CreateApplicationRequest applicationRequest)
        {
            string applicationStatus = applicationRequest.ApplicationStatus;
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return await connection.QuerySingleAsync<Guid>(
                    @"INSERT INTO Applications (ApplicationId, OrganisationId,ApplicationStatus, CreatedAt, CreatedBy)
                                        OUTPUT INSERTED.[Id] 
                                        VALUES (@QnaApplicationId, @OrganisationId,@applicationStatus, GETUTCDATE(), @userId)",
                    new { applicationRequest.QnaApplicationId, applicationRequest.OrganisationId, applicationStatus, applicationRequest.UserId });
            }
        }

        public async Task SubmitApplicationSequence(Domain.Entities.Application application)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
              var result =  await connection.ExecuteAsync(@"UPDATE Applications
                                                SET  ApplicationStatus = @ApplicationStatus, ApplyData = @ApplyData
                                                WHERE  (Applications.Id = @Id)",
                  new { application.ApplicationStatus, application.ApplyData, application.Id});
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
                            org.EndPointAssessorName as OrganisationName,
                            ap1.Id As Id,
                            JSON_VALUE(ap1.ApplyData, '$.Sequences[0].SequenceNo') As SequenceNo,
                            JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].SectionNo') SectionNo,
                            JSON_VALUE(ap1.ApplyData, '$.Apply.LatestInitSubmissionDate') As SubmittedDate,
                            JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionCount') As SubmissionCount,
                            CASE WHEN (JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Status') = @sequenceStatusFeedbackAdded) THEN @sequenceStatusFeedbackAdded
                            WHEN (JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionCount') > 1 AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].Status') = @financialStatusSubmitted) THEN @sequenceStatusResubmitted
                            WHEN (JSON_VALUE(ap1.ApplyData, '$.Apply.InitSubmissionCount') > 1 AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].RequestedFeedbackAnswered') = 'true')THEN @sequenceStatusResubmitted
                            ELSE JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].Status')
                            END As CurrentStatus
                            FROM Applications ap1
                            inner join Organisations org ON ap1.OrganisationId = org.Id
                            WHERE JSON_VALUE(ap1.ApplyData, '$.Sequences[0].SequenceNo') = '1' AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].SectionNo') = '3' 
                                AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].IsActive') = 'true'
                                AND ap1.ApplicationStatus = @applicationStatusInProgress
                                AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Status') = @sequenceStatusSubmitted
                                AND JSON_VALUE(ap1.ApplyData, '$.Sequences[0].Sections[2].Status') IN (@financialStatusSubmitted, @financialStatusInProgress)",
                        new
                        {
                            applicationStatusInProgress = ApplicationStatus.InProgress,
                            sequenceStatusSubmitted = ApplicationSequenceStatus.Submitted,
                            sequenceStatusFeedbackAdded = ApplicationSequenceStatus.FeedbackAdded,
                            sequenceStatusResubmitted = ApplicationSequenceStatus.Resubmitted,
                            financialStatusSubmitted = ApplicationSectionStatus.Submitted,
                            financialStatusInProgress = ApplicationSectionStatus.InProgress
                        })).ToList();
            }
        }

        public Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            throw new NotImplementedException();
        }

    }
}
