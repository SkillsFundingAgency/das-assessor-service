using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IApiValidationService _apiValidationService;
        private readonly IApplicationService _applicationService;
        private readonly IOrganisationsApiClient _orgApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IWebConfiguration _config;
        private const string WorkflowType = "EPAO";

        public ApplicationController(IApplicationService applicationService, IOrganisationsApiClient orgApiClient, IQnaApiClient qnaApiClient, IWebConfiguration config,
            IContactsApiClient contactsApiClient, IApplicationApiClient applicationApiClient, ILogger<ApplicationController> logger, IApiValidationService apiValidationService)
        {
            _logger = logger;
            _apiValidationService = apiValidationService;
            _applicationService = applicationService;
            _orgApiClient = orgApiClient;
            _contactsApiClient = contactsApiClient;
            _applicationApiClient = applicationApiClient;
            _qnaApiClient = qnaApiClient;
            _config = config;
        }

        [HttpGet("/Application")]
        public async Task<IActionResult> Applications()
        {
            _logger.LogInformation($"Got LoggedInUser from Session: { User.Identity.Name}");

            var userId = await GetUserId();
            var org = await _orgApiClient.GetOrganisationByUserId(userId);
            var applications = await _applicationApiClient.GetApplications(userId, false);
            applications = applications?.Where(app => app.ApplicationStatus != ApplicationStatus.Declined).ToList();

            if (applications is null || applications.Count == 0)
            {
                if (org != null)
                {   
                    if(org.RoEPAOApproved)
                    {
                        // an organistion maybe registered with no applications, as it has been migrated in 
                        // the approved state from the pre-digital service, display an empty list
                        return RedirectToAction(nameof(StandardApplications));
                    }

                    return View("~/Views/Application/Declaration.cshtml");
                }

                return RedirectToAction("Index", "OrganisationSearch");
            }
            else if (applications.Count == 1 && org?.RoEPAOApproved is true)
            {
                // when there is an existing application for an organisation that is registered 
                // then display the single application
                return RedirectToAction(nameof(StandardApplications));
            }
            else if (applications.Count > 1)
            {
                return RedirectToAction(nameof(StandardApplications));
            }

            // otherwise there must be a single application for an organisation which is not registered
            var application = applications.First();

            switch (application.ApplicationStatus)
            {
                case ApplicationStatus.FeedbackAdded:
                    return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
                case ApplicationStatus.Declined:
                case ApplicationStatus.Approved:
                    return View(applications);
                case ApplicationStatus.Submitted:
                case ApplicationStatus.Resubmitted:
                    return RedirectToAction("Submitted", new { application.Id });
                default:
                    return RedirectToAction("SequenceSignPost", new { application.Id });
            }
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("/Application/StandardApplications")]
        public async Task<IActionResult> StandardApplications()
        {
            var userId = await GetUserId();
            var applications = await _applicationApiClient.GetApplications(userId, false);
            applications = applications?.Where(app => app.ApplicationStatus != ApplicationStatus.Declined).ToList();

            return View(applications);
        }

        [HttpPost("/Application")]
        public async Task<IActionResult> StartApplication()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await GetUserContact(signinId);
            var org = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var existingApplications = (await _applicationApiClient.GetApplications(contact.Id, false))?.Where( x=> x.OrganisationId == org.Id && x.ApplicationStatus != ApplicationStatus.Declined);
            if(existingApplications != null)
            {
                var existingEmptyApplication = existingApplications.SingleOrDefault(x => x.StandardCode == null);
                if(existingEmptyApplication != null)
                    return RedirectToAction("SequenceSignPost", new { existingEmptyApplication.Id });
            }

            var applicationStartRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = org.EndPointAssessorName,
                    OrganisationReferenceId = org.Id.ToString(),
                    OrganisationType = org.OrganisationType,
                    // NOTE: Surely it would be a good idea to include more info from the preamble search here?? Not been spec'ed at this point though :(
                    CompanySummary = org.CompanySummary,
                    CharitySummary = org.CharitySummary
                })
            };

            var qnaResponse = await _qnaApiClient.StartApplications(applicationStartRequest);
            var allApplicationSequences = await _qnaApiClient.GetAllApplicationSequences(qnaResponse.ApplicationId);
            var sections = allApplicationSequences.Select(async sequence => await _qnaApiClient.GetSections(qnaResponse.ApplicationId, sequence.Id)).Select(t => t.Result).ToList();

            var id = await _applicationApiClient.CreateApplication(BuildCreateApplicationRequest(qnaResponse.ApplicationId, contact, org, _config.ReferenceFormat, allApplicationSequences, sections));

            return RedirectToAction("SequenceSignPost", new { Id = id });
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

            switch(application.ApplicationStatus)
            {
                case ApplicationStatus.Approved:
                    return View("~/Views/Application/Approved.cshtml", application);
                case ApplicationStatus.Declined:
                    return View("~/Views/Application/Rejected.cshtml", application);
                case ApplicationStatus.FeedbackAdded:
                    return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
                case ApplicationStatus.Submitted:
                case ApplicationStatus.Resubmitted:
                    return RedirectToAction("Submitted", new { application.Id });
                default:
                    break;
            }

            StandardApplicationData applicationData = null;

            if (application.ApplyData != null)
            {
                applicationData = new StandardApplicationData
                {
                    StandardName = application.ApplyData?.Apply?.StandardName
                };
            }

            if (IsSequenceActive(application,1))
            {
                return RedirectToAction("Sequence", new { Id , sequenceNo = 1});
            }
            else if (!IsSequenceActive(application, 1) && string.IsNullOrWhiteSpace(applicationData?.StandardName))
            {
                var org = await _orgApiClient.GetOrganisationByUserId(userId);
                if (org.RoEPAOApproved)
                {
                   return RedirectToAction("Index", "Standard", new { Id });
                }

                return View("~/Views/Application/Stage2Intro.cshtml", application.Id);
            }
            else if (!IsSequenceActive(application, 1) && !string.IsNullOrWhiteSpace(applicationData?.StandardName))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo = 2 });
            }

            throw new BadRequestException("Section does not have a valid DisplayType");
        }


        private static bool IsSequenceActive(ApplicationResponse applicationResponse, int sequenceNo)
        {
            //A sequence can be considered active even if it does not exist in the ApplyData, since it has not yet been submitted and is in progress.
            return applicationResponse.ApplyData?.Sequences?.Any(x => x.SequenceNo == sequenceNo && x.IsActive) ?? true;
        }

        [HttpGet("/Application/{Id}/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid Id, int sequenceNo)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo))
            {
                return RedirectToAction("Applications");
            }

            var sequence = await _qnaApiClient.GetSequenceBySequenceNo(application.ApplicationId, sequenceNo);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            var applyData = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
            
            var sequenceVm = new SequenceViewModel(sequence, application.Id, BuildPageContext(application, sequence), sections, applyData.Sections, null);
            if(application.ApplyData != null && application.ApplyData.Sequences != null)
            {
                var seq= application.ApplyData.Sequences.SingleOrDefault(x => x.SequenceId == sequence.Id && x.SequenceNo == sequence.SequenceNo);
                if (seq != null && seq.Status == ApplicationSequenceStatus.Submitted)
                    sequenceVm.Status = seq.Status;
            }
            return View(sequenceVm);
        }

        [HttpGet("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}")]
        public async Task<IActionResult> Section(Guid Id, int sequenceNo, int sectionNo)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo, sectionNo))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo });
            }

            var sequence = await _qnaApiClient.GetSequenceBySequenceNo(application.ApplicationId, sequenceNo);
            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);
            var applicationSection = new ApplicationSection { Section = section, Id = Id };
            applicationSection.SequenceNo = sequenceNo;
            applicationSection.PageContext = BuildPageContext(application, sequence);

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

        [HttpGet("/Application/{id}/Cancel")]
        public async Task<IActionResult> ResetApplicatonToStage1(Guid id)
        {
            var userId = await GetUserId();
            await _applicationService.ResetApplicationToStage1(id, userId);
            return RedirectToAction(nameof(SequenceSignPost), new { id });
        }

        [HttpGet("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}/Pages/{pageId}"), ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> Page(Guid Id, int sequenceNo, int sectionNo, string pageId, string __redirectAction, string __summaryLink = "Show")
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo, sectionNo))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo });
            }

            var sequence = await _qnaApiClient.GetSequenceBySequenceNo(application.ApplicationId, sequenceNo);
            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            PageViewModel viewModel = null;
            var returnUrl = Request.Headers["Referer"].ToString();
            var pageContext = BuildPageContext(application, sequence);
            if (!ModelState.IsValid)
            {

                // when the model state has errors the page will be displayed with the values which failed validation
                var page = JsonConvert.DeserializeObject<Page>((string)TempData["InvalidPage"]);
                page = await GetDataFedOptions(page);

                var errorMessages = !ModelState.IsValid
                    ? ModelState.SelectMany(k => k.Value.Errors.Select(e => new ValidationErrorDetail()
                    {
                        ErrorMessage = e.ErrorMessage,
                        Field = k.Key
                    })).ToList()
                    : null;

                if (page.ShowTitleAsCaption)
                {
                    page.Title = section.Title;
                }

                UpdateValidationDetailsForAddress(page, errorMessages);
                viewModel = new PageViewModel(Id, sequenceNo, sectionNo, pageId, page, pageContext, __redirectAction,
                    returnUrl, errorMessages, __summaryLink);
            }
            else
            {
                // when the model state has no errors the page will be displayed with the last valid values which were saved
                try
                {
                    var page = await _qnaApiClient.GetPageBySectionNo(application.ApplicationId, sequenceNo, sectionNo, pageId);

                    if (page != null && (!page.Active))
                    {
                        var nextPage = page.Next.FirstOrDefault(p => p.Conditions is null || p.Conditions.Count == 0);

                        if (nextPage?.ReturnId != null && nextPage?.Action == "NextPage")
                        {
                            pageId = nextPage.ReturnId;
                            return RedirectToAction("Page",
                                new { Id, sequenceNo, sectionNo, pageId, __redirectAction });
                        }
                        else
                        {
                            return RedirectToAction("Section", new { Id, sequenceNo, sectionNo });
                        }
                    }

                    page = await GetDataFedOptions(page);

                    if (page.ShowTitleAsCaption)
                    {
                        page.Title = section.Title;
                    }
                    
                    viewModel = new PageViewModel(Id, sequenceNo, sectionNo, page.PageId, page, pageContext, __redirectAction,
                        returnUrl, null, __summaryLink);

                }
                catch (Exception ex)
                {
                    if(ex.Message.Equals("Could not find the page", StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Applications");
                    throw ex;
                }

                ProcessPageVmQuestionsForStandardName(viewModel.Questions, application);
            }

            if (viewModel.AllowMultipleAnswers)
            {
                return View("~/Views/Application/Pages/MultipleAnswers.cshtml", viewModel);
            }

            return View("~/Views/Application/Pages/Index.cshtml", viewModel);
        }

        [HttpPost("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}/Pages/{pageId}/multiple"), ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> SaveMultiplePageAnswers(Guid Id, int sequenceNo, int sectionNo, string pageId, string formAction, string __redirectAction, string __summaryLink)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo, sectionNo))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo });
            }

            try
            {
                var page = await _qnaApiClient.GetPageBySectionNo(application.ApplicationId, sequenceNo, sectionNo, pageId);

                if (page.AllowMultipleAnswers)
                {
                    var answers = GetAnswersFromForm(page);
                    var pageAddResponse = await _qnaApiClient.AddAnswersToMultipleAnswerPage(application.ApplicationId, page.SectionId.Value, page.PageId, answers);
                    if (pageAddResponse?.Success != null && pageAddResponse.Success)
                    {
                        if (formAction == "Add")
                        {
                            return RedirectToAction("Page", new
                            {
                                Id,
                                sequenceNo,
                                sectionNo,
                                pageId = pageAddResponse.Page.PageId,
                                __redirectAction,
                                __summaryLink
                            });
                        }

                        if (__redirectAction == "Feedback")
                            return RedirectToAction("Feedback", new { Id });

                        var nextAction = pageAddResponse.Page.Next.SingleOrDefault(x => x.Action == "NextPage");

                        if (!string.IsNullOrEmpty(nextAction.Action))
                            return RedirectToNextAction(Id, sequenceNo, sectionNo, nextAction.Action, nextAction.ReturnId, __redirectAction);
                    }
                    else if (page.PageOfAnswers?.Count > 0 && formAction != "Add")
                    {
                        if (page.HasFeedback && page.HasNewFeedback && !page.AllFeedbackIsCompleted && __redirectAction == "Feedback")
                        {
                            page = StoreEnteredAnswers(answers, page);
                            SetResponseValidationErrors(pageAddResponse?.ValidationErrors, page);
                            if (!page.AllFeedbackIsCompleted || pageAddResponse?.ValidationErrors.Count > 0)
                            {
                                return RedirectToAction("Page", new { Id, sequenceNo, sectionNo, pageId, __redirectAction, __summaryLink });
                            }
                            return RedirectToAction("Feedback", new { Id });
                        }
                        else if (pageAddResponse.ValidationErrors?.Count == 0 || answers.All(x => x.Value == string.Empty || Regex.IsMatch(x.Value, "^[,]+$")))
                        {
                            var nextAction = page.Next.SingleOrDefault(x => x.Action == "NextPage");

                            if (!string.IsNullOrEmpty(nextAction.Action))
                            {
                                if (__redirectAction == "Feedback")
                                {
                                    foreach (var answer in answers)
                                    {
                                        if (page.Next.Exists(y => y.Conditions.Exists(x => x.QuestionId == answer.QuestionId || x.QuestionTag == answer.QuestionId)))
                                        {
                                            return RedirectToNextAction(Id, sequenceNo, sectionNo, nextAction.Action, nextAction.ReturnId, __redirectAction, "Hide");
                                        }
                                        break;
                                    }

                                    return RedirectToAction("Feedback", new { Id });
                                }

                                return RedirectToNextAction(Id, sequenceNo, sectionNo, nextAction.Action, nextAction.ReturnId, __redirectAction);
                            }
                        }
                    }

                    page = StoreEnteredAnswers(answers, page);

                    SetResponseValidationErrors(pageAddResponse?.ValidationErrors, page);

                }
                else
                {
                    return BadRequest("Page is not of a type of Multiple Answers");
                }
                
                return RedirectToAction("Page", new { Id, sequenceNo, sectionNo, pageId, __redirectAction, __summaryLink });
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Could not find the page", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Applications");
                throw ex;
            }
        }

        [HttpPost("/Application/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}/Pages/{pageId}"), ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> SaveAnswers(Guid Id, int sequenceNo, int sectionNo, string pageId, string __redirectAction, string __summaryLink)
        { 
            var application = await _applicationApiClient.GetApplication(Id);

            if (!CanUpdateApplication(application, sequenceNo, sectionNo))
                return RedirectToAction("Sequence", new { Id, sequenceNo });

            try
            {
                var updatePageResult = default(SetPageAnswersResponse);
                var page = await _qnaApiClient.GetPageBySectionNo(application.ApplicationId, sequenceNo, sectionNo, pageId);
                var isFileUploadPage = page.Questions?.Any(q => q.Input.Type == "FileUpload");

                var answers = new List<Answer>();

                // NOTE: QnA API stipulates that a Page cannot contain a mixture of FileUploads and other Question Types
                if (isFileUploadPage == false)
                {
                    answers = GetAnswersFromForm(page);

                    if (__redirectAction == "Feedback" && !HasAtLeastOneAnswerChanged(page, answers) && !page.AllFeedbackIsCompleted)
                        SetAnswerNotUpdated(page);
                    else
                        updatePageResult = await _qnaApiClient.SetPageAnswers(application.ApplicationId, page.SectionId.Value, page.PageId, answers);
                }
                else if (isFileUploadPage == true)
                {
                    var errorMessages = new List<ValidationErrorDetail>();

                    answers = GetAnswersFromFiles();

                    if (answers.Count < 1)
                    {
                        // Nothing to upload
                        if (__redirectAction == "Feedback")
                        {
                            if (page.HasFeedback && page.HasNewFeedback && !page.AllFeedbackIsCompleted)
                            {
                                SetAnswerNotUpdated(page);
                                return RedirectToAction("Page", new { Id, sequenceNo, sectionNo, pageId, __redirectAction, __summaryLink });
                            }
                            else if (FileValidator.AllRequiredFilesArePresent(page, errorMessages, ModelState))
                            {
                                return RedirectToAction("Feedback", new { Id });
                            }
                        }
                        else if (FileValidator.AllRequiredFilesArePresent(page, errorMessages, ModelState))
                        {
                            return ForwardToNextSectionOrPage(page, Id, sequenceNo, sectionNo, __redirectAction);
                        }
                    }
                    else
                    {
                        if (FileValidator.FileValidationPassed(answers, page, errorMessages, ModelState, HttpContext.Request.Form.Files))
                            updatePageResult = await UploadFilesToStorage(application.ApplicationId, page.SectionId.Value, page.PageId, page);
                    }                  
                }

                var apiValidationResult = await _apiValidationService.CallApiValidation(page, answers);
                if (!apiValidationResult.IsValid)
                {
                    if (updatePageResult is null) updatePageResult = new SetPageAnswersResponse { ValidationPassed = false };

                    if (updatePageResult.ValidationErrors == null)
                    {
                        updatePageResult.ValidationErrors = new List<KeyValuePair<string, string>>();
                    }

                    updatePageResult.ValidationErrors.AddRange(apiValidationResult.ErrorMessages);
                }

                if (updatePageResult?.ValidationPassed == true)
                {
                    // TODO: Update section.AllRequestedFeedback to the value of that currently held in QnA
                    // applySection.RequestedFeedbackAnswered = qnaSection.QnAData.RequestedFeedbackAnswered

                    if (__redirectAction == "Feedback")
                    { 
                        foreach( var answer in answers)
                        {
                            if (page.Next.Exists(y => y.Conditions.Exists(x => x.QuestionId == answer.QuestionId || x.QuestionTag == answer.QuestionId)))
                            {
                                return RedirectToNextAction(Id, sequenceNo, sectionNo, updatePageResult.NextAction, updatePageResult.NextActionId, __redirectAction, "Hide");
                            }
                            break;
                        }

                        return RedirectToAction("Feedback", new { Id });
                    }

                    if (!string.IsNullOrEmpty(updatePageResult.NextAction))
                        return RedirectToNextAction(Id, sequenceNo, sectionNo, updatePageResult.NextAction, updatePageResult.NextActionId, __redirectAction);
                }

                if (isFileUploadPage != true)
                {
                    page = StoreEnteredAnswers(answers, page);
                }

                SetResponseValidationErrors(updatePageResult?.ValidationErrors, page);

                return RedirectToAction("Page", new { Id, sequenceNo, sectionNo, pageId, __redirectAction, __summaryLink });
            }
            catch (Exception ex)
            {  
                if (ex.Message.Equals("Could not find the page", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Applications");

                _logger.LogError(ex, ex.Message);

                throw ex;
            }
        }


        [HttpPost("/Application/{Id}/RefreshApplicationData")]
        public async Task<IActionResult> RefreshApplicationData(Guid Id)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);

            if(applicationData != null)
            {
                var companyDetails = !string.IsNullOrWhiteSpace(applicationData.CompanySummary?.CompanyNumber) ? await _orgApiClient.GetCompanyDetails(applicationData.CompanySummary.CompanyNumber) : null;
                var charityDetails = int.TryParse(applicationData.CharitySummary?.CharityNumber, out var charityNumber) ? await _orgApiClient.GetCharityDetails(charityNumber) : null;

                applicationData.CompanySummary = Mapper.Map<CompaniesHouseSummary>(companyDetails);
                applicationData.CharitySummary = Mapper.Map<CharityCommissionSummary>(charityDetails);

                await _qnaApiClient.UpdateApplicationData(application.ApplicationId, applicationData);
            }

            return RedirectToAction("SequenceSignPost", new { Id });
        }

        [HttpPost("/Application/DeleteAnswer")]
        public async Task<IActionResult> DeleteAnswer(Guid Id, int sequenceNo, int sectionNo, string pageId, Guid answerId, string __redirectAction, string __summaryLink = "False")
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo, sectionNo))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo });
            }

            var page = await _qnaApiClient.GetPageBySectionNo(application.ApplicationId, sequenceNo, sectionNo, pageId);
            try
            {
                await _qnaApiClient.RemovePageAnswer(application.ApplicationId, page.SectionId.Value, page.PageId, answerId);
            }
            catch (HttpRequestException ex) {
                _logger.LogError($"Page answer removal errored : {ex} ");
            }

            return RedirectToAction("Page", new { Id, sequenceNo, sectionNo, pageId, __redirectAction, __summaryLink });
        }

        [HttpGet("Application/{Id}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        public async Task<IActionResult> Download(Guid Id, Guid sectionId, string pageId, string questionId, string filename)
        {
            var application = await _applicationApiClient.GetApplication(Id);

            var response = await _qnaApiClient.DownloadFile(application.ApplicationId, sectionId, pageId, questionId, filename);

            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);

        }

        [HttpGet("Application/{Id}/SequenceNo/{sequenceNo}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/Filename/{filename}/RedirectAction/{__redirectAction}")]
        public async Task<IActionResult> DeleteFile(Guid Id, int sequenceNo, Guid sectionId, string pageId, string questionId, string filename, string __redirectAction)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var section = await _qnaApiClient.GetSection(application.ApplicationId, sectionId);

            if (section is null || !CanUpdateApplication(application, sequenceNo, section.SectionNo))
            {
                return RedirectToAction("Sequence", new { Id, sequenceNo });
            }

            await _qnaApiClient.DeleteFile(application.ApplicationId, section.Id, pageId, questionId, filename);

            return RedirectToAction("Page", new { Id, sequenceNo, section.SectionNo, pageId, __redirectAction });
        }

        [HttpPost("/Application/{Id}/Submit/{sequenceNo}")]
        public async Task<IActionResult> Submit(Guid Id, int sequenceNo)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            if (!CanUpdateApplication(application, sequenceNo))
            {
                return RedirectToAction("Applications");
            }

            var sequence = await _qnaApiClient.GetSequenceBySequenceNo(application.ApplicationId, sequenceNo);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequence.SequenceNo);
            var applySections = applySequence.Sections;

            var errors =  ValidateSubmit(sections, applySections);
            if (errors.Any())
            {
                var applyData = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
                var sequenceVm = new SequenceViewModel(sequence, Id, BuildPageContext(application,sequence),sections,applyData.Sections, errors);

                if (applyData.Status == ApplicationSequenceStatus.FeedbackAdded)
                {
                    return View("~/Views/Application/Feedback.cshtml", sequenceVm);
                }
                else
                {
                    return View("~/Views/Application/Sequence.cshtml", sequenceVm);
                }
            }
      
            var dictRequestedFeedbackAnswered = sections.Select(t => new { t.SectionNo, t.QnAData.RequestedFeedbackAnswered })
               .ToDictionary(t => t.SectionNo, t => t.RequestedFeedbackAnswered);

            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await GetUserContact(signinId);
            var submitRequest = BuildSubmitApplicationSequenceRequest(application.Id, dictRequestedFeedbackAnswered,_config.ReferenceFormat, sequence.SequenceNo, contact.Id);

            if (await _applicationApiClient.SubmitApplicationSequence(submitRequest))
            {
                await _qnaApiClient.AllFeedbackCompleted(application.ApplicationId, sequence.Id);
                return RedirectToAction("Submitted", new { Id });
            }

            return RedirectToAction("NotSubmitted", new { Id });
        }

        [HttpGet("/Application/{Id}/Submitted")]
        public async Task<IActionResult> Submitted(Guid Id)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            return View("~/Views/Application/Submitted.cshtml", new SubmittedViewModel
            {
                ReferenceNumber = application?.ApplyData?.Apply?.ReferenceNumber,
                FeedbackUrl = _config.FeedbackUrl,
                StandardName = application?.ApplyData?.Apply?.StandardName
            });
        }

        [HttpGet("/Application/{Id}/NotSubmitted")]
        public async Task<IActionResult> NotSubmitted(Guid Id)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            return View("~/Views/Application/NotSubmitted.cshtml", new SubmittedViewModel
            {
                ReferenceNumber = application?.ApplyData?.Apply?.ReferenceNumber,
                FeedbackUrl = _config.FeedbackUrl,
                StandardName = application?.ApplyData?.Apply?.StandardName
            });
        }

        [HttpGet("/Application/{id}/Feedback")]
        public async Task<IActionResult> Feedback(Guid id)
        {
            var application = await _applicationApiClient.GetApplication(id);
            var allApplicationSequences = await _qnaApiClient.GetAllApplicationSequences(application.ApplicationId);
            var sequenceNo = application.ApplyData.Sequences.SingleOrDefault(x => x.IsActive && x.Status == ApplicationSequenceStatus.FeedbackAdded)?.SequenceNo;

            if(sequenceNo == null )
                return RedirectToAction("Applications");

            var sequence = allApplicationSequences.Single(x => x.SequenceNo == sequenceNo);

            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            var applyData = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequence.SequenceNo);

            var sequenceVm = new SequenceViewModel(sequence, application.Id, BuildPageContext(application, sequence), sections, applyData.Sections, null);

            return View("~/Views/Application/Feedback.cshtml", sequenceVm);
        }

        private async Task<Page> GetDataFedOptions(Page page)
        {
            if (page != null)
            {
                foreach (var question in page.Questions)
                {
                    if (question.Input.Type.StartsWith("DataFed_"))
                    {
                        // Get data from API using question.Input.DataEndpoint
                        // var questionOptions = await _applicationApiClient.GetQuestionDataFedOptions();

                        // NOTE: For now it seems the only DataFed type is delivery areas and someone has coded it that way in the api client
                        var deliveryAreas = await _applicationApiClient.GetQuestionDataFedOptions();
                        var questionOptions = deliveryAreas.Select(da => new Option() { Label = da.Area, Value = da.Area }).ToList();

                        question.Input.Options = questionOptions;
                        question.Input.Type = question.Input.Type.Replace("DataFed_", "");
                    }
                }
            }

            return page;
        }


        private void SetAnswerNotUpdated(Page page)
        {
            var validationErrors = new List<KeyValuePair<string, string>>();
            var updatePageResult = new SetPageAnswersResponse();
            foreach (var question in page.Questions)
                validationErrors.Add(new KeyValuePair<string, string>(question.QuestionId, "Unable to save as you have not updated your answer"));

            updatePageResult.ValidationPassed = false;
            updatePageResult.ValidationErrors = validationErrors;
            SetResponseValidationErrors(updatePageResult?.ValidationErrors, page);
        }

        private bool HasAtLeastOneAnswerChanged(Page page,List<Answer> answers)
        {
            _logger.LogInformation($"HasAtLeastOneAnswerChanged -> Is page null? {(page == null ? "Yes": "No")}");
            _logger.LogInformation($"HasAtLeastOneAnswerChanged -> page.Questions null? {(page.Questions == null ? "Yes": "No")}");

            foreach (var pageQuestion in page.Questions)
            {
                _logger.LogInformation($"HasAtLeastOneAnswerChanged -> page.Question.Id {pageQuestion.QuestionId} Input null? {(pageQuestion.Input == null ? "Yes": "No")}");
                _logger.LogInformation($"HasAtLeastOneAnswerChanged -> page.Question.Id {pageQuestion.QuestionId} Input.Type null? {(pageQuestion.Input.Type == null ? "Yes": "No")}");
            }
            
            _logger.LogInformation($"HasAtLeastOneAnswerChanged -> Checks ok.  Page JSON: {JsonConvert.SerializeObject(page)}");
            
            var atLeastOneAnswerChanged = page.Questions.Any(q => q.Input.Type == "FileUpload" );

            foreach (var question in page.Questions)
            {
                var answer = answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                var existingAnswer = page.PageOfAnswers.SelectMany(poa => poa.Answers).FirstOrDefault(a => a.QuestionId == question.QuestionId);

                atLeastOneAnswerChanged = atLeastOneAnswerChanged
                    ? true
                    : !answer?.Value.Equals(existingAnswer?.Value, StringComparison.OrdinalIgnoreCase) ?? answer != existingAnswer;

                if (question.Input.Options != null)
                {
                    foreach (var option in question.Input.Options)
                    {
                        if (answer?.Value == option.Value.ToString())
                        {
                            if (option.FurtherQuestions != null)
                            {
                                var atLeastOneFutherQuestionAnswerChanged = page.Questions.Any(q => q.Input.Type == "FileUpload");

                                foreach (var furtherQuestion in option.FurtherQuestions)
                                {
                                    var furtherAnswer = answers.FirstOrDefault(a => a.QuestionId == furtherQuestion.QuestionId);
                                    var existingFutherAnswer = page.PageOfAnswers.SelectMany(poa => poa.Answers).FirstOrDefault(a => a.QuestionId == furtherQuestion.QuestionId);

                                    atLeastOneFutherQuestionAnswerChanged = atLeastOneFutherQuestionAnswerChanged
                                        ? true
                                        : !furtherAnswer?.Value.Equals(existingFutherAnswer?.Value, StringComparison.OrdinalIgnoreCase) ?? furtherAnswer != existingFutherAnswer;
                                }

                                atLeastOneAnswerChanged = atLeastOneAnswerChanged ? true : atLeastOneFutherQuestionAnswerChanged;
                            }
                        }
                    }
                }
            }

            return atLeastOneAnswerChanged;
        }

        private RedirectToActionResult ForwardToNextSectionOrPage(Page page, Guid Id, int sequenceNo, int sectionNo, string __redirectAction)
        {
            var next = page.Next.FirstOrDefault(x => x.Action == "NextPage");
            if (next != null)
                return RedirectToNextAction(Id, sequenceNo, sectionNo, next.Action, next.ReturnId, __redirectAction);
            return RedirectToAction("Section", new { Id, sequenceNo, sectionNo });
        }

        private static string BuildPageContext(ApplicationResponse application, QnA.Api.Types.Sequence sequence)
        {
            string pageContext = string.Empty;
            if (sequence.SequenceNo == 2)
            {
                pageContext = $"{application?.ApplyData?.Apply?.StandardReference } {application?.ApplyData?.Apply?.StandardName}";
            }
            return pageContext;
        }

        private static Page StoreEnteredAnswers(List<Answer> answers, Page page)
        {
            if (answers != null && answers.Any())
            {
                if (page.PageOfAnswers is null || !page.PageOfAnswers.Any())
                {
                    page.PageOfAnswers = new List<PageOfAnswers> { new PageOfAnswers { Answers = new List<Answer>() } };
                }

                page.PageOfAnswers.Add(new PageOfAnswers { Answers = answers });
            }

            return page;
        }


        private RedirectToActionResult RedirectToNextAction(Guid Id, int sequenceNo, int sectionNo, string nextAction, string nextActionId, string __redirectAction, string __summaryLink = "Show")
        {
            if (nextAction == "NextPage")
            {
                return RedirectToAction("Page", new
                {
                    Id,
                    sequenceNo,
                    sectionNo,
                    pageId = nextActionId,
                    __redirectAction,
                    __summaryLink
                });
            }

            return nextAction == "ReturnToSection"
                ? RedirectToAction("Section", "Application", new { Id, sequenceNo, sectionNo })
                : RedirectToAction("Sequence", "Application", new { Id, sequenceNo });
        }

        private void SetResponseValidationErrors(List<KeyValuePair<string, string>> validationErrors, Page page)
        {
            if (validationErrors != null)
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            TempData["InvalidPage"] = JsonConvert.SerializeObject(page);
        }

        private async Task<SetPageAnswersResponse> UploadFilesToStorage(Guid applicationId, Guid sectionId, string pageId, Page page)
        {
            SetPageAnswersResponse response = new SetPageAnswersResponse(null);
            if (HttpContext.Request.Form.Files.Any() && page != null)
                response = await _qnaApiClient.Upload(applicationId, sectionId, pageId, HttpContext.Request.Form.Files);
            return response;
        }

        private List<Answer> GetAnswersFromForm(Page page)
        {
            List<Answer> answers = new List<Answer>();

            // These are special in that they drive other things and thus should not be deemed as an answer
            var excludedInputs = new List<string> { "formAction", "postcodeSearch", "checkAll" };

            // Add answers from the Form post
            foreach (var keyValuePair in HttpContext.Request.Form.Where(f => !f.Key.StartsWith("__") && !excludedInputs.Contains(f.Key, StringComparer.InvariantCultureIgnoreCase)))
            {
                answers.Add(new Answer() { QuestionId = keyValuePair.Key, Value = keyValuePair.Value });
            }

            // Check if any Page Question is missing and add the default answer
            foreach (var questionId in page.Questions.Select(q => q.QuestionId))
            {
                if(!answers.Any(a => a.QuestionId == questionId))
                {
                    // Add default answer if it's missing
                    answers.Add(new Answer { QuestionId = questionId, Value = string.Empty });
                }
            }

            #region FurtherQuestion_Processing
            // Get all questions that have FurtherQuestions in a ComplexRadio
            var questionsWithFutherQuestions = page.Questions.Where(x => x.Input.Type == "ComplexRadio" && x.Input.Options != null && x.Input.Options.Any(o => o.FurtherQuestions != null && o.FurtherQuestions.Any()));

            foreach (var question in questionsWithFutherQuestions)
            {
                var answerForQuestion = answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);

                // Remove FurtherQuestion answers to all other Options as they were not selected and thus should not be stored
                foreach (var furtherQuestion in question.Input.Options.Where(opt => opt.Value != answerForQuestion?.Value && opt.FurtherQuestions != null).SelectMany(opt => opt.FurtherQuestions))
                {
                    foreach (var answer in answers.Where(a => a.QuestionId == furtherQuestion.QuestionId))
                    {
                        answer.Value = string.Empty;
                    }
                }
            }
            #endregion FurtherQuestion_Processing

            // Address inputs require special processing
            if (page.Questions.Any(x => x.Input.Type == "Address"))
            {
                answers = ProcessPageVmQuestionsForAddress(page, answers);
            }

            return answers;
        }

        private List<Answer> GetAnswersFromFiles()
        {
            List<Answer> answers = new List<Answer>();

            // Add answers from the Files sent within the Form post
            if (HttpContext.Request.Form.Files != null)
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    answers.Add(new Answer() { QuestionId = file.Name, Value = file.FileName });
                }

            }

            return answers;
        }

        private static List<Answer> ProcessPageVmQuestionsForAddress(Page page, List<Answer> answers)
        {

            if (page.Questions.Any(x => x.Input.Type == "Address"))
            {
                Dictionary<string, JObject> answerValueDictionary = new Dictionary<string, JObject>();

                // Address input fields will contain _Key_
                foreach (var formVariable in answers.Where(x => x.QuestionId.Contains("_Key_")))
                {
                    var answerKey = formVariable.QuestionId.Split("_Key_");
                    if (!answerValueDictionary.ContainsKey(answerKey[0]))
                    {
                        answerValueDictionary.Add(answerKey[0], new JObject());
                    }

                    answerValueDictionary[answerKey[0]].Add(
                        answerKey.Count() == 1 ? string.Empty : answerKey[1],
                        formVariable.Value.ToString());
                }

                // Remove anything that contains _Key_ as it has now been processed correctly
                answers = answers.Where(x => !x.QuestionId.Contains("_Key_")).ToList();

                foreach (var answerValue in answerValueDictionary)
                {
                    if (answerValue.Value.Count > 1)
                    {
                        var answer = answers.FirstOrDefault(a => a.QuestionId == answerValue.Key);

                        if (answer is null)
                        {
                            answers.Add(new Answer() { QuestionId = answerValue.Key, Value = answerValue.Value.ToString() });
                        }
                        else
                        {
                            answer.Value = answerValue.Value.ToString();
                        }
                    }
                }

            }

            return answers;
        }


        private static void ProcessPageVmQuestionsForStandardName(List<QuestionViewModel> pageVmQuestions, ApplicationResponse application)
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

            var standardName = application?.ApplyData?.Apply?.StandardName;

            if (string.IsNullOrEmpty(standardName)) standardName = "the standard to be selected";

            foreach (var question in pageVmQuestions)
            {
                question.Label = question.Label?.Replace($"[{placeholderString}]", standardName);
                question.Hint = question.Hint?.Replace($"[{placeholderString}]", standardName);
                question.QuestionBodyText = question.QuestionBodyText?.Replace($"[{placeholderString}]", standardName);
                question.ShortLabel = question.Label?.Replace($"[{placeholderString}]", standardName);
            }
        }

        private static SubmitApplicationSequenceRequest BuildSubmitApplicationSequenceRequest(Guid applicationId, Dictionary<int,bool?> dictRequestedFeedbackAnswered, string referenceFormat, int sequenceNo, Guid userId)
        {  
            return new SubmitApplicationSequenceRequest
            {
                ApplicationId = applicationId,
                ApplicationReferenceFormat = referenceFormat,
                SequenceNo = sequenceNo,
                SubmittingContactId = userId,
                RequestedFeedbackAnswered = dictRequestedFeedbackAnswered
            };
        }

        private static CreateApplicationRequest BuildCreateApplicationRequest(Guid qnaApplicationId, ContactResponse contact, OrganisationResponse org,
           string referenceFormat, List<QnA.Api.Types.Sequence> sequences, List<List<Section>> sections)
        {

            return new CreateApplicationRequest
            {
                QnaApplicationId = qnaApplicationId,
                OrganisationId = org.Id,
                ApplicationReferenceFormat = referenceFormat,
                CreatingContactId = contact.Id,
                ApplySequences = sequences.Select(sequence => new ApplySequence
                {
                    SequenceId = sequence.Id,
                    Sections = sections.SelectMany(y => y.Where(x => x.SequenceNo == sequence.SequenceNo).Select(x => new ApplySection
                    {
                        SectionId = x.Id,
                        SectionNo = x.SectionNo,
                        Status = ApplicationSectionStatus.Draft,
                        RequestedFeedbackAnswered = x.QnAData.RequestedFeedbackAnswered
                    })).ToList(),
                    Status = ApplicationSequenceStatus.Draft,
                    IsActive = sequence.IsActive,
                    SequenceNo = sequence.SequenceNo,
                    NotRequired = sequence.NotRequired
                }).ToList()
            };
        }



        private static List<ValidationErrorDetail> ValidateSubmit(List<Section> qnaSections, List<ApplySection> applySections)
        {
            var validationErrors = new List<ValidationErrorDetail>();

            var sections = qnaSections?.Where(x => !applySections?.Any(p => p.SectionNo == x.SectionNo && p.NotRequired)??false).ToList();

            if (sections is null )
            {
                var validationError = new ValidationErrorDetail(string.Empty, $"Cannot submit empty sequence");
                validationErrors.Add(validationError);
            }
            else if (sections.Any(sec => sec.QnAData.Pages.Count(x => x.Complete) != sec.QnAData.Pages.Count(x => x.Active)))
            {
                foreach (var sectionQuestionsNotYetCompleted in sections.Where(sec => sec.QnAData.Pages.Count(x => x.Complete) != sec.QnAData.Pages.Count(x => x.Active)))
                {
                    var validationError = new ValidationErrorDetail(sectionQuestionsNotYetCompleted.Id.ToString(), $"You need to complete the '{sectionQuestionsNotYetCompleted.LinkTitle}' section");
                    validationErrors.Add(validationError);
                }
            }
            else if (sections.Any(sec => sec.QnAData.RequestedFeedbackAnswered is false || sec.QnAData.Pages.Any(p => !p.AllFeedbackIsCompleted)))
            {
                foreach (var sectionFeedbackNotYetCompleted in sections.Where(sec => sec.QnAData.RequestedFeedbackAnswered is false || sec.QnAData.Pages.Any(p => !p.AllFeedbackIsCompleted)))
                {
                    var validationError = new ValidationErrorDetail(sectionFeedbackNotYetCompleted.Id.ToString(), $"You need to complete the '{sectionFeedbackNotYetCompleted.LinkTitle}' section");
                    validationErrors.Add(validationError);
                }
            }

            return validationErrors;
        }

        private async Task<Guid> GetUserId()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await GetUserContact(signinId);

            return contact?.Id ?? Guid.Empty;
        }

        private async Task<ContactResponse> GetUserContact(string signinId)
        {
            return await _contactsApiClient.GetContactBySignInId(signinId);
        }

        private static bool CanUpdateApplication(ApplicationResponse application, int sequenceNo, int? sectionNo = null)
        {
            bool canUpdate = false;

            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress, ApplicationStatus.FeedbackAdded };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft, ApplicationSequenceStatus.FeedbackAdded };

            if (application != null && application.ApplyData != null && validApplicationStatuses.Contains(application.ApplicationStatus))
            {
                var sequence = application.ApplyData.Sequences?.FirstOrDefault(seq => !seq.NotRequired && seq.IsActive && seq.SequenceNo == sequenceNo);

                if (sequence != null && validApplicationSequenceStatuses.Contains(sequence.Status))
                {
                    if (sectionNo.HasValue)
                    {
                        var section = sequence.Sections.FirstOrDefault(sec => !sec.NotRequired && sec.SectionNo == sectionNo);

                        if (section != null)
                        {
                            canUpdate = true;
                        }
                    }
                    else
                    {
                        // No need to check the section
                        canUpdate = true;
                    }
                }
            }

            return canUpdate;
        }

        //Todo: Remove this function if and when the _Address.cshtml is refactored or the qna modelstate 
        //reflects the keys that are set in the _Address.cshtml. Currently the ValidationErrorDetailTagHelper will not 
        //update the address fields because of the keys mismatch.
        private static void UpdateValidationDetailsForAddress(Page page, List<ValidationErrorDetail> errorMessages)
        {
            var question = page.Questions.SingleOrDefault(x => x.Input.Type == "Address");
            if (question != null)
            {
                foreach (var error in errorMessages)
                {
                    switch (error.ErrorMessage)
                    {
                        case "Enter building and street":
                            error.Field = $"{question.QuestionId}_Key_AddressLine1";
                            break;
                        case "Enter town or city":
                            error.Field = $"{question.QuestionId}_Key_AddressLine3";
                            break;
                        case "Enter postcode":
                            error.Field = $"{question.QuestionId}_Key_Postcode";
                            break;
                    }
                }
            }

        }
    }
}