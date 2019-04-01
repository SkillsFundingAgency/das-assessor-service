using System;


namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    public class UpdateOrganisationFinancialTrackRecordViewModel
    {
        public Guid OrganisationId { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public string UpdatedBy { get; set; }
        public string LegalName { get; set; }
    }
}
