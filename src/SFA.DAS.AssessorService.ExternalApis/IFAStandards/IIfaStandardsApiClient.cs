using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;

namespace SFA.DAS.AssessorService.ExternalApis.IFAStandards
{
    public interface IIfaStandardsApiClient : IDisposable
    {
        Task<IfaStandard> GetStandard(int standardId);
        Task<List<IfaStandard>> GetAllStandards();
    }
}
