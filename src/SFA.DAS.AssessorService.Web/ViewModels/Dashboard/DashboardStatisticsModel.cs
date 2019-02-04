using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.ViewModels.Dashboard
{
    public class DashboardStatisticsModel
    {
        public int PipelinesCount { get; set; }
        public int AssessmentsCount { get; set; }
        public int StandardsCount { get; set; }
    }
}
