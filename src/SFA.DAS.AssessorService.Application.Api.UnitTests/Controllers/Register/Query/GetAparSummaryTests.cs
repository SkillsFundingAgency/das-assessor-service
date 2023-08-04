using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetAparSummaryTests
    {
        private static RegisterQueryController _sut;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private static object _result;

        private List<AparSummaryItem> _expectedAssessmentOrganisationSummaries;
        private AparSummaryItem _assOrgSummary1;
        private AparSummaryItem _assOrgSummary2;

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _assOrgSummary1 = new AparSummaryItem { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddDays(-1) };
            _assOrgSummary2 = new AparSummaryItem { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddMonths(-1) };

            _expectedAssessmentOrganisationSummaries = new List<AparSummaryItem>
            {
                _assOrgSummary1,
                _assOrgSummary2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAparSummaryRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSummaries);
            _sut = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = await _sut.GetAparSummary();
        }

        [Test]
        public void GetAparSummary_MediatorShouldSendGetAparSummaryRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAparSummaryRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAparSummary_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAparSummary_ResultsAreOfTypeListAssessmentOrganisationListSummary_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AparSummaryItem>>();
        }

        [Test]
        public void GetAparSummary_ResultsMatchExpectedListAssessmentOrganisationListSummary_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AparSummaryItem>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assOrgSummary1);
            organisations.Should().Contain(_assOrgSummary2);
        }
    }
}
