using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests
{
    public static class EntityHelpers
    {
        public static Mock<DbSet<T>> CreateMockSet<T>(this IQueryable<T> data)
            where T : BaseEntity
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        public static T WithComputedFieldsFromData<T>(this T certificate)
            where T : CertificateBase
        {
            var data = certificate.CertificateData;
            if (data == null)
            {
                return certificate;
            }

            var type = typeof(CertificateBase);

            void Set(string name, object value)
            {
                var prop = type.GetProperty(name);
                if (prop != null && value != null)
                {
                    prop.SetValue(certificate, value);
                }
            }

            Set(nameof(CertificateBase.StandardName), data.StandardName);
            Set(nameof(CertificateBase.StandardReference), data.StandardReference);
            Set(nameof(CertificateBase.StandardLevel), data.StandardLevel);
            Set(nameof(CertificateBase.Version), data.Version);
            Set(nameof(CertificateBase.AchievementDate), data.AchievementDate);
            Set(nameof(CertificateBase.LearnerGivenNames), data.LearnerGivenNames);
            Set(nameof(CertificateBase.LearnerFamilyName), data.LearnerFamilyName);
            Set(nameof(CertificateBase.ContactOrganisation), data.ContactOrganisation);
            Set(nameof(CertificateBase.ContactName), data.ContactName);
            Set(nameof(CertificateBase.ContactAddLine1), data.ContactAddLine1);
            Set(nameof(CertificateBase.ContactAddLine2), data.ContactAddLine2);
            Set(nameof(CertificateBase.ContactAddLine3), data.ContactAddLine3);
            Set(nameof(CertificateBase.ContactAddLine4), data.ContactAddLine4);
            Set(nameof(CertificateBase.ContactPostCode), data.ContactPostCode);

            return certificate;
        }
    }
}
