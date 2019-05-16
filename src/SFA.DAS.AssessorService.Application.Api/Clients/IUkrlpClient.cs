using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Clients
{
    public interface IUkrlpClient
    {
        Task<List<ProviderDetails>> GetTrainingProviderByUkprn(long ukprn);
    }
}