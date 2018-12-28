using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;
using AutoMapper;
using Newtonsoft.Json;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{  
    [Authorize]
    public class CertificateApprovalsController : CertificateBaseController
    {
        public CertificateApprovalsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
        }
        

        [HttpGet]
        public async Task<IActionResult> New()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesToBeApproved = new CertificateApprovalViewModel
            {
                ToBeApprovedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.ToBeApproved && q.PrivatelyFundedStatus == null))
            };

            return View(certificatesToBeApproved);
        }

        [HttpGet]
        public async Task<IActionResult> SentForApproval()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesSentForApproval = new CertificateApprovalViewModel
            {
                SentForApprovalCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.ToBeApproved &&
                                q.PrivatelyFundedStatus == CertificateStatus.SentForApproval)),

            };

            return View(certificatesSentForApproval);
        }

        [HttpGet]
        public async Task<IActionResult> Approved()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesApproved = new CertificateApprovalViewModel
            {

                ApprovedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.Submitted && q.PrivatelyFundedStatus == CertificateStatus.Approved))
            };

            return View(certificatesApproved);
        }

        [HttpGet]
        public async Task<IActionResult> Rejected()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesThatAreRejected = new CertificateApprovalViewModel
            {

                RejectedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.Draft && q.PrivatelyFundedStatus == CertificateStatus.Rejected))
            };

            return View(certificatesThatAreRejected);
        }

        [HttpPost(Name = "Approvals")]
        public async Task<IActionResult> Approvals(
            CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            certificateApprovalViewModel.UserName = ContextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            foreach (var approvalResult in certificateApprovalViewModel.ApprovalResults)
            {
                switch (approvalResult.IsApproved)
                {
                    case CertificateStatus.ToBeApproved when approvalResult.PrivatelyFundedStatus == null:
                        approvalResult.PrivatelyFundedStatus = CertificateStatus.SentForApproval;
                        break;
                    case CertificateStatus.ToBeApproved when approvalResult.PrivatelyFundedStatus == CertificateStatus.SentForApproval:
                        approvalResult.IsApproved = CertificateStatus.Submitted;
                        approvalResult.PrivatelyFundedStatus = CertificateStatus.Approved;
                        break;
                    case CertificateStatus.Draft when (approvalResult.PrivatelyFundedStatus == null || 
                                                       approvalResult.PrivatelyFundedStatus == CertificateStatus.SentForApproval):
                        approvalResult.PrivatelyFundedStatus = CertificateStatus.Rejected;
                        break;
                    case CertificateStatus.Submitted when (approvalResult.PrivatelyFundedStatus == CertificateStatus.Rejected):
                        approvalResult.PrivatelyFundedStatus = CertificateStatus.Approved;
                        break;
                }
            }

            var match = certificateApprovalViewModel.ApprovalResults.Where(x =>
                x.IsApproved == CertificateStatus.Approved || x.IsApproved == CertificateStatus.ToBeApproved || x.IsApproved == CertificateStatus.Submitted);
            if (!match.Any())
                certificateApprovalViewModel.ActionHint = CertificateStatus.Rejected;

           await ApiClient.ApproveCertificates(certificateApprovalViewModel);
            return RedirectToAction(certificateApprovalViewModel.ActionHint);
        }


        [HttpGet]
        public async Task<FileContentResult> ExportSentForApproval()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();

            var data = certificates?.Where(q => q.Status == CertificateStatus.ToBeApproved &&
                                                q.PrivatelyFundedStatus == CertificateStatus.SentForApproval).Select(ToDictionary<object>);
          
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(ToDataTable(data), true);

                return File(package.GetAsByteArray(), "application/excel", $"SentForApproval.xlsx");
            }
        }

        private static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }

        private static DataTable ToDataTable(IEnumerable<IDictionary<string, object>> list)
        {
            var dataTable = new DataTable();

            if (list != null || list.Any())
            {
                var columnNames = list.SelectMany(dict => dict.Keys).Distinct();
                dataTable.Columns.AddRange(columnNames.Select(col => new DataColumn(col)).ToArray());

                foreach (var item in list)
                {
                    var row = dataTable.NewRow();
                    foreach (var key in item.Keys)
                    {
                        row[key] = item[key];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
    }
}