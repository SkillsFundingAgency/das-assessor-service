using Dapper;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffReportRepository : IStaffReportRepository
    {
        private readonly AssessorDbContext _assessorDbContext;
        private readonly IDbConnection _connection;

        public StaffReportRepository(AssessorDbContext assessorDbContext, IDbConnection connection)
        {
            _assessorDbContext = assessorDbContext;
            _connection = connection;
        }

        public Task<IEnumerable<StaffReport>> GetReportList()
        {
            var reports = _assessorDbContext.StaffReports.OrderBy(sr => sr.DisplayOrder).ToList();

            return Task.FromResult(reports.AsEnumerable());
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId)
        {
            var report = _assessorDbContext.StaffReports.FirstOrDefault(rep => rep.Id == reportId);

            if (report is null)
            {
                return null;
            }
            else if(report.StoredProcedure == "StaffReports_DetailedExtract")
            {
                string fromdate = DateTime.UtcNow.AddDays(1 - DateTime.UtcNow.Day).ToString("yyyy-MM-dd");
                string todate = DateTime.UtcNow.AddDays(1 - DateTime.UtcNow.Day).AddMonths(1).ToString("yyyy-MM-dd");

                return (await _connection.QueryAsync(report.StoredProcedure, param: new { fromdate, todate }, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }

            return (await _connection.QueryAsync(report.StoredProcedure, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }

        Task<ReportType> IStaffReportRepository.GetReportTypeFromId(Guid reportId)
        {
            return Task.Run(() =>
            {

                var report = _assessorDbContext.StaffReports.FirstOrDefault(rep => rep.Id == reportId);

                if (report != null && report.ReportType == "Download")
                    return ReportType.Download;

                return ReportType.ViewOnScreen;
            });
        }
    }
}
