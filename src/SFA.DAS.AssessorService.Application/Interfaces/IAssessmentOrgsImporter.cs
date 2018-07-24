using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsImporter
    {
       AssessmentOrgsImportResponse ImportAssessmentOrganisations(string operation);
    }
}
