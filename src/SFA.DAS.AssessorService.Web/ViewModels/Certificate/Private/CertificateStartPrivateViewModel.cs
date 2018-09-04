using System.Collections.Generic;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateStartPrivateViewModel
    {
        public long Uln { get; set; }    
        public string Surname { get; set; }
        public bool IsPrivatelyFunded { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }

    }
}
