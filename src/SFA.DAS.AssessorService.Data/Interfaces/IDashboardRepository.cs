using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IDashboardRepository
    {
        Task<EpaoDashboardResult> GetEpaoDashboard(string endPointAssessorOrganisationId, int pipelineCutoff);
    }

    public class EpaoDashboardResult
    {
        public int Standards { get; set; }
        public int Pipeline { get; set; }
        public int Assessments { get; set; }
    }
}
