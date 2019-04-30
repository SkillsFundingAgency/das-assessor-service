﻿using System;
using System.Collections.Generic;
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
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Domain.Paging;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{  
    [Authorize]
    public class CertificateApprovalsController : CertificateBaseController
    {
        private const int PageSize = 10;
        public CertificateApprovalsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
            
        }
        

        [HttpGet]
        public async Task<IActionResult> New(int? pageIndex)
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved(0, pageIndex ?? 1, CertificateStatus.ToBeApproved,null);
            var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items.OrderByDescending(x => x.CreatedDay));
            var certificatesToBeApproved = new CertificateApprovalViewModel
            {
                ToBeApprovedCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items,certificates.TotalRecordCount,certificates.PageIndex,certificates.PageSize)
            };

            return View(certificatesToBeApproved);
        }

        [HttpGet]
        public async Task<IActionResult> SentForApproval(int? pageIndex)
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved(PageSize, pageIndex ?? 1, CertificateStatus.ToBeApproved, CertificateStatus.SentForApproval);
            var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items.OrderBy(x => x.UpdatedAt));
            var certificatesSentForApproval = new CertificateApprovalViewModel
            {
                SentForApprovalCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items, certificates.TotalRecordCount, certificates.PageIndex, certificates.PageSize)

            };

            return View(certificatesSentForApproval);
        }

        [HttpGet]
        public async Task<IActionResult> Approved(int? pageIndex)
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved(PageSize, pageIndex ?? 1, CertificateStatus.Submitted, CertificateStatus.Approved);
            var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items);
            var certificatesApproved = new CertificateApprovalViewModel
            {
                ApprovedCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items, certificates.TotalRecordCount, certificates.PageIndex, certificates.PageSize)
            };

            return View(certificatesApproved);
        }

        [HttpGet]
        public async Task<IActionResult> Rejected(int? pageIndex)
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved(PageSize, pageIndex ?? 1, CertificateStatus.Draft, CertificateStatus.Rejected);
            var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items);
            var certificatesThatAreRejected = new CertificateApprovalViewModel
            {
                RejectedCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items, certificates.TotalRecordCount, certificates.PageIndex, certificates.PageSize)
            };

            return View(certificatesThatAreRejected);
        }

        [HttpPost(Name = "Approvals")]
        public async Task<IActionResult> Approvals(
            CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            var approvalsValidationFailed = await ValidateReasonForChange1(certificateApprovalViewModel);
            if (approvalsValidationFailed != null)
                return approvalsValidationFailed;

            certificateApprovalViewModel.UserName = ContextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            SetPrivatelyFundedCertApprovalStatus(certificateApprovalViewModel);
            DeterminNextActionUsingCurrrentStatus(certificateApprovalViewModel);

            await ApiClient.ApproveCertificates(certificateApprovalViewModel);
            return RedirectToAction(certificateApprovalViewModel.ActionHint);
        }
        
        [HttpGet]
        public async Task<FileContentResult> ExportSentForApproval()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved(0,1, CertificateStatus.ToBeApproved, null);
            var data = certificates?.Items
                .Select(x => ToDictionary<object>(x, ApprovalToExcelAttributeMappings()));
          
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(ToDataTable(data), true);

                return File(package.GetAsByteArray(), "application/excel", $"CerificatesToBeApproved-{DateTime.Now:dd-MM-yyyyTHH:mm:ss}.xlsx");
            }
        }


        private async Task<IActionResult> ValidateReasonForChange1(CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            ValidateReasonForChange(certificateApprovalViewModel);
            if (ModelState.IsValid) return null;

            if (certificateApprovalViewModel.ApprovalResults.Any(
                x => x.IsApproved == CertificateStatus.Draft && string.IsNullOrEmpty(x.PrivatelyFundedStatus)))
            {
                var certificates = await ApiClient.GetCertificatesToBeApproved(PageSize,
                    certificateApprovalViewModel.PageIndex ?? 1, CertificateStatus.ToBeApproved, null);
                var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items);
                var certificatesToBeApproved = new CertificateApprovalViewModel
                {
                    ToBeApprovedCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items,
                        certificates.TotalRecordCount,
                        certificateApprovalViewModel.PageIndex ?? certificates.PageIndex, certificates.PageSize)
                };

                return View("~/Views/CertificateApprovals/New.cshtml", certificatesToBeApproved);
            }

            if (certificateApprovalViewModel.ApprovalResults.Any(
                x => (x.IsApproved == CertificateStatus.ToBeApproved || x.IsApproved == CertificateStatus.Draft) &&
                     x.PrivatelyFundedStatus == CertificateStatus.SentForApproval))
            {
                var certificates = await ApiClient.GetCertificatesToBeApproved(PageSize,
                    certificateApprovalViewModel.PageIndex ?? 1, CertificateStatus.ToBeApproved,
                    CertificateStatus.SentForApproval);
                var items = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates.Items);
                var certificatesSentForApproval = new CertificateApprovalViewModel
                {
                    SentForApprovalCertificates = new PaginatedList<CertificateDetailApprovalViewModel>(items,
                        certificates.TotalRecordCount,
                        certificateApprovalViewModel.PageIndex ?? certificates.PageIndex, certificates.PageSize)
                };

                {
                    return View("~/Views/CertificateApprovals/SentForApproval.cshtml", certificatesSentForApproval);
                }
            }

            return null;
        }

        private void ValidateReasonForChange(CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            var count = 0;
            foreach (var approvalResult in certificateApprovalViewModel.ApprovalResults)
            {
                //Will only have status of draft if rejected, so check reason for change is updated
                if (string.IsNullOrEmpty(approvalResult.ReasonForChange) &&
                    approvalResult.IsApproved == CertificateStatus.Draft)
                {
                    ModelState.AddModelError($"approvalResults[{count++}].ReasonForChange", "Please enter a reason for rejection");
                }
            }
        }

        private static void SetPrivatelyFundedCertApprovalStatus(CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            foreach (var approvalResult in certificateApprovalViewModel.ApprovalResults)
            {
                switch (approvalResult.IsApproved)
                {
                    //When its a new certificate that needs approval the PrivatelyFundedStatus will always be null 
                    case CertificateStatus.ToBeApproved when approvalResult.PrivatelyFundedStatus == null:
                        approvalResult.PrivatelyFundedStatus = CertificateStatus.SentForApproval;
                        break;
                    case CertificateStatus.Submitted when approvalResult.PrivatelyFundedStatus == CertificateStatus.SentForApproval:
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
        }

        private static void DeterminNextActionUsingCurrrentStatus(CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            //If we are on the New Screen
            if (certificateApprovalViewModel.ActionHint == CertificateStatus.SentForApproval)
            {
                //If all certificates are rejected just go to the rejected screen
                if (certificateApprovalViewModel.ApprovalResults.All(x => x.IsApproved != CertificateStatus.ToBeApproved))
                    certificateApprovalViewModel.ActionHint = CertificateStatus.Rejected;
            }
            else
            {
                //We got here that means our current screen is the Sent For Approval screen, since you can't submit from 
                //Approved and Reject screen
                if (certificateApprovalViewModel.ApprovalResults.Any(
                    x => x.IsApproved == CertificateStatus.ToBeApproved))
                {
                    //If there are still some certificates left to be approved|rejected stay on the Sent For Approval screen
                    certificateApprovalViewModel.ActionHint = CertificateStatus.SentForApproval;
                }
                else if (certificateApprovalViewModel.ApprovalResults.Any(
                    x => x.IsApproved == CertificateStatus.Submitted))
                {
                    //If there where any certificates marked as approved, set the next screen as Approved
                    certificateApprovalViewModel.ActionHint = CertificateStatus.Approved;
                }
                else
                {
                    //Only go to reject screen when all certificate where rejected
                    certificateApprovalViewModel.ActionHint = CertificateStatus.Rejected;
                }
            }

        }

        private static Dictionary<string, object> ToDictionary<TValue>(object obj,string sentForApprovalExcelAttributeMapping)
        {
           var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            var destDictionary = new Dictionary<string, object>();
            MatchAndCreateNewMapping(sentForApprovalExcelAttributeMapping, dictionary, destDictionary);
            return destDictionary;
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

        private static string ApprovalToExcelAttributeMappings()
        {
            return new JObject
            {
                {"EpaoId" , "EPAO ID"},
                {"EpaoName" , "EPAO Name"},
                {"TrainingProvider","Provider Name" },
                {"Ukprn" , "Provider UKPRN"},
                {"Uln" ,"Unique Learner Number"},
                {"FirstName" , "First Name"},
                {"LastName" , "Family Name"},
                {"StandardCode" , "Standard Code"},
                {"StandardName", "Standard Name"},
                {"Level" , "Level"},
                {"CourseOption" , "Option (if applicable)"},
                {"OverallGrade" , "Overall Grade"},
                {"LearningStartDate" , "Learning Start Date"},
                {"AchievementDate" , "Achievement Date"}
            }.ToString();

        }

        private static void MatchAndCreateNewMapping(string mappingJson, dynamic sourceJson, IDictionary<string, object> destinationDictionary)
        {
            var mappings = JsonConvert.DeserializeObject<Dictionary<string, object>>(mappingJson);
            foreach (var mapping in mappings)
            {
                foreach (KeyValuePair<string, object> source in sourceJson)
                {
                    if (mapping.Key != source.Key) continue;
                    if (mapping.Key == "LearningStartDate" || mapping.Key == "AchievementDate")
                        destinationDictionary.Add(mapping.Value.ToString(),
                            ((DateTime?) source.Value)?.ToShortDateString());
                    else
                        destinationDictionary.Add(mapping.Value.ToString(), source.Value);
                    break;
                }
            }
        }
    }
}
