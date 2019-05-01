using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class MenuFilter: IActionFilter
    {
        private readonly ISessionService _sessionService;
        private readonly string _currentPage;
        public MenuFilter(ISessionService sessionService, string currentPage)
        {
            _sessionService = sessionService;
            _currentPage = currentPage;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _sessionService.Set("CurrentPage", _currentPage);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
