namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;

    public class UpdateOrganisationTradingNameViewModel
    {
        public string CurrentTradingName { get; set; }
        public Guid OrganisationId { get; set; }
        public string TradingName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
