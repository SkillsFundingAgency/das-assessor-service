using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffReportRepository : IStaffReportRepository
    {
        private readonly IAssessorUnitOfWork _assessorUnitOfWork;
        private readonly IDbConnection _connection;

        public StaffReportRepository(IAssessorUnitOfWork assessorUnitOfWork, IDbConnection connection)
        {
            _assessorUnitOfWork = assessorUnitOfWork;
            _connection = connection;
        }

        public Task<IEnumerable<StaffReport>> GetReportList()
        {
            var reports = _assessorUnitOfWork.AssessorDbContext.StaffReports.OrderBy(sr => sr.DisplayOrder).ToList();

            return Task.FromResult(reports.AsEnumerable());
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId)
        {
            var report = _assessorUnitOfWork.AssessorDbContext.StaffReports.FirstOrDefault(rep => rep.Id == reportId);

            if (report is null)
            {
                return null;
            }

            return (await _connection.QueryAsync(report.StoredProcedure, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure)
        {
            var reports = _assessorUnitOfWork.AssessorDbContext.StaffReports.ToList();

            var worksheetDetails = reports
                .Where(report => report.ReportType == "Download")
                .SelectMany(report =>
                {
                    try
                    {
                        var reportDetails = JsonConvert.DeserializeObject<ReportDetails>(report.ReportDetails);
                        return reportDetails?.Worksheets ?? new List<WorksheetDetails>();
                    }
                    catch (JsonReaderException)
                    {
                        return new List<WorksheetDetails>();
                    }
                })
                .FirstOrDefault(worksheet => worksheet.StoredProcedure == storedProcedure);

                if (worksheetDetails == null)
                {
                    throw new NotFoundException($"No matching report found for stored procedure '{storedProcedure}'");
                }

            return (await _connection.QueryAsync(worksheetDetails.StoredProcedure, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }

        Task<ReportType> IStaffReportRepository.GetReportTypeFromId(Guid reportId)
        {
            return Task.Run(() =>
            {
                var report = _assessorUnitOfWork.AssessorDbContext.StaffReports.FirstOrDefault(rep => rep.Id == reportId);
                if (report != null && report.ReportType == "Download")
                    return ReportType.Download;
                return ReportType.ViewOnScreen;
            });
        }

        public Task<ReportDetails> GetReportDetailsFromId(Guid reportId)
        {
            return Task.Run(() =>
            {
                var report = _assessorUnitOfWork.AssessorDbContext.StaffReports.FirstOrDefault(rep => rep.Id == reportId);

                if (report?.ReportDetails == null)
                    return new ReportDetails();

                var reportDetails = JsonConvert.DeserializeObject<ReportDetails>(report?.ReportDetails);
                return reportDetails;
            });
        }

    }
}
