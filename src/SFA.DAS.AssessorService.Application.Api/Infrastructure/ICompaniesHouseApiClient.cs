using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    /// <summary>
    /// Companies house api client interface.
    /// </summary>
    public interface ICompaniesHouseApiClient
    {
        /// <summary>Gets the company.</summary>
        /// <param name="companyNumber">The company number.</param>
        Task<Company> GetCompany(string companyNumber);
        /// <summary>Determines whether the company is actively trading.</summary>
        /// <param name="companyNumber">The company number.</param>
        Task<bool> IsCompanyActivelyTrading(string companyNumber);
    }
}