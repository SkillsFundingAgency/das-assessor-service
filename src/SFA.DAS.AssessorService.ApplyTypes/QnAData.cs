using SFA.DAS.QnA.Api.Types.Page;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class QnAData
    {
        public List<Page> Pages { get; set; }
        public FinancialApplicationGrade FinancialApplicationGrade { get; set; }
    }
}