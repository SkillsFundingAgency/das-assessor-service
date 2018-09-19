using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class PageController : Controller
    {
        private readonly IApplyApiClient _apiClient;

        public PageController(IApplyApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("Page/{pageId}")]
        public async Task<IActionResult> Index(string pageId)
        {
            var userId = "1"; // From User / Session / Cookie etc.
            var page = await _apiClient.GetPage(userId, pageId);

            var pageVm = new PageViewModel(page);
            
            
            return View("~/Views/Apply/Pages/Index.cshtml", pageVm);
        }

        [HttpPost("Page/{pageId}")]
        public async Task<IActionResult> SaveAnswers(string pageId)
        {
            var userId = "1";

            var answers = new List<Answer>();

            foreach (var keyValuePair in HttpContext.Request.Form.Where(f => !f.Key.StartsWith("__")))
            {
                answers.Add(new Answer() {QuestionId = keyValuePair.Key, Value = keyValuePair.Value});
            }

            var updatePageResult = await _apiClient.UpdatePage(userId, pageId, answers);

            if (updatePageResult.ValidationPassed)
            {
                var pageNext = updatePageResult.Page.Next;
                
                if (pageNext.Action == "NextPage")
                {
                    return RedirectToAction("Index", new {pageId = pageNext.ReturnId});
                }

                return pageNext.Action == "ReturnToSequence"
                    ? RedirectToAction("Sequence", "Sequence", new {sequenceId = pageNext.ReturnId})
                    : RedirectToAction("Index", "Sequence");
            }
            else
            {
                foreach (var error in updatePageResult.ValidationErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }
            
            var pageVm = new PageViewModel(updatePageResult.Page);
            return View("~/Views/Apply/Pages/Index.cshtml", pageVm);
        }
    }

    public class PageViewModel
    {
        public PageViewModel(Page page)
        {
            Title = page.Title;
            PageId = page.PageId;
            SequenceId = page.SequenceId;

            var questions = page.Questions;
            var answers = page.Answers;

            Questions = new List<QuestionViewModel>();
            
            Questions.AddRange(questions.Select(q => new QuestionViewModel()
            {
                Label = q.Label,
                QuestionId = q.QuestionId,
                Type = q.Input.Type,
                Hint = q.Hint,
                Options = q.Input.Options,
                Value = answers?.SingleOrDefault(a => a?.QuestionId == q.QuestionId)?.Value
            }));
        }

        public string PageId { get; set; }
        public string Title { get; set; }
        
        public List<QuestionViewModel> Questions { get; set; }
        public string SequenceId { get; set; }
    }


    public class QuestionViewModel
    {
        public string QuestionId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string Value { get; set; }
        public dynamic Options { get; set; }
    }
}