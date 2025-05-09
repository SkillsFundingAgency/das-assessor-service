﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards
{
    public class WhenHandlingGetAppliedStandardVersionsForEpaoRequest
    {
        [Test, MoqAutoData]
        public async Task ThenGetsAllStandardVersions(
            [Frozen] Mock<IRegisterQueryRepository> repository,
            GetAppliedStandardVersionsForEpaoRequest request,
            List<AppliedStandardVersion> versions,
            GetAppliedStandardVersionsForEpaoHandler sut)
        {
            //Arrange
            repository.Setup(s => s.GetAppliedStandardVersionsForEPAO(request.OrganisationId, request.StandardReference)).ReturnsAsync(versions);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().BeEquivalentTo(versions);
        }
    }
}
