using System;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Factories
{
    public static class StandardFactory
    {
        public static StandardModel Create(
            string title = "Standard 1",
            string referenceNumber = "ST0001",
            int larsCode = 123,
            string version = "1.0")
        {
            ConvertVersionStringToInts(version, out var major, out var minor);

            return new StandardModel
            {
                StandardUId = $"{referenceNumber}_{version}",
                IFateReferenceNumber = referenceNumber,
                LarsCode = larsCode,
                Title = title,
                Version = version,
                Level = 4,
                Status = "Approved for delivery",
                TypicalDuration = 12,
                TrailblazerContact = "TrailblazerContact",
                VersionMajor = major,
                VersionMinor = minor,
                StandardPageUrl = "www.standard.com",
                EqaProviderName = string.Empty,
                OverviewOfRole = "OverviewOfRole",
            };
        }

        public static StandardModel WithEffectiveFrom(this StandardModel standard, DateTime? effectiveFrom)
        {
            standard.EffectiveFrom = effectiveFrom;
            return standard;
        }

        public static StandardModel WithEffectiveTo(this StandardModel standard, DateTime? effectiveTo)
        {
            standard.EffectiveTo = effectiveTo;
            return standard;
        }

        public static StandardModel WithVersionEarliestStartDate(this StandardModel standard, DateTime? versionEarliestStartDate)
        {
            standard.VersionEarliestStartDate = versionEarliestStartDate;
            return standard;
        }

        public static StandardModel WithVersionLatestStartDate(this StandardModel standard, DateTime? versionLatestStartDate)
        {
            standard.VersionLatestStartDate = versionLatestStartDate;
            return standard;
        }

        public static StandardModel WithVersionLatestEndDate(this StandardModel standard, DateTime? versionLatestEndDate)
        {
            standard.VersionLatestEndDate = versionLatestEndDate;
            return standard;
        }

        public static StandardModel WithVersionApprovedForDelivery(this StandardModel standard, DateTime? versionApprovedForDelivery)
        {
            standard.VersionApprovedForDelivery = versionApprovedForDelivery;
            return standard;
        }

        public static StandardModel WithEPAChanged(this StandardModel standard, bool epaChanged)
        {
            standard.EPAChanged = epaChanged;
            return standard;
        }

        public static StandardModel WithEqaProviderName(this StandardModel standard, string eqaProviderName)
        {
            standard.EqaProviderName = eqaProviderName;
            return standard;
        }

        public static StandardModel WithEpaoMustBeApprovedByRegulatorBody(this StandardModel standard, bool epaoMustBeApprovedByRegulatorBody)
        {
            standard.EpaoMustBeApprovedByRegulatorBody = epaoMustBeApprovedByRegulatorBody;
            return standard;
        }

        private static void ConvertVersionStringToInts(string version, out int major, out int minor)
        {
            var parts = version.Split('.');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid version format");
            }

            major = int.Parse(parts[0]);
            minor = int.Parse(parts[1]);
        }
    }
}
