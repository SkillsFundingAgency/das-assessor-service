using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates;

public class WhenSystemUpdatesStandardCertificate
{
    private Mock<IAssessorDbContext> _mockDbContext;
    private CertificateRepository _sut;

    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<IAssessorDbContext>();

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var mockUnitOfWork = new Mock<IAssessorUnitOfWork>();

        mockUnitOfWork
            .SetupGet(x => x.AssessorDbContext)
            .Returns(_mockDbContext.Object);

        _sut = new CertificateRepository(mockUnitOfWork.Object);
    }

    [Test]
    public async Task ThenDateOfBirthIsUpdated()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var updatedDateOfBirth = new DateTime(2000, 2, 3, 0, 0, 0, DateTimeKind.Utc);

        var storedCertificate = new Certificate
        {
            Id = certificateId,
            DateOfBirth = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CertificateData = new CertificateData()
        };

        var updatedCertificate = new Certificate
        {
            Id = certificateId,
            DateOfBirth = updatedDateOfBirth,
            CertificateData = new CertificateData()
        };

        _mockDbContext
            .Setup(x => x.Set<Certificate>())
            .ReturnsDbSet(new List<Certificate>
            {
                storedCertificate
            });

        // Act
        await _sut.UpdateStandardCertificate(
            updatedCertificate,
            "test-user",
            action: null,
            updateLog: false);

        // Assert
        storedCertificate.DateOfBirth.Should().Be(updatedDateOfBirth);

        _mockDbContext.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ThenDateOfBirthCanBeUpdatedToNull()
    {
        // Arrange
        var certificateId = Guid.NewGuid();

        var storedCertificate = new Certificate
        {
            Id = certificateId,
            DateOfBirth = new DateTime(2000, 2, 3, 0, 0, 0, DateTimeKind.Utc),
            CertificateData = new CertificateData()
        };

        var updatedCertificate = new Certificate
        {
            Id = certificateId,
            DateOfBirth = null,
            CertificateData = new CertificateData()
        };

        _mockDbContext
            .Setup(x => x.Set<Certificate>())
            .ReturnsDbSet(new List<Certificate>
            {
                storedCertificate
            });

        // Act
        await _sut.UpdateStandardCertificate(
            updatedCertificate,
            "test-user",
            action: null,
            updateLog: false);

        // Assert
        storedCertificate.DateOfBirth.Should().BeNull();
    }
}