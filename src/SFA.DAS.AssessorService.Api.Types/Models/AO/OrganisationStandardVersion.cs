using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardVersion
    {
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public int LarsCode { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Version { get; set; }
        public int Level { get; set; }
        public int OrganisationStandardId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }

        public static explicit operator OrganisationStandardVersion(Domain.Entities.OrganisationStandardVersion version)
        {
            return new OrganisationStandardVersion
            {
                StandardUId = version.StandardUId,
                LarsCode = version.LarsCode,
                IFateReferenceNumber = version.IFateReferenceNumber,
                Version = version.Version,
                Title = version.Title,
                Level = version.Level,
                OrganisationStandardId = version.OrganisationStandardId,
                EffectiveFrom = version.EffectiveFrom,
                EffectiveTo = version.EffectiveTo,
                DateVersionApproved = version.DateVersionApproved,
                Comments = version.Comments,
                Status = version.Status
            };
        }
    }
}