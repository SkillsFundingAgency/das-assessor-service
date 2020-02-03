using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    /// <summary>
    /// Companies House API docs are located at: https://developer.companieshouse.gov.uk/api/docs/index.html
    /// There is a Web-Friendly version located at: https://beta.companieshouse.gov.uk/
    /// </summary>
    public class CompaniesHouseApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<CompaniesHouseApiClient> _logger;
        private readonly IWebConfiguration _config;

        public CompaniesHouseApiClient(HttpClient client, ILogger<CompaniesHouseApiClient> logger,
            IWebConfiguration configurationService)
        {
            _client = client;
            _logger = logger;
            _config = configurationService;
        }

        public async Task<AssessorService.Api.Types.CompaniesHouse.Company> GetCompany(string companyNumber)
        {
            var company = await GetCompanyDetails(companyNumber);

            if (company != null)
            {
                company.Officers = await GetOfficers(company.CompanyNumber);
                company.PeopleWithSignificantControl = await GetPersonsWithSignificantControl(company.CompanyNumber);
            }

            return company;
        }

        public async Task<bool> IsCompanyActivelyTrading(string companyNumber)
        {
            var isTrading = false;

            var company = await GetCompanyDetails(companyNumber);

            if (company != null)
            {
                if(company.Status is null && "charitable-incorporated-organisation".Equals(company.Type, StringComparison.InvariantCultureIgnoreCase))
                {
                    isTrading = company.DissolvedOn == null && company.IsLiquidated != true;
                }
                else if("active".Equals(company.Status, StringComparison.InvariantCultureIgnoreCase))
                {
                    isTrading = company.DissolvedOn == null && company.IsLiquidated != true;
                } 
            }

            return isTrading;
        }

        #region HTTP Request Helpers

        private AuthenticationHeaderValue GetBasicAuthHeader()
        {
            var bytes = Encoding.ASCII.GetBytes($"{_config.CompaniesHouseApiAuthentication.ApiKey}:");
            var token = Convert.ToBase64String(bytes);
            return new AuthenticationHeaderValue("Basic", token);
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = GetBasicAuthHeader();

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        #endregion HTTP Request Helpers

        private async Task<AssessorService.Api.Types.CompaniesHouse.Company> GetCompanyDetails(string companyNumber)
        {
            _logger.LogInformation($"Searching Companies House - Company Details. Company Number: {companyNumber}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.CompanyDetails>($"/company/{companyNumber}");
            return Mapper
                .Map<AssessorService.Api.Types.Models.CompaniesHouse.CompanyDetails,
                    AssessorService.Api.Types.CompaniesHouse.Company>(apiResponse);
        }

        private async Task<IEnumerable<AssessorService.Api.Types.CompaniesHouse.Officer>> GetOfficers(
            string companyNumber, bool activeOnly = true)
        {
            _logger.LogInformation($"Searching Companies House - Officers. Company Number: {companyNumber}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.OfficerList>(
                    $"/company/{companyNumber}/officers?items_per_page=100");

            var items = activeOnly ? apiResponse.items?.Where(i => i.resigned_on is null) : apiResponse.items;

            var officers =
                Mapper
                    .Map<IEnumerable<AssessorService.Api.Types.Models.CompaniesHouse.Officer>,
                        IEnumerable<AssessorService.Api.Types.CompaniesHouse.Officer>>(items);

            foreach (var officer in officers)
            {
                officer.Disqualifications = await GetOfficerDisqualifications(officer.Id);
            }

            return officers;
        }

        private async Task<IEnumerable<AssessorService.Api.Types.CompaniesHouse.Disqualification>>
            GetOfficerDisqualifications(string officerId)
        {
            _logger.LogInformation(
                $"Searching Companies House - Natural Officer's Disqualifications. Officer Id: {officerId}");
            var apiResponseNatural =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.DisqualificationList>(
                    $"/disqualified-officers/natural/{officerId}");

            _logger.LogInformation(
                $"Searching Companies House - Corporate Officer's Disqualifications. Officer Id: {officerId}");
            var apiResponseCorporate =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.DisqualificationList>(
                    $"/disqualified-officers/corporate/{officerId}");

            var disqualifications = new List<AssessorService.Api.Types.Models.CompaniesHouse.Disqualification>();

            if (apiResponseNatural?.disqualifications != null)
            {
                disqualifications.AddRange(apiResponseNatural.disqualifications);
            }

            if (apiResponseCorporate?.disqualifications != null)
            {
                disqualifications.AddRange(apiResponseCorporate.disqualifications);
            }

            return Mapper
                .Map<IEnumerable<AssessorService.Api.Types.Models.CompaniesHouse.Disqualification>,
                    IEnumerable<AssessorService.Api.Types.CompaniesHouse.Disqualification>>(disqualifications);
        }

        private async Task<IEnumerable<AssessorService.Api.Types.CompaniesHouse.PersonWithSignificantControl>>
            GetPersonsWithSignificantControl(string companyNumber, bool activeOnly = true)
        {
            _logger.LogInformation(
                $"Searching Companies House - Persons With Significant Control. Company Number: {companyNumber}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.PersonWithSignificantControlList>(
                    $"/company/{companyNumber}/persons-with-significant-control?items_per_page=100");

            var items = activeOnly ? apiResponse.items?.Where(i => i.ceased_on is null) : apiResponse.items;
            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.CompaniesHouse.PersonWithSignificantControl>,
                IEnumerable<AssessorService.Api.Types.CompaniesHouse.PersonWithSignificantControl>>(items);
        }

        private async Task<IEnumerable<dynamic>> GetCharges(string companyNumber)
        {
            _logger.LogInformation($"Searching Companies House - Charges. Company Number: {companyNumber}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.ChargeList>(
                    $"/company/{companyNumber}/charges");
            return apiResponse.items;
        }

        private async Task<dynamic> GetInsolvencyDetails(string companyNumber)
        {
            _logger.LogInformation($"Searching Companies House - Insolvency. Company Number: {companyNumber}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.CompaniesHouse.InsolvencyDetails>(
                    $"/company/{companyNumber}/insolvency");
            return apiResponse;
        }
    }
}
