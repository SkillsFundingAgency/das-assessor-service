using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.CompaniesHouse
{
    public interface ICompaniesHouseApiClient
    {
        Task<Company> GetCompany(string companyNumber);
        Task<bool> IsCompanyActivelyTrading(string companyNumber);
    }
}