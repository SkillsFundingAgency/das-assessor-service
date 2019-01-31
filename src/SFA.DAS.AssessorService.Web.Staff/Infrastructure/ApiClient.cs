using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Models;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        private async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) ;
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            return await Get<List<CertificateResponse>>("/api/v1/certificates?statusses=Submitted");
        }

        public async Task<List<CertificateSummaryResponse>> GetCertificatesToBeApproved()
        {
            return await Get<List<CertificateSummaryResponse>>("/api/v1/certificates/approvals");
        }

        public async Task<StaffSearchResult> Search(string searchString, int page)
        {
            return await Get<StaffSearchResult>($"/api/v1/staffsearch?searchQuery={searchString}&page={page}");
        }

        public async Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString)
        {
            return await Get<List<AssessmentOrganisationSummary>>(
                $"/api/ao/assessment-organisations/search/{searchString}");
        }

        public async Task<string> ImportOrganisations()
        {
            var uri = "/api/ao/assessment-organisations/";
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.PatchAsync(new Uri(uri, UriKind.Relative), null))
            {
                var res = await response.Content.ReadAsAsync<AssessmentOrgsImportResponse>();
                return res.Status;
            }
        }


        public async Task<List<OrganisationType>> GetOrganisationTypes()
        {
            return await Get<List<OrganisationType>>($"/api/ao/organisation-types");
        }
        

        public async Task<EpaOrganisation> GetEpaOrganisation(string organisationId)
        {
            return await Get<EpaOrganisation>($"api/ao/assessment-organisations/{organisationId}");
        }
        
        public async Task<AssessmentOrganisationContact> GetEpaCntact(string contactId)
        {
            return await Get<AssessmentOrganisationContact>($"api/ao/assessment-organisations/contacts/{contactId}");
        }
        
        public async Task<List<ContactResponse>> GetEpaOrganisationContacts(string organisationId)
        {
            return await Get<List<ContactResponse>>($"api/v1/contacts/get-all/{organisationId}");
        }

        public async Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId)
        {
            return await Get<List<OrganisationStandardSummary>>($"/api/ao/assessment-organisations/{organisationId}/standards");
        }

        public async Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request)
        {
            var result =
                await Post<CreateEpaOrganisationRequest, EpaOrganisationResponse>("api/ao/assessment-organisations",
                    request);
            return result.Details;
        }

        public async Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request)
        {
            var result =
                await Post<CreateEpaOrganisationStandardRequest, EpaoStandardResponse>("api/ao/assessment-organisations/standards",
                    request);
            return result.Details;
        }

        public async Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            var result =
                await Put<UpdateEpaOrganisationStandardRequest, EpaoStandardResponse>("api/ao/assessment-organisations/standards",
                    request);
            return result.Details;
        }

        public async Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request)
        {
            var result = await Put<UpdateEpaOrganisationRequest, EpaOrganisationResponse>("api/ao/assessment-organisations", request);
            return result.Details;
        }

        public async Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request)
        {
            var result = await Post<CreateEpaOrganisationContactRequest, EpaOrganisationContactResponse>("api/ao/assessment-organisations/contacts", request);
            return result.Details;
        }

        public async Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request)
        {
            var result = await Put<UpdateEpaOrganisationContactRequest, EpaOrganisationContactResponse>("api/ao/assessment-organisations/contacts", request);
            return result.Details;
        }
        
        public async Task<PaginatedList<StaffBatchSearchResult>> BatchSearch(int batchNumber, int page)
        {
            return await Get<PaginatedList<StaffBatchSearchResult>>(
                $"/api/v1/staffsearch/batch?batchNumber={batchNumber}&page={page}");
        }

        public async Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page)
        {
            return await Get<PaginatedList<StaffBatchLogResult>>($"/api/v1/staffsearch/batchlog?page={page}");
        }

        public async Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs)
        {
            return await Get<LearnerDetail>($"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}&alllogs={allLogs}");
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            return await Get<Certificate>($"api/v1/certificates/{certificateId}");
        }

        public async Task<Organisation> GetOrganisation(Guid id)
        {
            return await Get<Organisation>($"/api/v1/organisations/organisation/{id}");
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            return await Get<List<Option>>($"api/v1/certificates/options/?stdCode={stdCode}");
        }

        public async Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest)
        {
            return await Put<UpdateCertificateRequest, Certificate>("api/v1/certificates/update", certificateRequest);
        }

        public async Task<ScheduleRun> GetNextScheduleToRunNow()
        {
            return await Get<ScheduleRun>($"api/v1/schedule?scheduleType=1");
        }

        public async Task<ScheduleRun> GetNextScheduledRun(int scheduleType)
        {
            return await Get<ScheduleRun>($"api/v1/schedule/next?scheduleType={scheduleType}");
        }

        public async Task<object> RunNowScheduledRun(int scheduleType)
        {
            return await Post<object, object>($"api/v1/schedule/runnow?scheduleType={scheduleType}", default(object));
        }

        public async Task<object> CreateScheduleRun(ScheduleRun schedule)
        {
            return await Put<ScheduleRun, object>($"api/v1/schedule/create", schedule);
        }

        public async Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId)
        {
            return await Get<ScheduleRun>($"api/v1/schedule?scheduleRunId={scheduleRunId}");
        }

        public async Task<IList<ScheduleRun>> GetAllScheduledRun(int scheduleType)
        {
            return await Get<IList<ScheduleRun>>($"api/v1/schedule/all?scheduleType={scheduleType}");
        }

        public async Task<object> DeleteScheduleRun(Guid scheduleRunId)
        {
            return await Delete<object>($"api/v1/schedule?scheduleRunId={scheduleRunId}");
        }

        public async Task<Certificate> PostReprintRequest(
            StaffCertificateDuplicateRequest staffCertificateDuplicateRequest)
        {
            return await Post<StaffCertificateDuplicateRequest, Certificate>("api/v1/staffcertificatereprint",
                staffCertificateDuplicateRequest);
                }

        public async Task<List<StandardSummary>> SearchStandards(string searchString)
        {
            return await Get<List<StandardSummary>>($"/api/ao/assessment-organisations/standards/search/{searchString}");
        }

        public async Task ApproveCertificates(CertificatePostApprovalViewModel certificatePostApprovalViewModel)
        {
            await Post<CertificatePostApprovalViewModel>("api/v1/certificates/approvals",
                certificatePostApprovalViewModel);
        }

        public async Task<List<DeliveryArea>> GetDeliveryAreas()
        {
            return await Get<List<DeliveryArea>>("/api/ao/delivery-areas");
        }

        public async Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId)
        {
            return await Get<OrganisationStandard>($"/api/ao/assessment-organisations/organisation-standard/{organisationStandardId}");
        }

        #region Reports
        public async Task<IEnumerable<StaffReport>> GetReportList()
        {
            return await Get<IEnumerable<StaffReport>>($"api/v1/staffreports");
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId)
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"api/v1/staffreports/{reportId}");
        }

        public async Task<ReportType> GetReportTypeFromId(Guid reportId)
        {
            return await Get<ReportType>($"api/v1/staffreports/{reportId}/report-type");
        }

        public async Task<ReportDetails> GetReportDetailsFromId(Guid reportId)
        {
            return await Get<ReportDetails>($"api/v1/staffreports/{reportId}/report-details");
        }

        public async Task GatherAndCollateStandards()
        {
             await Post($"api/ao/update-standards", new GatherStandardsRequest());
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure)
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"api/v1/staffreports/report-content/{storedProcedure}");
        }
        #endregion
    }
}