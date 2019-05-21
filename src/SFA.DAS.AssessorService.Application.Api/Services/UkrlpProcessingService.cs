using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class UkrlpProcessingService: IUkrlpProcessingService
    {
        //TODO: Possibly turn this into a class of values held against string?
        private const string CompaniesHouse = "companies house";
        private const string CharityCommision = "charity commission";

        public UkrlpProviderDetails ProcessDetails(List<ProviderDetails> providerDetails)
        {
            if (providerDetails== null || providerDetails.Count==0)
                return new UkrlpProviderDetails();

            var providerDetail = new UkrlpProviderDetails();

            var firstResult = providerDetails.ToList().FirstOrDefault();
            providerDetail.LegalName = firstResult.ProviderName;
            if (firstResult.ProviderAliases != null && firstResult.ProviderAliases.Count > 0)
            {
                providerDetail.TradingName = firstResult.ProviderAliases.First().Alias;
            }

            if (firstResult.VerificationDetails == null)
            {
                return providerDetail;
            }

            foreach (var verificationDetail in firstResult.VerificationDetails)
            {
                if (verificationDetail.VerificationAuthority?.ToLower() == CompaniesHouse.ToLower())
                {
                    providerDetail.CompanyNumber = verificationDetail.VerificationId;
                }

                if (verificationDetail.VerificationAuthority?.ToLower() == CharityCommision.ToLower())
                {
                    providerDetail.CharityNumber = verificationDetail.VerificationId;
                }
            }
            return providerDetail;
        }
    }

    public interface IUkrlpProcessingService
    {
        UkrlpProviderDetails ProcessDetails(List<ProviderDetails> providerDetails);
    }
}
