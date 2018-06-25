using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiClient _apiClient;

        public DashboardController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<IActionResult> Index()
        {
            var certs = await _apiClient.GetCertificates();
            return View(certs);
        }
    }
}