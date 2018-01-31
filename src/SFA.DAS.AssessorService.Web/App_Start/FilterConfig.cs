using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.AssessorService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
