using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.UKRLP
{
    public class UkrlpProviderDetails
    {
        public bool IsMatched => !string.IsNullOrEmpty(LegalName);
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
    }
}
