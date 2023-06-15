using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    /// <summary>
    /// Gets the company.
    /// <param name="companyNumber">The company number.</param>
    /// </summary>
    public interface ICompaniesHouseApiClient
    {
        Task<Company> GetCompany(string companyNumber);
        Task<bool> IsCompanyActivelyTrading(string companyNumber);
    }
}