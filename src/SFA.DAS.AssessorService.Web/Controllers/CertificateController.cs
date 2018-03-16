using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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

        public CertificateController(ILogger<CertificateController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateViewModel vm)
        {
            vm.SetUpGrades();
            // Call API to create or return current certificate
            if (vm.Uln == 1111111111)
            {
                vm.Id = Guid.NewGuid();
                vm.SelectedGrade = "Credit";
                vm.GivenNames = "Tony";
                vm.FamilyName = "Stark";
                vm.Standard = "Robotics";
            }
            else
            {
                vm.Id = Guid.NewGuid();
                vm.SelectedGrade = "";
                vm.GivenNames = "Peter";
                vm.FamilyName = "Parker";
                vm.Standard = "Photography";
            }
            TempData.Put("Certificate", vm);
            return RedirectToAction("Grade", "Certificate");
        }

        [HttpGet]
        public async Task<IActionResult> Grade()
        {
            var vm = TempData.Get<CertificateViewModel>("Certificate");

            if (vm == null)
            {
                return RedirectToAction("Index", "Search");
            }

            return View(vm);
        }
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

        public Guid Id { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string SelectedGrade { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public string Standard { get; set; }
    }
}