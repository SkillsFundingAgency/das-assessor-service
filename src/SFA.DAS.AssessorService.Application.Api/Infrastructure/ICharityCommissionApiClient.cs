using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    /// <summary>
    /// Charity commision api client interface.
    /// </summary>
    public interface ICharityCommissionApiClient
    {
        /// <summary>Gets the charity.</summary>
        /// <param name="charityNumber">The charity number.</param>
        Task<Charity> GetCharity(int charityNumber);
        /// <summary>Determines whether the charity is actively trading.</summary>
        /// <param name="charityNumber">The charity number.</param>
        Task<bool> IsCharityActivelyTrading(int charityNumber);
    }
}