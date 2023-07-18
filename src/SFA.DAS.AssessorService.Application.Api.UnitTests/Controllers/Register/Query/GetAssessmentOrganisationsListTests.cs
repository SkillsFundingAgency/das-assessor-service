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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetAssessmentOrganisationsListTests
    {
        private static RegisterQueryController _sut;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private static object _result;

        private List<AssessmentOrganisationListSummary> _expectedAssessmentOrganisationSummaries;
        private AssessmentOrganisationListSummary _assOrgSummary1;
        private AssessmentOrganisationListSummary _assOrgSummary2;

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _assOrgSummary1 = new AssessmentOrganisationListSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddDays(-1) };
            _assOrgSummary2 = new AssessmentOrganisationListSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddMonths(-1) };

            _expectedAssessmentOrganisationSummaries = new List<AssessmentOrganisationListSummary>
            {
                _assOrgSummary1,
                _assOrgSummary2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationsListRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSummaries);
            _sut = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = await _sut.GetAssessmentOrganisationsList();
        }

        [Test]
        public void GetAssessmentOrganisationsList_MediatorShouldSendGetAssessmentOrganisationsListRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAssessmentOrganisationsListRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAssessmentOrganisationsList_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAssessmentOrganisationsList_ResultsAreOfTypeListAssessmentOrganisationListSummary_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AssessmentOrganisationListSummary>>();
        }

        [Test]
        public void GetAssessmentOrganisationsList_ResultsMatchExpectedListAssessmentOrganisationListSummary_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AssessmentOrganisationListSummary>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assOrgSummary1);
            organisations.Should().Contain(_assOrgSummary2);
        }
    }
}
