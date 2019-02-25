using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public interface IAnswerService
    {
        Task<List<string>> InjectApplyOrganisationAndContactIntoRegister(Guid applicationId);
        Task<string> GetAnswer(Guid applicationId, string questionTag);
    }
}
