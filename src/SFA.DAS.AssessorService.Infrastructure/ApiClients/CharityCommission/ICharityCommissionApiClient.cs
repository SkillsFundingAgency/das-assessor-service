using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.CharityCommission
{
    public interface ICharityCommissionApiClient
    {
        Task<Charity> GetCharity(int charityNumber);
        Task<bool> IsCharityActivelyTrading(int charityNumber);
    }
}