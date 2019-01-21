using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class ReportViewModel
    {
        public Guid? ReportId { get; set; }
        public IEnumerable<StaffReport> Reports { get; set; }

        public IEnumerable<IDictionary<string, object>> SelectedReportData { get; set; }

        public StaffReport SelectedReport
        {
            get
            {
                if (Reports is null)
                {
                    return null;
                }
                else
                {
                    return Reports.FirstOrDefault(rep => rep.Id == ReportId);
                }
            }
        }
    }
}
