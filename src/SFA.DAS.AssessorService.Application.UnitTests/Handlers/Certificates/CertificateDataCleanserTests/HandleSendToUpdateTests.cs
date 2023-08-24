using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.CertificateDataCleanserTests
{
    public class HandleSendToUpdateTests
    {
        [Test]
        public void AddressFields_AreNotCleared_IfCertificateWasCreatedByAPI_And_SendToChangedFromNoneToEmployer()
        {
            var certificate = new Certificate() { CreatedBy = "API" };
            var currentCertificateData = GetCertificateDataContainingAddress(CertificateSendTo.None);
            var updatedCertificateData = GetCertificateDataContainingAddress(CertificateSendTo.Employer);

            var result = CertificateDataCleanser.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            Assert.Multiple(() =>
            {
                Assert.That(result.ContactName, Is.Not.Null);
                Assert.That(result.Department, Is.Not.Null);
                Assert.That(result.ContactOrganisation, Is.Not.Null);
                Assert.That(result.ContactAddLine1, Is.Not.Null);
                Assert.That(result.ContactAddLine2, Is.Not.Null);
                Assert.That(result.ContactAddLine3, Is.Not.Null);
                Assert.That(result.ContactAddLine4, Is.Not.Null);
                Assert.That(result.ContactPostCode, Is.Not.Null);
            });
        }

        [TestCase(CertificateSendTo.None, CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Employer, CertificateSendTo.Employer, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.Apprentice, "NOT API")]
        [TestCase(CertificateSendTo.None, CertificateSendTo.None, "API")]
        [TestCase(CertificateSendTo.Employer, CertificateSendTo.Employer, "API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.Apprentice, "API")]
        public void AddressFields_AreNotCleared_If_SendToHasNotChanged(
            CertificateSendTo currentSendTo,
            CertificateSendTo updatedSendTo,
            string certificateCreatedBy)
        {
            var certificate = new Certificate() { CreatedBy = certificateCreatedBy };
            var currentCertificateData = GetCertificateDataContainingAddress(currentSendTo);
            var updatedCertificateData = GetCertificateDataContainingAddress(updatedSendTo);

            var result = CertificateDataCleanser.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            Assert.Multiple(() =>
            {
                Assert.That(result.ContactName, Is.Not.Null);
                Assert.That(result.Department, Is.Not.Null);
                Assert.That(result.ContactOrganisation, Is.Not.Null);
                Assert.That(result.ContactAddLine1, Is.Not.Null);
                Assert.That(result.ContactAddLine2, Is.Not.Null);
                Assert.That(result.ContactAddLine3, Is.Not.Null);
                Assert.That(result.ContactAddLine4, Is.Not.Null);
                Assert.That(result.ContactPostCode, Is.Not.Null);
            });
        }

        [TestCase(CertificateSendTo.None, CertificateSendTo.Employer, "NOT API")]
        [TestCase(CertificateSendTo.Employer, CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.Employer, "NOT API")]
        [TestCase(CertificateSendTo.Employer, CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.Employer, "NOT API")]
        [TestCase(CertificateSendTo.Employer, CertificateSendTo.None, "API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.None, "API")]
        [TestCase(CertificateSendTo.Apprentice, CertificateSendTo.Employer, "API")]
        public void AddressFieldsAreCleared_If_SendToHasChangedInCertainWays(
            CertificateSendTo currentSendTo,
            CertificateSendTo updatedSendTo,
            string certificateCreatedBy)
        {
            var certificate = new Certificate() { CreatedBy = certificateCreatedBy };
            var currentCertificateData = GetCertificateDataContainingAddress(currentSendTo);
            var updatedCertificateData = GetCertificateDataContainingAddress(updatedSendTo);

            var result = CertificateDataCleanser.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            Assert.Multiple(() =>
            {
                Assert.That(result.ContactName, Is.Null);
                Assert.That(result.Department, Is.Null);
                Assert.That(result.ContactOrganisation, Is.Null);
                Assert.That(result.ContactAddLine1, Is.Null);
                Assert.That(result.ContactAddLine2, Is.Null);
                Assert.That(result.ContactAddLine3, Is.Null);
                Assert.That(result.ContactAddLine4, Is.Null);
                Assert.That(result.ContactPostCode, Is.Null);
            });
        }

        [TestCase(CertificateSendTo.None, "API")]
        [TestCase(CertificateSendTo.Employer, "API")]
        [TestCase(CertificateSendTo.Apprentice, "API")]
        [TestCase(CertificateSendTo.None, "NOT API")]
        [TestCase(CertificateSendTo.Employer, "NOT API")]
        [TestCase(CertificateSendTo.Apprentice, "NOT API")]
        public void ContactName_IsSet_ToFullName_If_UpdatedSendTo_IsApprentice(CertificateSendTo currentSendTo, string certificateCreatedBy)
        {
            var certificate = new Certificate() { CreatedBy = certificateCreatedBy };
            var currentCertificateData = GetCertificateDataContainingAddress(currentSendTo);
            var updatedCertificateData = GetCertificateDataContainingAddress(CertificateSendTo.Apprentice);

            var result = CertificateDataCleanser.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            Assert.That(result.ContactName, Is.EqualTo(result.FullName.ToUpper()));
        }

        [Test]
        public void Option_IsSetToNull_IfVersionHasChanged()
        {
            var currentData = new CertificateData() { Version = "1.0", CourseOption = "SomeOption" };
            var newData = new CertificateData() { Version = "1.1", CourseOption = "SomeOption" };

            CertificateDataCleanser.HandleSendToUpdate(new Certificate(), currentData, newData);

            Assert.That(newData.CourseOption, Is.Null);
        }

        private static CertificateData GetCertificateDataContainingAddress(CertificateSendTo certificateSendTo)
        {
            return new CertificateData()
            {
                SendTo = certificateSendTo,
                ContactName = "contact name",
                FullName = "full name",
                Department = "department",
                ContactOrganisation = "organisation",
                ContactAddLine1 = "address line 1",
                ContactAddLine2 = "address line 2",
                ContactAddLine3 = "address line 3",
                ContactAddLine4 = "address line 4",
                ContactPostCode = "postcode"
            };
        }
    }
}


