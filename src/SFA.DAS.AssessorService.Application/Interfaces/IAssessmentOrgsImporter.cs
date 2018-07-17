using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsImporter
    {
       Task<AssessmentOrgsImportResponse> ImportAssessmentOrganisations();
    }

    public class AssessmentOrgsImporter : IAssessmentOrgsImporter
    {
        public async Task<AssessmentOrgsImportResponse> ImportAssessmentOrganisations()
        {
            return new AssessmentOrgsImportResponse {Status = "ok2"};
        }
    }
}
