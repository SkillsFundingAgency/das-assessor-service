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

        private DateTime effectiveFrom1;
        private DateTime effectiveFrom2;
        private DateTime effectiveTo2;

        [SetUp]
        public void Setup()
        {
            effectiveFrom1 = DateTime.Today.AddYears(-1);
            effectiveFrom2 = DateTime.Today.AddMonths(-1);
            effectiveTo2 = DateTime.Today.AddMonths(1);

            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _standard1 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode1, EffectiveFrom = effectiveFrom1 };
            _standard2 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode2, EffectiveFrom = effectiveFrom2, EffectiveTo = effectiveTo2 };
            _standard3 = new OrganisationStandardSummary { OrganisationId = _organisationId, StandardCode = _standardCode3 };

            _expectedStandards = new List<OrganisationStandardSummary>
            {
                _standard1,
                _standard2,
                _standard3
            };

            _request = new GetStandardsByOrganisationRequest { OrganisationId = _organisationId };

            Logger = new Mock<ILogger<GetStandardsByAssessmentOrganisationHandler>>();

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardByOrganisationId(_organisationId))
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));

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
            Assert.AreEqual(effectiveFrom1, standard.EffectiveFrom);
            Assert.AreEqual(null, standard.EffectiveTo);
        }

        [Test]
        public void GetStandardsForOrganisationReturnsStandard2OfStandardsWithExpected1Period()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.Where(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode2).First();
            Assert.AreEqual(effectiveFrom2, standard.EffectiveFrom);
            Assert.AreEqual(effectiveTo2, standard.EffectiveTo);
        }
    }
}
