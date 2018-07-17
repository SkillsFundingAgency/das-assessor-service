using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.AssessmentOrgs
{
    public class ImportAssessmentOrgsHandler: IRequestHandler<AssessmentOrgsImportRequest, AssessmentOrgsImportResponse>
    {

        private readonly IAssessmentOrgsImporter _assessmentOrgsImporter;

        public ImportAssessmentOrgsHandler(IAssessmentOrgsImporter assessmentOrgsImporter)
        {
            _assessmentOrgsImporter = assessmentOrgsImporter;
        }

        public async Task<AssessmentOrgsImportResponse> Handle(AssessmentOrgsImportRequest request, CancellationToken cancellationToken)
        {
            return await _assessmentOrgsImporter.ImportAssessmentOrganisations();


        }
    }
}
