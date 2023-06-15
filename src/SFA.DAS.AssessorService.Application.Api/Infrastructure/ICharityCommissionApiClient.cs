using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    /// <summary>
    /// Gets the charity.
    /// <param name="charityNumber">The charity number.</param>
    /// </summary>
    public interface ICharityCommissionApiClient
    {
        Task<Charity> GetCharity(int charityNumber);
        Task<bool> IsCharityActivelyTrading(int charityNumber);
    }
}