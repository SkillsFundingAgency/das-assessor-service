using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<EpaoDashboardResult> GetEpaoDashboard(string endPointAssessorOrganisationId);
    }

    public class EpaoDashboardResult
    {
        public int Standards { get; set; }
        public int Pipeline { get; set; }
        public int Assessments { get; set; }
    }
}
