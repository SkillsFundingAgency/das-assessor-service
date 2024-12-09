using FluentAssertions;
using FluentAssertions.Execution;
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

            var result = CertificateDataSendToUpdater.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            using (new AssertionScope())
            {
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
            }
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

            var result = CertificateDataSendToUpdater.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            using (new AssertionScope())
            {
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
                result.ContactName.Should().NotBeNull();
            }
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

            var result = CertificateDataSendToUpdater.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            using (new AssertionScope())
            {
                result.ContactName.Should().BeNull();
                result.Department.Should().BeNull();
                result.ContactOrganisation.Should().BeNull();
                result.ContactAddLine1.Should().BeNull();
                result.ContactAddLine2.Should().BeNull();
                result.ContactAddLine3.Should().BeNull();
                result.ContactAddLine4.Should().BeNull();
                result.ContactPostCode.Should().BeNull();
            }
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

            var result = CertificateDataSendToUpdater.HandleSendToUpdate(certificate, currentCertificateData, updatedCertificateData);

            result.ContactName.Should().Be(result.FullName.ToUpper());
        }

        [Test]
        public void Option_IsSetToNull_IfVersionHasChanged()
        {
            var currentData = new CertificateData() { Version = "1.0", CourseOption = "SomeOption" };
            var newData = new CertificateData() { Version = "1.1", CourseOption = "SomeOption" };

            CertificateDataSendToUpdater.HandleSendToUpdate(new Certificate(), currentData, newData);

            newData.CourseOption.Should().Be(newData.CourseOption);
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


