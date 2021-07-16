using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetAppliedStandardVersionsForEpaoTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private IEnumerable<AppliedStandardVersion> _expectedVersions;
        private const string OrganisationId = "ABC123";
        private const string StandardReference = "ST1234";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _expectedVersions = new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.Approved},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.2M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.3M, LarsCode = 1, EPAChanged = true, ApprovedStatus = ApprovedStatus.ApplyInProgress},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.4M, LarsCode = 1, EPAChanged = true, ApprovedStatus = ApprovedStatus.NotYetApplied},
               };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAppliedStandardVersionsForEpaoRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedVersions);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetAppliedStandardVersionsForEpao(OrganisationId, StandardReference).Result;
        }

        [Test]
        public void GetStandardVersionsByOrganisationIdAndStandardReferenceReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetStandardVersionsByOrganisationIdAndStandardReferenceRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAppliedStandardVersionsForEpaoRequest>(), new CancellationToken()));
        }

        [Test]
        public void GetStandardVersionsByOrganisationIdAndStandardReferenceReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeListAppliedStandardVersion()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AppliedStandardVersion>>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationStandard()
        {
            var versions = ((OkObjectResult)_result).Value as IEnumerable<AppliedStandardVersion>;
            versions.Should().BeEquivalentTo(_expectedVersions);
        }
    }
}
