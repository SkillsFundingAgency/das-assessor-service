using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class SequenceController : Controller
    {
        private readonly IApplyApiClient _apiClient;

        public SequenceController(IApplyApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("sequence")]
        public async Task<IActionResult> Index()
        {
            var userId = "1"; // From User / Session / Cookie etc.
            var sequenceSummary = await _apiClient.GetSequenceSummary(userId);
            return View("~/Views/Apply/Sequences/Index.cshtml", sequenceSummary);
        }

        [HttpGet("sequence/{sequenceId}")]
        public async Task<IActionResult> Sequence(string sequenceId)
        {
            var userId = "1"; // From User / Session / Cookie etc.
            var sequence = await _apiClient.GetSequence(userId, sequenceId);
            return View("~/Views/Apply/Sequences/Sequence.cshtml", sequence);
        }
    }
}