using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates;

public class WhenSystemCreatesStandardCertificate
{
    private Mock<IAssessorDbContext> _mockDbContext;
    private Mock<IAssessorUnitOfWork> _mockUnitOfWork;
    private Mock<DbSet<CertificateBase>> _mockCertificateSet;
    private CertificateRepository _sut;

    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<IAssessorDbContext>();
        _mockUnitOfWork = new Mock<IAssessorUnitOfWork>();
        _mockCertificateSet = new Mock<DbSet<CertificateBase>>();

        _mockDbContext
            .Setup(x => x.StandardCertificates)
            .ReturnsDbSet(new List<Certificate>());

        _mockDbContext
            .Setup(x => x.Set<CertificateBase>())
            .Returns(_mockCertificateSet.Object);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var mockCertificateLogs = new Mock<DbSet<CertificateLog>>();

        mockCertificateLogs
            .Setup(x => x.Add(It.IsAny<CertificateLog>()))
            .Returns((EntityEntry<CertificateLog>)null);

        _mockDbContext
            .Setup(x => x.CertificateLogs)
            .Returns(mockCertificateLogs.Object);

        _mockCertificateSet
            .Setup(x => x.AddAsync(
                It.IsAny<CertificateBase>(),
                It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EntityEntry<CertificateBase>>(
                (EntityEntry<CertificateBase>)null));

        _mockUnitOfWork
            .SetupGet(x => x.AssessorDbContext)
            .Returns(_mockDbContext.Object);

        _mockUnitOfWork
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns(
                (Func<Task> action, CancellationToken cancellationToken) =>
                    action());

        _sut = new CertificateRepository(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task ThenNullDateOfBirthIsPreserved()
    {
        // Arrange
        var certificate = new Certificate
        {
            Uln = 1234567890,
            StandardCode = 123,
            DateOfBirth = null,
            CertificateData = new CertificateData
            {
                EpaDetails = new EpaDetails()
            }
        };

        CertificateBase addedCertificate = null;

        _mockCertificateSet
            .Setup(x => x.AddAsync(
                It.IsAny<CertificateBase>(),
                It.IsAny<CancellationToken>()))
            .Callback<CertificateBase, CancellationToken>(
                (entity, cancellationToken) => addedCertificate = entity)
            .Returns(new ValueTask<EntityEntry<CertificateBase>>(
                (EntityEntry<CertificateBase>)null));

        // Act
        await _sut.NewStandardCertificate(certificate);

        // Assert
        addedCertificate
            .Should()
            .BeOfType<Certificate>()
            .Which.DateOfBirth
            .Should()
            .BeNull();

        _mockDbContext.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ThenPopulatedDateOfBirthIsPreserved()
    {
        // Arrange
        var dateOfBirth = new DateTime(2000, 2, 3, 0, 0, 0, DateTimeKind.Utc);

        var certificate = new Certificate
        {
            Uln = 1234567890,
            StandardCode = 123,
            DateOfBirth = dateOfBirth,
            CertificateData = new CertificateData
            {
                EpaDetails = new EpaDetails()
            }
        };

        CertificateBase addedCertificate = null;

        _mockCertificateSet
            .Setup(x => x.AddAsync(
                It.IsAny<CertificateBase>(),
                It.IsAny<CancellationToken>()))
            .Callback<CertificateBase, CancellationToken>(
                (entity, cancellationToken) => addedCertificate = entity)
            .Returns(new ValueTask<EntityEntry<CertificateBase>>(
                (EntityEntry<CertificateBase>)null));

        // Act
        await _sut.NewStandardCertificate(certificate);

        // Assert
        addedCertificate
            .Should()
            .BeOfType<Certificate>()
            .Which.DateOfBirth
            .Should()
            .Be(dateOfBirth);
    }
}