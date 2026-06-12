using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApplicationApiClient : ApiClientBase, IApplicationApiClient
    {
        public ApplicationApiClient(IAssessorApiClientFactory clientFactory, ILogger<ApplicationApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<List<ApplicationResponse>> GetOrganisationApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/organisation-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve organsisation applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetStandardApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/standard-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve standard applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve withdrawal applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetOrganisationWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/organisation-withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve organisation withdrawal applications");
            }
        }

        public async Task<List<ApplicationResponse>> GetStandardWithdrawalApplications(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{userId}/standard-withdrawal-applications"))
            {
                return await RequestAndDeserialiseAsync<List<ApplicationResponse>>(request, $"Could not retrieve standard withdrawal applications");
            }
        }

        public async Task<ApplicationResponse> GetApplication(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{id}/application"))
            {
                return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve applications");
            }
        }

        public async Task<ApplicationResponse> GetApplicationForUser(Guid id, Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/user/{userId}/application/{id}"))
            {
                return await RequestAndDeserialiseAsync<ApplicationResponse>(request, $"Could not retrieve application {id} for user {userId}");
            }
        }

        public async Task<DateTime?> GetLatestWithdrawalDateForStandard(Guid organisationId, int? standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/applications/{organisationId}/application/latest-withdrawal-date/{standardCode}"))
            {
                return await RequestAndDeserialiseAsync<DateTime?>(request, $"Could not retrieve latest withdrawal date of standard {standardCode} for Organisation {organisationId}");
            }
        }

        public async Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/createApplication"))
            {
                return await PostPutRequestWithResponseAsync<CreateApplicationRequest, Guid>(request, createApplicationRequest);
            }
        }

        public async Task DeleteApplications(DeleteApplicationsRequest deleteApplicationsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/deleteApplications"))
            {
                await PostPutRequestAsync<DeleteApplicationsRequest>(request, deleteApplicationsRequest);
            }
        }

        public async Task<bool> SubmitApplicationSequence(SubmitApplicationSequenceRequest submitApplicationRequest)
        {

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/submitApplicationSequence"))
            {
                return await PostPutRequestWithResponseAsync<SubmitApplicationSequenceRequest, bool>(request, submitApplicationRequest);
            }
        }

        public async Task<bool> UpdateStandardData(Guid Id, int standardCode, string referenceNumber, string standardName, List<string> versions, string standardApplicationType = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/updateStandardData"))
            {
                return await PostPutRequestWithResponseAsync<UpdateStandardDataRequest, bool>(request, new UpdateStandardDataRequest
                {
                    Id = Id,
                    StandardCode = standardCode,
                    ReferenceNumber = referenceNumber,
                    StandardName = standardName,
                    Versions = versions,
                    StandardApplicationType = standardApplicationType
                });
            }
        }

        public async Task<bool> ResetApplicationToStage1(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/applications/resetApplicationToStage1"))
            {
                return await PostPutRequestWithResponseAsync<ResetApplicationToStage1Request, bool>(request, new ResetApplicationToStage1Request
                {
                    Id = id
                });
            }
        }

        #region Application

        public async Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Review/ApplicationReviewStatusCounts"))
            {
                return await RequestAndDeserialiseAsync<ApplicationReviewStatusCounts>(request, "Count not retrieve application review status counts");
            }
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetOrganisationApplications(OrganisationApplicationsRequest organisationApplicationsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Review/OrganisationApplications"))
            {
                return await PostPutRequestWithResponseAsync<OrganisationApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(request, organisationApplicationsRequest);
            }
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetStandardApplications(StandardApplicationsRequest standardApplicationsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Review/StandardApplications"))
            {
                return await PostPutRequestWithResponseAsync<StandardApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(request, standardApplicationsRequest);
            }
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetWithdrawalApplications(WithdrawalApplicationsRequest withdrawalApplicationsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Review/WithdrawalApplications"))
            {
                return await PostPutRequestWithResponseAsync<WithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(request, withdrawalApplicationsRequest);
            }
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/StartReview"))
            {
                await PostPutRequestAsync(request, new { reviewer });
            }
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/Evaluate"))
            {
                await PostPutRequestAsync(request, new { isSectionComplete, evaluatedBy });
            }
        }

        public async Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Return"))
            {
                await PostPutRequestAsync(request, new { returnType, returnedBy });
            }
        }

        #endregion

        #region Financial

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Financial/OpenApplications"))
            {
                return await RequestAndDeserialiseAsync<List<FinancialApplicationSummaryItem>>(request, "Count not retrieve open financial applications");
            }
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Financial/FeedbackAddedApplications"))
            {
                return await RequestAndDeserialiseAsync<List<FinancialApplicationSummaryItem>>(request, "Count not retrieve feedback added financial applications");
            }
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Financial/ClosedApplications"))
            {
                return await RequestAndDeserialiseAsync<List<FinancialApplicationSummaryItem>>(request, "Count not retrieve closed financial applications");
            }
        }

        public async Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Financial/{applicationId}/StartReview"))
            {
                await PostPutRequestAsync(request, new { reviewer });
            }
        }

        public async Task ReturnFinancialReview(Guid applicationId, Domain.Entities.FinancialGrade grade)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/Financial/{applicationId}/Return"))
            {
                await PostPutRequestAsync(request, grade);
            }
        }

        #endregion

        #region Feedback

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/AddFeedback"))
            {
                await PostPutRequestAsync(request, feedback);
            }
        }

        public async Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/DeleteFeedback"))
            {
                await PostPutRequestAsync(request, feedbackId);
            }
        }

        #endregion

        #region Answer Injection Service

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/ao/assessment-organisations/update-financials"))
            {
                await PostPutRequestAsync(request, updateFinancialsRequest);
            }
        }
        
        #endregion
    }
}
