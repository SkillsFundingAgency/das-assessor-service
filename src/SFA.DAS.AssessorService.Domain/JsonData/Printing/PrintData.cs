using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Printing
{
    public class PrintData
    {
        public CoverLetter CoverLetter { get; set; }
        public PostalContact PostalContact { get; set; }
        public List<PrintCertificate> Certificates { get; set; }
    }

}
