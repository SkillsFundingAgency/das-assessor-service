using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Utils;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class CertificateController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public CertificateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateStartViewModel vm)
        {
            _contextAccessor.HttpContext.Session.Remove("CertificateSession");

            // Call API to create or return current certificate
            var certificateIdFromTheApi = Guid.NewGuid();

            _contextAccessor.HttpContext.Session.SetString("CertificateSession",
                JsonConvert.SerializeObject(new CertificateSession()
                {
                    CertificateId = certificateIdFromTheApi,
                    Uln = vm.Uln,
                    StandardCode = vm.StandardCode
                }));

            return RedirectToAction("Grade", "Certificate");
        }

        [HttpGet]
        public async Task<IActionResult> Grade()
        {
            var sessionString = _contextAccessor.HttpContext.Session.GetString("CertificateId");
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            // Call API to get certificate for this ID.

            var certificateGradeViewModel = new CertificateGradeViewModel();

            certificateGradeViewModel.SetUpGrades();
            certificateGradeViewModel.Id = certSession.CertificateId;
            certificateGradeViewModel.SelectedGrade = "Credit";
            certificateGradeViewModel.GivenNames = "Tony";
            certificateGradeViewModel.FamilyName = "Stark";
            certificateGradeViewModel.Standard = "Robotics";

            return View(certificateGradeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Grade(CertificateGradeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.SetUpGrades();
                return View(vm);
            }
            // Send the grade to the API for this cert.  Returns the current Cert


            TempData.Put("Certificate", vm);
            return RedirectToAction("Options");
        }

        [HttpGet]
        public async Task<IActionResult> Options()
        {
            var vm = TempData.Get<CertificateViewModel>("Certificate");

            if (vm == null)
            {
                return RedirectToAction("Index", "Search");
            }

            return View();
        }
    }

    public class CertificateSession
    {
        public Guid CertificateId { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
    }

    public class CertificateStartViewModel
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
    }


    public class CertificateGradeViewModel
    {
        public void SetUpGrades()
        {
            Grades = new List<SelectListItem>
            {
                new SelectListItem {Text = "Pass", Value = "Pass"},
                new SelectListItem {Text = "Credit", Value = "Credit"},
                new SelectListItem {Text = "Merit", Value = "Merit"},
                new SelectListItem {Text = "Distinction", Value = "Distinction"},
                new SelectListItem {Text = "Pass with excellence", Value = "Pass with excellence"},
                new SelectListItem {Text = "No grade awarded", Value = "No grade awarded"}
            };
        }

        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string SelectedGrade { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public string Standard { get; set; }
    }



    public class CertificateViewModel
    {

        public void SetUpGrades()
        {
            Grades = new List<SelectListItem>
            {
                new SelectListItem {Text = "Pass", Value = "Pass"},
                new SelectListItem {Text = "Credit", Value = "Credit"},
                new SelectListItem {Text = "Merit", Value = "Merit"},
                new SelectListItem {Text = "Distinction", Value = "Distinction"},
                new SelectListItem {Text = "Pass with excellence", Value = "Pass with excellence"},
                new SelectListItem {Text = "No grade awarded", Value = "No grade awarded"}
            };
        }

        public void SetUpOptions()
        {
            Options = new List<SelectListItem>
            {
                new SelectListItem {Text = "Yes", Value = "True"},
                new SelectListItem {Text = "No", Value = "False"},
            };
        }

        public Guid Id { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string SelectedGrade { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public bool HasAdditionalLearningOptions { get; set; }
        public List<SelectListItem> Options { get; set; }
        public string Option { get; set; }


        public string Standard { get; set; }
    }
}