using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApplyApiClient _applyApiClient;

        public AnswerService(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<List<string>> InjectApplyOrganisationAndContactIntoRegister(Guid applicationId)
        {
            var tradingName = await GetAnswer(applicationId, "trading-name");
            var useTradingName = await GetAnswer(applicationId, "use-trading-name");
            var contactName = await GetAnswer(applicationId, "contact-name");
            var contactAddress = await GetAnswer(applicationId, "contact-address");  //MFCMFC deal with address better
            var contactPostcode = await GetAnswer(applicationId, "contact-postcode");
            var contactEmail = await GetAnswer(applicationId, "contact-email");
            var contactPhoneNumber = await GetAnswer(applicationId, "contact-phone-number");
            var companyUkprn = await GetAnswer(applicationId, "company-ukprn");
            var companyNumber = await GetAnswer(applicationId, "company-number");
            var charityNumber = await GetAnswer(applicationId, "charity-number");
            var standardWebsite = await GetAnswer(applicationId, "standard-website");

            var request = new CreateOrganisationContactRequest
            {
                OrganisationName = "to get",
                OrganisationType = "to get",
                OrganisationUkprn = "to get",
                TradingName = tradingName,
                UseTradingName = true, //useTradingName,
                ContactName = contactName,
                ContactAddress1 = contactAddress,
                ContactAddress2 = null,
                ContactAddress3 = null,
                ContactAddress4 = null,

                ContactPostcode = contactPostcode,
                ContactEmail = contactEmail,
                ContactPhoneNumber = contactPhoneNumber,
                CompanyUkprn = companyUkprn,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber,
                StandardWebsite = standardWebsite
            };
            

            return new List<string>();
        }

        public async Task<string> GetAnswer(Guid applicationId, string questionTag)
        {
           var response= await _applyApiClient.GetAnswer(applicationId, questionTag);

            return response.Answer;
        }
    }
}
