using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    public interface ICharityCommissionApiClient
    {
        Task<Charity> GetCharity(int charityNumber);
        Task<bool> IsCharityActivelyTrading(int charityNumber);
    }
}