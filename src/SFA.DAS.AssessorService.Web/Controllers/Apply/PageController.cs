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
    }

    public class PageViewModel
    {
        public PageViewModel(Page page)
        {
            Title = page.Title;
            PageId = page.PageId;

            var inputs = page.Questions.SelectMany(q => q.Inputs);
            var outputs = page.Questions.SelectMany(q => q.Outputs);

            List<QuestionInputViewModel> inputViewModels = new List<QuestionInputViewModel>();

            foreach (var pageQuestion in page.Questions)
            {
                if (!pageQuestion.AllowMultipleAnswers)
                {
                    inputViewModels.AddRange(pageQuestion.Inputs.Select(i => new QuestionInputViewModel()
                    {
                        QuestionId = pageQuestion.QuestionId,
                        InputId = i.InputId,
                        Type = i.Type,
                        Label = i.Label,
                        Hint = i.Hint,
                        Value = outputs.SelectMany(o => o.Values).SingleOrDefault(v => v.InputId == i.InputId)?.Value
                    }));
                }
                else
                {
                    
                }
            }

            Questions = page.Questions.Select(q => new QuestionViewModel()
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                InputViewModels = inputViewModels.Where(ivm => ivm.QuestionId == q.QuestionId).ToList()
            }).ToList();


        }

        public string PageId { get; set; }
        public string Title { get; set; }
        
        public List<QuestionViewModel> Questions { get; set; }
    }


    public class QuestionViewModel
    {
        public string Title { get; set; }
        public string QuestionId { get; set; }
        public List<QuestionInputViewModel> InputViewModels { get; set; }
    }

    public class QuestionInputViewModel
    {
        public string QuestionId { get; set; }
        public string InputId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string Value { get; set; }
    }
}