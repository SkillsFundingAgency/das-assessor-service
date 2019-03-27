using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApplyApiClient _applyApiClient;
      

        public AnswerService(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        {
            var tradingName = await GetAnswer(applicationId, "trading-name");
            var useTradingNameString = await GetAnswer(applicationId, "use-trading-name");
            var contactName = await GetAnswer(applicationId, "contact-name");
            var contactAddress1 = await GetAnswer(applicationId, "contact-address") ?? await GetAnswer(applicationId, "contact-address1");
            var contactAddress2 = await GetAnswer(applicationId, "contact-address2");
            var contactAddress3 = await GetAnswer(applicationId, "contact-address3");
            var contactAddress4 = await GetAnswer(applicationId, "contact-address4");
            var contactPostcode = await GetAnswer(applicationId, "contact-postcode");
            var contactEmail = await GetAnswer(applicationId, "contact-email");
            var contactPhoneNumber = await GetAnswer(applicationId, "contact-phone-number");
            var companyUkprn = await GetAnswer(applicationId, "company-ukprn");
            var companyNumber = await GetAnswer(applicationId, "company-number");
            var charityNumber = await GetAnswer(applicationId, "charity-number");
            var standardWebsite = await GetAnswer(applicationId, "standard-website");
            var organisation = await _applyApiClient.GetOrganisationForApplication(applicationId);
            var organisationCreatedBy = organisation.CreatedBy;
            var organisationName = organisation?.Name;
            var organisationType = organisation?.OrganisationType;
            var organisationUkprn = organisation?.OrganisationUkprn;
            var organisationReferenceType = organisation?.OrganisationDetails?.OrganisationReferenceType;
            var isEpaoApproved = organisation?.RoEPAOApproved;
            var useTradingName = useTradingNameString != null && (useTradingNameString.ToLower() == "yes" || useTradingNameString.ToLower() == "true" || useTradingNameString.ToLower() == "1");
            
            var command = new CreateOrganisationContactCommand
            (organisationName,
                organisationType,
                organisationUkprn?.ToString(),
                organisationReferenceType,
                isEpaoApproved,
                tradingName,
                useTradingName,
                contactName,
                contactAddress1,
                contactAddress2,
                contactAddress3,
                contactAddress4,
                contactPostcode,
                contactEmail,
                contactPhoneNumber,
                companyUkprn,
                companyNumber,
                charityNumber,
                standardWebsite,
                organisationCreatedBy);

            return command;
        }
        public async Task<string> GetAnswer(Guid applicationId, string questionTag)
        {
           var response= await _applyApiClient.GetAnswer(applicationId, questionTag);
            return response.Answer;
        }

   
    }

  
}
