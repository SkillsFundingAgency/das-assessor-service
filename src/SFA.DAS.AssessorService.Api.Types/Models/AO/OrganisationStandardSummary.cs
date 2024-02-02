using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardSummary
    {
        public int Id { get; set; }
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }

        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public Guid? ContactId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public List<int> DeliveryAreas { get; set; }
        public string Comments { get; set; }
        public List<OrganisationStandardVersion> StandardVersions { get; set; }

        public OrganisationStandardData OrganisationStandardData { get; set; }

        public string Title
        {
            get
            {
                if (StandardVersions != null && StandardVersions.Count > 0)
                {
                    return StandardVersions.OrderByDescending(s => s.VersionMajor).ThenBy(t => t.VersionMinor).First().Title;
                }

                return string.Empty;
            }
        }

        public int LarsCode
        {
            get
            {
                if (StandardVersions != null && StandardVersions.Count > 0)
                {
                    return StandardVersions.OrderByDescending(s => s.VersionMajor).ThenBy(t => t.VersionMinor).First().LarsCode;
                }

                return 0;
            }
        }

        public string IFateReferenceNumber
        {
            get
            {
                if (StandardVersions != null && StandardVersions.Count > 0)
                {
                    return StandardVersions.OrderByDescending(s => s.VersionMajor).ThenBy(t => t.VersionMinor).First().IFateReferenceNumber;
                }

                return string.Empty;
            }
        }
    }
}
