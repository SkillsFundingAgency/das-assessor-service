using System;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Validators;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates
{
    public class When_UpdateCertificatePrintRequestValidator
    {
        private UpdateCertificatePrintRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateCertificatePrintRequestValidator();
        }

        [Test]
        public void Validator_Should_Pass_For_Valid_Request()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                Address = new CertificatePrintAddress
                {
                    ContactName = "Name",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                },
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsTrue(result.IsValid, "Validator should accept a complete print request payload");
        }

        [Test]
        public void Validator_Should_Fail_When_Address_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid, "Validator should fail when Address is not provided");
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Address"));
        }

        [Test]
        public void Validator_Should_Fail_When_ContactName_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                Address = new CertificatePrintAddress
                {
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                },
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Address.ContactName"));
        }

        [Test]
        public void Validator_Should_Fail_When_ContactPostCode_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                Address = new CertificatePrintAddress
                {
                    ContactName = "Name",
                    ContactAddLine1 = "Line1"
                },
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Address.ContactPostCode"));
        }

        [Test]
        public void Validator_Should_Fail_When_PrintRequestedAt_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                Address = new CertificatePrintAddress
                {
                    ContactName = "Name",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                },
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "PrintRequestedAt"));
        }

        [Test]
        public void Validator_Should_Fail_When_PrintRequestedBy_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = Guid.NewGuid(),
                Address = new CertificatePrintAddress
                {
                    ContactName = "Name",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                },
                PrintRequestedAt = DateTime.UtcNow
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "PrintRequestedBy"));
        }

        [Test]
        public void Validator_Should_Fail_When_CertificateId_Missing()
        {
            // Arrange
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                Address = new CertificatePrintAddress
                {
                    ContactName = "Name",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                },
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "Apprentice"
            };

            // Act
            ValidationResult result = _validator.Validate(cmd);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "CertificateId"));
        }
    }
}
