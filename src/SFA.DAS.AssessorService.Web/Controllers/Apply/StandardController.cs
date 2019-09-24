using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class StandardController : Controller
    {
        private readonly IApplicationApiClient _apiClient;
        private readonly ISessionService _sessionService;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IWebConfiguration _config;

        public StandardController(IApplicationApiClient apiClient, IQnaApiClient qnaApiClient, IContactsApiClient contactsApiClient, ISessionService sessionService, IWebConfiguration config)
        {
            _apiClient = apiClient;
            _sessionService = sessionService;
            _qnaApiClient = qnaApiClient;
            _contactsApiClient = contactsApiClient;
            _config = config; 
           
        }

        [HttpGet("Standard/{id}")]
        public IActionResult Index(Guid id)
        {
            var standardViewModel = new StandardViewModel { Id = id };
            return View("~/Views/Application/Standard/FindStandard.cshtml", standardViewModel);
        }

        [HttpPost("Standard/{id}")]
        public async Task<IActionResult> Search(StandardViewModel model)
        {
            if (string.IsNullOrEmpty(model.StandardToFind) || model.StandardToFind.Length <= 2)
            {
                ModelState.AddModelError(nameof(model.StandardToFind), "Enter a valid search string (more than 2 characters)");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index), new { model.Id });
            }

            var results = await _apiClient.GetStandards();

            model.Results = results.Where(r => r.Title.ToLower().Contains(model.StandardToFind.ToLower())).ToList();

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> StandardConfirm(Guid id, int standardCode)
        {
            var application = await _apiClient.GetApplication(id);
            var standardViewModel = new StandardViewModel { Id = id, StandardCode = standardCode};
            var results = await _apiClient.GetStandards();
            standardViewModel.SelectedStandard = results.FirstOrDefault(r => r.StandardId == standardCode);
            standardViewModel.ApplicationStatus = application.ApplyData.Apply.StandardCode == standardCode ? application.ApplicationStatus : string.Empty;
            return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
        }

        [HttpPost("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> StandardConfirm(StandardViewModel model, Guid id, int standardCode)
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await GetUserContact(signinId);
            var application = await _apiClient.GetApplication(id);
            var results = await _apiClient.GetStandards();
            model.SelectedStandard = results.FirstOrDefault(r => r.StandardId == standardCode);
            
            model.ApplicationStatus = application.ApplyData.Apply.StandardCode == standardCode ? application.ApplicationStatus : string.Empty;

            if (!model.IsConfirmed)
            {
                ModelState.AddModelError(nameof(model.IsConfirmed), "Please tick to confirm");
                TempData["ShowErrors"] = true;
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }

            if (!string.IsNullOrEmpty(model.ApplicationStatus))
            {
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }

            var allApplicationSequences = await _qnaApiClient.GetAllApplicationSequences(application.ApplicationId);
            var sequence = allApplicationSequences.Single(x => x.SequenceNo == 2);

            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            if (await _apiClient.Submit(BuildApplicationRequest(id, contact.Id,
               _config.ReferenceFormat, contact.GivenNames,
               contact?.Email,
               standardCode,
               model.SelectedStandard?.ReferenceNumber,
               model.SelectedStandard.Title,
               ApplicationStatus.InProgress,
               ApplicationSequenceStatus.Draft,
               sequence, sections)))
            {
                return RedirectToAction("Applications", "Application");
            }

            throw new BadRequestException("Failed to start application stage2 apply process.");
        }

        private SubmitApplicationRequest BuildApplicationRequest(Guid id, Guid userId,
          string referenceFormat, string contactName, string email, int standardCode,
          string standardReference, string standardName, string applicationStatus, string sectionSequenceStatus,
          Sequence sequence, List<Section> sections)
        {
            var applySections = sections.Select(x => new ApplySection
            {
                SectionId = x.Id,
                SectionNo = x.SectionNo,
                Status = sectionSequenceStatus,
                RequestedFeedbackAnswered = x.QnAData.RequestedFeedbackAnswered
            }).ToList();

            return new SubmitApplicationRequest
            {
                ApplicationId = id,
                ReferenceFormat = referenceFormat,
                ContactName = contactName,
                StandardCode = standardCode,
                StandardReference = standardReference,
                StandardName = standardName,
                ApplicationStatus = applicationStatus,
                Email = email,
                UserId = userId,
                Sequence = new ApplySequence
                {
                    SequenceId = sequence.Id,
                    Sections = applySections,
                    Status = sectionSequenceStatus,
                    IsActive = sequence.IsActive,
                    SequenceNo = sequence.SequenceNo,
                    NotRequired = sequence.NotRequired
                }
            };
        }

        private async Task<ContactResponse> GetUserContact(string signinId)
        {
            return await _contactsApiClient.GetContactBySignInId(signinId);
        }
    }
}