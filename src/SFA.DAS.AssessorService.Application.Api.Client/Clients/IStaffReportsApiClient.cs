using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStaffReportsApiClient
    {
        Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure);
        Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId);
        Task<ReportDetails> GetReportDetailsFromId(Guid reportId);
        Task<IEnumerable<StaffReport>> GetReportList();
        Task<ReportType> GetReportTypeFromId(Guid reportId);
    }
}