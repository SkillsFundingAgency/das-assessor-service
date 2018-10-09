using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetStandardsByOrganisationIdTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetStandardsByAssessmentOrganisationHandler GetStandardsByAssessmentOrganisationHandler;
        protected Mock<ILogger<GetStandardsByAssessmentOrganisationHandler>> Logger;
        private List<OrganisationStandardSummary> _expectedStandards;
        private OrganisationStandardSummary _standard1;
        private OrganisationStandardSummary _standard2;
        private OrganisationStandardSummary _standard3;
        private readonly string _organisationId = "EPAXXX";
        private GetStandardsByOrganisationRequest _request;
        private int _standardCode1 = 1;
        private int _standardCode2 = 2;
        private int _standardCode3 = 3;
        private OrganisationStandardPeriod period1a;
        private OrganisationStandardPeriod period1b;
        private OrganisationStandardPeriod period2;
        private List<OrganisationStandardPeriod> _expectedPeriods1;
        private List<OrganisationStandardPeriod> _expectedPeriods2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _standard1 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode1, Periods = new List<OrganisationStandardPeriod>() };
            _standard2 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode2, Periods = new List<OrganisationStandardPeriod>() };
            _standard3 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode3, Periods = new List<OrganisationStandardPeriod>() };

            _expectedStandards = new List<OrganisationStandardSummary>
            {
                _standard1,
                _standard2,
                _standard3
            };

            period1a = new OrganisationStandardPeriod {EffectiveFrom = DateTime.Today.AddYears(-1)};
            period1b = new OrganisationStandardPeriod { EffectiveFrom = DateTime.Today.AddMonths(-1),  EffectiveTo = DateTime.Today.AddMonths(1) };
            _expectedPeriods1 = new List<OrganisationStandardPeriod>
            {
                period1a,
                period1b
            };

            period1b = new OrganisationStandardPeriod { EffectiveFrom = DateTime.Today.AddMonths(-1)};
            _expectedPeriods2 = new List<OrganisationStandardPeriod>
            {
                period2
            };

            _request = new GetStandardsByOrganisationRequest { OrganisationId = _organisationId };

            Logger = new Mock<ILogger<GetStandardsByAssessmentOrganisationHandler>>();

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardByOrganisationId(_organisationId))
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardPeriodsByOrganisationStandard(_organisationId, _standardCode1))
                .Returns(Task.FromResult(_expectedPeriods1.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardPeriodsByOrganisationStandard(_organisationId, _standardCode2))
                .Returns(Task.FromResult(_expectedPeriods2.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardPeriodsByOrganisationStandard(_organisationId, _standardCode3))
                .Returns(Task.FromResult(new List<OrganisationStandardPeriod>().AsEnumerable()));

            GetStandardsByAssessmentOrganisationHandler =
                new GetStandardsByAssessmentOrganisationHandler(RegisterQueryRepository.Object, Logger.Object);
        }


        [Test]
        public void GetStandardsByOrganisationRepoIsCalledWhenHandlerInvoked()
        {
            GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetOrganisationStandardByOrganisationId(_organisationId));
        }


        [Test]
        public void GetPeriodByOrganisationStandardRepoIsCalledThreeTimesWhenHandlerInvoked()
        {
            GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetOrganisationStandardPeriodsByOrganisationStandard(_organisationId,It.IsAny<int>()),Times.Exactly(3));
        }

        [Test]
        public void GetStandardsForOrganisationReturns3SetsOfStandards()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            standards.Count.Should().Be(3);
        }

        [Test]
        public void GetStandardsForOrganisationReturnsStandard1OfStandardsWithExpected2Periods()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.Where(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode1).First();
            var periods = standard.Periods;
            Assert.AreEqual(periods,_expectedPeriods1);
        }

        [Test]
        public void GetStandardsForOrganisationReturnsStandard2OfStandardsWithExpected1Period()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.Where(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode2).First();
            var periods = standard.Periods;
            Assert.AreEqual(periods, _expectedPeriods2);
        }


        [Test]
        public void GetStandardsForOrganisationReturnsStandard3OfStandardsWithExpectedNoPeriods()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.Where(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode3).First();
            var periods = standard.Periods;
            periods.Count.Should().Be(0);
        }
    }
}
