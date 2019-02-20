using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

namespace SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient
{

    public interface IStandardCollationApiClient : IDisposable
    {
        Task<List<StandardCollation>> GetStandardCollations();
        Task<StandardCollation> GetStandardCollation(int standardId);
    }
}
