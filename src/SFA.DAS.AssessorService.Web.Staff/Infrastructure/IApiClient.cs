using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public interface IApiClient
    {
        Task ApproveCertificates(CertificatePostApprovalViewModel certificatePostApprovalViewModel);
        Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page);
        Task<PaginatedList<StaffBatchSearchResult>> BatchSearch(int batchNumber, int page);
        Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request);
        Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request);
        Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request);
        Task<object> CreateScheduleRun(ScheduleRun schedule);
        Task<object> DeleteScheduleRun(Guid scheduleRunId);
        Task GatherAndCollateStandards();
        Task<IList<ScheduleRun>> GetAllScheduledRun(int scheduleType);
        Task<Certificate> GetCertificate(Guid certificateId);
        Task<List<CertificateResponse>> GetCertificates();
        Task<List<CertificateSummaryResponse>> GetCertificatesToBeApproved();
        Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure);
        Task<List<DeliveryArea>> GetDeliveryAreas();
        Task<AssessmentOrganisationContact> GetEpaCntact(string contactId);
        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);
        Task<List<ContactResponse>> GetEpaOrganisationContacts(string organisationId);
        Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId);
        Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task<ScheduleRun> GetNextScheduleToRunNow();
        Task<List<Option>> GetOptions(int stdCode);
        Task<Organisation> GetOrganisation(Guid id);
        Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId);
        Task<List<OrganisationType>> GetOrganisationTypes();
        Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId);
        Task<ReportDetails> GetReportDetailsFromId(Guid reportId);
        Task<IEnumerable<StaffReport>> GetReportList();
        Task<ReportType> GetReportTypeFromId(Guid reportId);
        Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId);
        Task<string> ImportOrganisations();
        Task<Certificate> PostReprintRequest(StaffCertificateDuplicateRequest staffCertificateDuplicateRequest);
        Task<object> RunNowScheduledRun(int scheduleType);
        Task<StaffSearchResult> Search(string searchString, int page);
        Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString);
        Task<List<StandardSummary>> SearchStandards(string searchString);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest);
        Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request);
        Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request);
        Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request);
        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);
    }
}