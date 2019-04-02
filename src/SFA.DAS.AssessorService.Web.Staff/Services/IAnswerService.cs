using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public interface IAnswerService
    {
        Task<string> GetAnswer(Guid applicationId, string questionTag);
        Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId);
        Task<CreateOrganisationStandardCommand> GatherAnswersForOrganisationStandardForApplication(Guid applicationId, string endPointAssessorOrganisationId);
    }
}
