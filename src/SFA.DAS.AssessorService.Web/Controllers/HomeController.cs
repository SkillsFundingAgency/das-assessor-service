using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Models;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IStandardsApiClient _standardsApiClient;

        public HomeController(IDistributedCache cache, IStandardsApiClient standardsApiClient)
        {
            _cache = cache;
            _standardsApiClient = standardsApiClient;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotRegistered()
        {
            return View();
        }

        public async Task<IActionResult> NotActivated(string epaoId)
        {
            GetEpaoRegisteredStandardsResponse standard;

            try
            {
                var standards = await _standardsApiClient.GetEpaoRegisteredStandards(epaoId, 1);
                standard = standards.Items.FirstOrDefault();
            }
            catch
            {
                standard = null;
            }

            return View(standard);
        }

        public IActionResult InvalidRole()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
