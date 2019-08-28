using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private const string WorkflowType = "EPAO";

        public ApplicationController(IOrganisationsApiClient apiClient, IQnaApiClient qnaApiClient,
            IContactsApiClient contactsApiClient, IApplicationApiClient applicationApiClient, ILogger<ApplicationController> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
            _contactsApiClient = contactsApiClient;
            _applicationApiClient = applicationApiClient;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Application")]
        public async Task<IActionResult> Applications()
        {
            _logger.LogInformation($"Got LoggedInUser from Session: { User.Identity.Name}");

            var userId = await GetUserId();
            var org = await _apiClient.GetOrganisationByUserId(userId);
            var applications = await _applicationApiClient.GetApplications(userId, false);
            applications = applications.Where(app => app.ApplicationStatus != ApplicationStatus.Rejected).ToList();

            if (!applications.Any())
            {
                //ON-2068 Registered org  with no application created via digital service then
                //display empty list of application screen
                if (org != null)
                    return org.RoEPAOApproved ? View(applications) : View("~/Views/Application/Declaration.cshtml");

            }
            //ON-2068 If there is an existing application for an org that is registered then display it
            //in a list of application screen
            if (applications.Count() == 1 && (org != null && org.RoEPAOApproved))
                return View(applications);

            if (applications.Count() > 1)
                return View(applications);

            //This always return one record otherwise the previous logic would have handled the response
            var application = applications.First();

            switch (application.ApplicationStatus)
            {
                case ApplicationStatus.FeedbackAdded:
                    return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
                case ApplicationStatus.Rejected:
                case ApplicationStatus.Approved:
                    return View(applications);
                default:
                    return RedirectToAction("SequenceSignPost", new { Id = application.Id });
            }

        }

        [HttpPost("/Application")]
        public async Task<IActionResult> StartApplication()
        {
            var userId = await GetUserId();
            var org = await _apiClient.GetOrganisationByUserId(userId);

            var applicationStartRequest = new StartApplicationRequest
            {
                UserReference = userId.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData {
                    UseTradingName = false,
                    OrganisationName = org.EndPointAssessorName,
                    OrganisationReferenceId = org.Id.ToString()
                })
            };

            var qnaResponse = await _qnaApiClient.StartApplications(applicationStartRequest);

            var  appResponse = await _applicationApiClient.CreateApplication(new CreateApplicationRequest
            {
                ApplicationStatus = ApplicationStatus.InProgress,
                OrganisationId = org.Id,
                QnaApplicationId = qnaResponse?.ApplicationId??Guid.Empty,
                UserId = userId
            });

            return RedirectToAction("SequenceSignPost", new { Id = appResponse });
        }

        [HttpGet("/Application/{Id}")]
        public async Task<IActionResult> SequenceSignPost(Guid Id)
        {
            var userId = await GetUserId();
            var application = await _applicationApiClient.GetApplication(Id);

            if (application is null)
            {
                return RedirectToAction("Applications");
            }

            if (application.ApplicationStatus == ApplicationStatus.Approved)
            {
                return View("~/Views/Application/Approved.cshtml", application);
            }

            if (application.ApplicationStatus == ApplicationStatus.Rejected)
            {
                return View("~/Views/Application/Rejected.cshtml", application);
            }

            if (application.ApplicationStatus == ApplicationStatus.FeedbackAdded)
            {
                return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
            }

            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);

            StandardApplicationData applicationData = null;

            if (application.ApplicationData != null)
            {
                applicationData = new StandardApplicationData
                {
                    StandardName = application.ApplicationData.StandardName
                };
            }
            var sequenceNo = (SequenceNo)sequence.SequenceNo;

            //// Only go to search if application hasn't got a selected standard?
            if (sequenceNo == SequenceNo.Stage1)
            {
                return RedirectToAction("Sequence", new { application.ApplicationId });
            }
            else if (sequenceNo == SequenceNo.Stage2 && string.IsNullOrWhiteSpace(applicationData?.StandardName))
            {
                var org = await _apiClient.GetOrganisationByUserId(userId);
                if (org.RoEPAOApproved)
                {
                   return RedirectToAction("Index", "Standard", new { application.Id });
                }

                return View("~/Views/Application/Stage2Intro.cshtml", application.Id);
            }
            else if (sequenceNo == SequenceNo.Stage2)
            {
                return RedirectToAction("Sequence", new { application.Id });
            }

            throw new BadRequestException("Section does not have a valid DisplayType");
        }

        [HttpGet("/Application/{Id}/Sequence")]
        public async Task<IActionResult> Sequence(Guid Id)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            
            var sequenceVm = new SequenceViewModel(sequence, application.Id, sections, null);
            return View(sequenceVm);
        }

        [HttpGet("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionId}")]
        public async Task<IActionResult> Section(Guid Id, int sequenceNo, Guid sectionId)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var canUpdate = await CanUpdateApplication(application.ApplicationId, sequenceNo);
            if (!canUpdate)
            {
                return RedirectToAction("Sequence", new { Id });
            }

            var section = await _qnaApiClient.GetSection(application.ApplicationId, sectionId);
            var applicationSection = new ApplicationSection { Section = section, Id = Id };
            switch (section?.DisplayType)
            {
                case null:
                case SectionDisplayType.Pages:
                    return View("~/Views/Application/Section.cshtml", applicationSection);
                case SectionDisplayType.Questions:
                    return View("~/Views/Application/Section.cshtml", applicationSection);
                case SectionDisplayType.PagesWithSections:
                    return View("~/Views/Application/PagesWithSections.cshtml", applicationSection);
                default:
                    throw new BadRequestException("Section does not have a valid DisplayType");
            }
        }

        [HttpGet("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionId}/Pages/{pageId}"), ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> Page(Guid Id, int sequenceNo, Guid sectionId, string pageId, string redirectAction)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);

            var canUpdate = await CanUpdateApplication(application.ApplicationId, sequenceNo);
            if (!canUpdate)
            {
                return RedirectToAction("Sequence", new { Id });
            }

            PageViewModel viewModel = null;
            var returnUrl = Request.Headers["Referer"].ToString();

            if (!ModelState.IsValid)
            {
                // when the model state has errors the page will be displayed with the values which failed validation
                var page = JsonConvert.DeserializeObject<Page>((string)TempData["InvalidPage"]);

                var errorMessages = !ModelState.IsValid
                    ? ModelState.SelectMany(k => k.Value.Errors.Select(e => new ValidationErrorDetail()
                    {
                        ErrorMessage = e.ErrorMessage,
                        Field = k.Key
                    })).ToList()
                    : null;

                viewModel = new PageViewModel(Id, sequenceNo, sectionId, pageId, page, redirectAction,
                    returnUrl, errorMessages);
            }
            else
            {
                // when the model state has no errors the page will be displayed with the last valid values which were saved
                var page = await _qnaApiClient.GetPage(application.ApplicationId, sectionId, pageId);

                if (page != null && (!page.Active || page.NotRequired))
                {
                    var nextPage = page.Next.FirstOrDefault(p => p.Condition is null);

                    if (nextPage?.ReturnId != null && nextPage?.Action == "NextPage")
                    {
                        pageId = nextPage.ReturnId;
                        return RedirectToAction("Page",
                            new { Id, sequenceNo, sectionId, pageId, redirectAction });
                    }
                    else
                    {
                        return RedirectToAction("Section", new { Id, sequenceNo, sectionId });
                    }
                }

                page = await GetDataFedOptions(page);

                viewModel = new PageViewModel(Id, sequenceNo, sectionId, pageId, page, redirectAction,
                    returnUrl, null);

                ProcessPageVmQuestionsForStandardName(viewModel.Questions, application);
            }

            if (viewModel.AllowMultipleAnswers)
            {
                return View("~/Views/Application/Pages/MultipleAnswers.cshtml", viewModel);
            }

            return View("~/Views/Application/Pages/Index.cshtml", viewModel);
        }

        [HttpPost("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionId}/Pages/{pageId}"), ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> SaveAnswers(Guid Id, int sequenceNo, Guid sectionId, string pageId, string redirectAction, string __formAction)
        {
            var application = await _applicationApiClient.GetApplication(Id);

            var canUpdate = await CanUpdateApplication(application.ApplicationId, sequenceNo);
            if (!canUpdate)
            {
                return RedirectToAction("Sequence", new { application.ApplicationId });
            }

            var page = await _qnaApiClient.GetPage(application.ApplicationId, sectionId, pageId);

            var errorMessages = new List<ValidationErrorDetail>();
            var answers = new List<Answer>();

            var fileValidationPassed = FileValidator.FileValidationPassed(answers, page, errorMessages, ModelState, HttpContext.Request.Form.Files);
            GetAnswersFromForm(answers);

            if (page.AllowMultipleAnswers)
            {
                var pageAddResponse = await _qnaApiClient.AddPageAnswers(application.ApplicationId, sectionId, pageId, answers);
                if (pageAddResponse?.Success == null ? false : pageAddResponse.Success && fileValidationPassed)
                {
                    if (__formAction == "Add")
                    {
                        return RedirectToAction("Page", new
                        {
                            Id,
                            sequenceNo,
                            sectionId,
                            pageId = pageAddResponse.Page.PageId,
                            redirectAction
                        });
                    }
                    else
                    {
                        if (redirectAction == "Feedback")
                            return RedirectToAction("Feedback", new { Id });

                        var nextAction = pageAddResponse.Page.Next.SingleOrDefault(x => x.Action == "NextPage");

                        if (!string.IsNullOrEmpty(nextAction.Action))
                            return RedirectToNextAction(Id, sequenceNo, sectionId, redirectAction, nextAction.Action, nextAction.ReturnId);
                    }
                }
                else if(page.PageOfAnswers?.Count > 0)
                {
                    var nextAction = page.Next.SingleOrDefault(x => x.Action == "NextPage");

                    if (!string.IsNullOrEmpty(nextAction.Action))
                        return RedirectToNextAction(Id, sequenceNo, sectionId, redirectAction, nextAction.Action, nextAction.ReturnId);
                }

                await SetResponseValidationErrors(pageAddResponse?.ValidationErrors, page);
            }
            else
            {
                var updatePageResult = await _qnaApiClient.AddPageAnswer(application.ApplicationId, sectionId, pageId, answers);

                if (updatePageResult?.ValidationPassed == null ? false : updatePageResult.ValidationPassed && fileValidationPassed)
                {
                    await UploadFilesToStorage(application.ApplicationId, sectionId, pageId,page, answers);

                    if (redirectAction == "Feedback")
                        return RedirectToAction("Feedback", new { Id });

                    if (!string.IsNullOrEmpty(updatePageResult.NextAction))
                        return RedirectToNextAction(Id, sequenceNo, sectionId, redirectAction, updatePageResult.NextAction, updatePageResult.NextActionId);
                }

                await SetResponseValidationErrors(updatePageResult?.ValidationErrors, page);
            }

            return RedirectToAction("Page", new { Id, sequenceNo, sectionId, pageId, redirectAction });
        }

        [HttpPost("/Application/DeleteAnswer")]
        public async Task<IActionResult> DeleteAnswer(Guid Id, int sequenceNo, Guid sectionId, string pageId, Guid answerId, string redirectAction)
        {
            var application = await _applicationApiClient.GetApplication(Id);

            await _qnaApiClient.RemovePageAnswer(application.ApplicationId, sectionId, pageId, answerId);

            return RedirectToAction("Page", new { Id, sequenceNo, sectionId, pageId, redirectAction });
        }

        [HttpGet("Application/{Id}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        public async Task<IActionResult> Download(Guid Id, Guid sectionId, string pageId, string questionId, string filename)
        {
            var application = await _applicationApiClient.GetApplication(Id);

            var response = await _qnaApiClient.DownloadFile(application.ApplicationId, sectionId, pageId, questionId, filename);

            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);

        }

        [HttpGet("Application/{Id}/SequenceNo/{sequenceNo}Section/{sectionId}/Page/{pageId}/Question/{questionId}/download/{filename}/RedirectAction/{redirectAction}")]
        public async Task<IActionResult> DeleteFile(Guid Id, int sequenceNo, Guid sectionId, string pageId, string questionId, string filename, string redirectAction)
        {
            var application = await _applicationApiClient.GetApplication(Id);

            await _qnaApiClient.DeleteFile(application.ApplicationId, sectionId, pageId, questionId, filename);

            return RedirectToAction("Page", new { Id, sequenceNo, sectionId, pageId,  redirectAction });
        }

        [HttpPost("/Application/Submit")]
        public async Task<IActionResult> Submit(Guid applicationId, int sequenceNo)
        {
            var application = await _applicationApiClient.GetApplication(applicationId);
            var canUpdate = await CanUpdateApplication(application.ApplicationId, sequenceNo);
            if (!canUpdate)
            {
                return RedirectToAction("Sequence", new { applicationId });
            }

            var activeSequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeSequence.Id);
            var errors = ValidateSubmit(sections);
            if (errors.Any())
            {
                var sequenceVm = new SequenceViewModel(activeSequence, applicationId,sections, errors);

                if (activeSequence.Status == ApplicationSequenceStatus.FeedbackAdded)
                {
                    return View("~/Views/Application/Feedback.cshtml", sequenceVm);
                }
                else
                {
                    return View("~/Views/Application/Sequence.cshtml", sequenceVm);
                }
            }

            //if (await _apiClient.Submit(applicationId, sequenceId, User.GetUserId(), User.GetEmail()))
            //{
            //    return RedirectToAction("Submitted", new { applicationId });
            //}
            //else
            //{
                // unable to submit
                return RedirectToAction("NotSubmitted", new { applicationId });
            //}
        }

        private RedirectToActionResult RedirectToNextAction(Guid Id, int sequenceNo, Guid sectionId, string redirectAction, string nextAction, string nextActionId)
        {
            if (nextAction == "NextPage")
            {
                return RedirectToAction("Page", new
                {
                    Id,
                    sequenceNo,
                    sectionId,
                    pageId = nextActionId,
                    redirectAction
                });
            }

            return nextAction == "ReturnToSection"
                ? RedirectToAction("Section", "Application", new { Id, sequenceNo, sectionId })
                : RedirectToAction("Sequence", "Application", new { Id });
        }

        private async Task SetResponseValidationErrors(List<KeyValuePair<string, string>> validationErrors, Page page)
        {
            if (validationErrors != null)
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            var invalidPage = await GetDataFedOptions(page);
            this.TempData["InvalidPage"] = JsonConvert.SerializeObject(invalidPage);
        }

        private async Task UploadFilesToStorage(Guid applicationId, Guid sectionId, string pageId, Page page, List<Answer> answers)
        {
            if (HttpContext.Request.Form.Files.Any() && page != null)
            {
                foreach(var answer in answers)
                {
                    if(page.Questions.Any(q => q.QuestionId == answer.QuestionId))
                    {
                        await _qnaApiClient.Upload(applicationId, sectionId, pageId, answer.QuestionId, answer.Value, HttpContext.Request.Form.Files);
                    }
                }
            }
        }

        private void GetAnswersFromForm(List<Answer> answers)
        {
            foreach (var keyValuePair in HttpContext.Request.Form.Where(f => !f.Key.StartsWith("__")))
            {
                answers.Add(new Answer() { QuestionId = keyValuePair.Key, Value = keyValuePair.Value });
            }
        }

        private async Task<Page> GetDataFedOptions(Page page)
        {
            if (page != null)
            {
                foreach (var question in page.Questions)
                {
                    if (question.Input.Type.StartsWith("DataFed_"))
                    {
                        var questionOptions = await _applicationApiClient.GetQuestionDataFedOptions();
                        question.Input.Options = questionOptions;
                        question.Input.Type = question.Input.Type.Replace("DataFed_", "");
                    }
                }
            }

            return page;
        }

        private void ProcessPageVmQuestionsForStandardName(List<QuestionViewModel> pageVmQuestions, ApplicationResponse application)
        {
            if (pageVmQuestions == null) return;

            var placeholderString = "StandardName";
            var isPlaceholderPresent = false;

            foreach (var question in pageVmQuestions)

                if (question.Label.Contains($"[{placeholderString}]") ||
                   question.Hint.Contains($"[{placeholderString}]") ||
                    question.QuestionBodyText.Contains($"[{placeholderString}]") ||
                    question.ShortLabel.Contains($"[{placeholderString}]")
                   )
                    isPlaceholderPresent = true;

            if (!isPlaceholderPresent) return;

            var standardName = application?.ApplicationData?.StandardName;

            if (string.IsNullOrEmpty(standardName)) standardName = "the standard to be selected";

            foreach (var question in pageVmQuestions)
            {
                question.Label = question.Label?.Replace($"[{placeholderString}]", standardName);
                question.Hint = question.Hint?.Replace($"[{placeholderString}]", standardName);
                question.QuestionBodyText = question.QuestionBodyText?.Replace($"[{placeholderString}]", standardName);
                question.ShortLabel = question.Label?.Replace($"[{placeholderString}]", standardName);
            }
        }
        private List<ValidationErrorDetail> ValidateSubmit(List<Section> sections)
        {
            var validationErrors = new List<ValidationErrorDetail>();

            if (sections is null)
            {
                var validationError = new ValidationErrorDetail(string.Empty, $"Cannot submit empty sequence");
                validationErrors.Add(validationError);
            }
            else if (sections.Where(sec => sec.QnAData.Pages.Count(x => x.Complete == true) != sec.QnAData.Pages.Count(x => x.Active)).Any())
            {
                foreach (var sectionQuestionsNotYetCompleted in sections.Where(sec => sec.QnAData.Pages.Count(x => x.Complete == true) != sec.QnAData.Pages.Count(x => x.Active)))
                {
                    var validationError = new ValidationErrorDetail(sectionQuestionsNotYetCompleted.Id.ToString(), $"You need to complete the '{sectionQuestionsNotYetCompleted.LinkTitle}' section");
                    validationErrors.Add(validationError);
                }
            }
            else if (sections.Where(sec => sec.QnAData.RequestedFeedbackAnswered is false || sec.QnAData.Pages.Any(p => !p.AllFeedbackIsCompleted)).Any())
            {
                foreach (var sectionFeedbackNotYetCompleted in sections.Where(sec => sec.QnAData.RequestedFeedbackAnswered is false || sec.QnAData.Pages.Any(p => !p.AllFeedbackIsCompleted)))
                {
                    var validationError = new ValidationErrorDetail(sectionFeedbackNotYetCompleted.Id.ToString(), $"You need to complete the '{sectionFeedbackNotYetCompleted.LinkTitle}' section");
                    validationErrors.Add(validationError);
                }
            }

            return validationErrors;
        }

        private async Task<bool> CanUpdateApplication(Guid applicationId, int sequenceNo)
        {
            bool canUpdate = false;

            var sequence = await _qnaApiClient.GetApplicationActiveSequence(applicationId);

            if (sequence?.Status != null && (int)sequence.SequenceNo == sequenceNo)
            {
                canUpdate = sequence.Status == ApplicationSequenceStatus.Draft || sequence.Status == ApplicationSequenceStatus.FeedbackAdded;
            }

            return canUpdate;
        }

        private async Task<Guid> GetUserId()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await _contactsApiClient.GetContactBySignInId(signinId);

            return contact?.Id ?? Guid.Empty;
        }
    }
}