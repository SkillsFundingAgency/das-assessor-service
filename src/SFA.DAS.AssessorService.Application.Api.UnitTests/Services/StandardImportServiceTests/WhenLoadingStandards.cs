﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenLoadingStandards
    {
        Mock<IStandardRepository> standardRepositoryMock;
        IEnumerable<StandardDetailResponse> standards;

        [SetUp]
        public async Task Initialize()
        {
            var fixture = new Fixture();
            standards = fixture.Build<StandardDetailResponse>().CreateMany();
            standardRepositoryMock = new Mock<IStandardRepository>();

            var sut = new StandardImportService(standardRepositoryMock.Object);

            await sut.LoadStandards(standards);
        }

        [Test]
        public void Then_Inserts_Data_Into_Standards_Table()
        {
            standardRepositoryMock.Verify(r => r.InsertStandards(It.IsAny<IEnumerable<Standard>>()), Times.Once);
        }
    }
}
