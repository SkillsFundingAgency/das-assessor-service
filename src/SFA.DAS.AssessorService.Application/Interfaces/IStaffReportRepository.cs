using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStaffReportRepository
    {
        Task<IEnumerable<StaffReport>> GetReportList();

        Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId);
    }
}
