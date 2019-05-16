using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IUkrlpApiClient
    {
        Task<UkrlpProviderDetails> Get(string ukprn);

    }
}