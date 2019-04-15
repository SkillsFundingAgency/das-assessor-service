namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using Api.Types.Models.Roatp;

    public class UpdateOrganisationProviderTypeViewModel
    {
        public IEnumerable<ProviderType> ProviderTypes { get; set; }
        public Dictionary<int, IEnumerable<OrganisationType>> OrganisationTypesByProviderType { get; set; }
        public string LegalName { get; set; }
        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string UpdatedBy { get; set; }

        public bool CanChangeOrganisationTypeForThisProvider(int comparisonProviderTypeId)
        {
            if ((ProviderTypeId == 1 || ProviderTypeId == 3) && (comparisonProviderTypeId == 2))
            {
                return true;
            }

            if ((ProviderTypeId == 2) && (comparisonProviderTypeId == 1 || comparisonProviderTypeId == 3))
            {
                return true;
            }

            return false;
        }
    }
}
