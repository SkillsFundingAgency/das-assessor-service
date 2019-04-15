namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using Api.Types.Models.Roatp;

    public class UpdateOrganisationStatusViewModel
    {
        public IEnumerable<OrganisationStatus> OrganisationStatuses { get; set; }
        public IEnumerable<RemovedReason> RemovedReasons { get; set; }
        public string LegalName { get; set; }
        public Guid OrganisationId { get; set; }
        public int OrganisationStatusId { get; set; }
        public int? RemovedReasonId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
