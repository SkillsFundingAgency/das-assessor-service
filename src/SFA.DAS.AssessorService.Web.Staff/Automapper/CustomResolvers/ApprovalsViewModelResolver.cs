using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Automapper.CustomResolvers
{
    public class ApprovalsViewModelResolver : IValueResolver<CertificateSummaryResponse, CertificateDetailApprovalViewModel,
        IEnumerable<SelectListItem>>
    {
        public IEnumerable<SelectListItem> Resolve(CertificateSummaryResponse source, CertificateDetailApprovalViewModel destination,
            IEnumerable<SelectListItem> destMember,
            ResolutionContext context)
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = "Please Select", Value = "ToBeApproved"},
                new SelectListItem
                {
                    Text = "Approve",
                    Value = "Approved"
                },
                new SelectListItem
                {
                    Text = "Reject",
                    Value = "Rejected"
                }
            };
        }
    }
}