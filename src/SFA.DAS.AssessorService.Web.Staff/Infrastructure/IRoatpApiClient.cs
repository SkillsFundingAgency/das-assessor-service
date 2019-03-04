
namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoatpApiClient
    {
       Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
       Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
    }
}
