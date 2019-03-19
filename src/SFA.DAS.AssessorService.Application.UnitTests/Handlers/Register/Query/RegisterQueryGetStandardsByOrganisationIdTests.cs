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
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.ExternalApis.Services;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetStandardsByOrganisationIdTests
    {
        private Mock<IRegisterQueryRepository> RegisterQueryRepository;
        private Mock<IStandardService> _standardService;
        private GetStandardsByAssessmentOrganisationHandler GetStandardsByAssessmentOrganisationHandler;
        private Mock<ILogger<GetStandardsByAssessmentOrganisationHandler>> Logger;
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
        private int _id1;
        private int _id2;
        private int _id3;
        private List<int> _expectedDeliveryAreas;
        private List<StandardSummary> _allStandardSummaries;
        private string _expectedTitle1;
        private string _expectedTitle2;

        [SetUp]
        public void Setup()
        {
            effectiveFrom1 = DateTime.Today.AddYears(-1);
            effectiveFrom2 = DateTime.Today.AddMonths(-1);
            effectiveTo2 = DateTime.Today.AddMonths(1);
            _id1 = 1;
            _id2 = 2;
            _id3 = 3;
            _expectedTitle1 = "Standard 1";
            _expectedTitle2 = "Standard 2";

            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _standardService = new Mock<IStandardService>();
            _standard1 = new OrganisationStandardSummary { Id = _id1, OrganisationId = _organisationId, StandardCode = _standardCode1, EffectiveFrom = effectiveFrom1 };
            _standard2 = new OrganisationStandardSummary { Id = _id2, OrganisationId = _organisationId, StandardCode = _standardCode2, EffectiveFrom = effectiveFrom2, EffectiveTo = effectiveTo2 };
            _standard3 = new OrganisationStandardSummary { Id = _id3, OrganisationId = _organisationId, StandardCode = _standardCode3 };

            _expectedStandards = new List<OrganisationStandardSummary>
            {
                _standard1,
                _standard2,
                _standard3
            };

            _expectedDeliveryAreas = new List<int>{1,2};

            _allStandardSummaries = new List<StandardSummary>
            {
                new StandardSummary {Id = _id1.ToString(), Title = _expectedTitle1},
                new StandardSummary {Id = _id2.ToString(), Title = _expectedTitle2},
                new StandardSummary {Id = _id3.ToString(), Title = "Standard 3"},
                new StandardSummary{ Id = "534", Title = "Unmapped standard"}
            };
            
            _request = new GetStandardsByOrganisationRequest { OrganisationId = _organisationId };

            Logger = new Mock<ILogger<GetStandardsByAssessmentOrganisationHandler>>();

            RegisterQueryRepository.Setup(r => r.GetOrganisationStandardByOrganisationId(_organisationId))
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetDeliveryAreaIdsByOrganisationStandardId(_id1))
                .Returns(Task.FromResult(_expectedDeliveryAreas.AsEnumerable()));

            _standardService.Setup(s => s.GetAllStandardsV2()).Returns(Task.FromResult(_allStandardSummaries.AsEnumerable()));
            
            GetStandardsByAssessmentOrganisationHandler =
                new GetStandardsByAssessmentOrganisationHandler(RegisterQueryRepository.Object, _standardService.Object,Logger.Object);
        }

        [Test]
        public void GetStandardsByOrganisationRepoIsCalledWhenHandlerInvoked()
        {
            GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetOrganisationStandardByOrganisationId(_organisationId));
            RegisterQueryRepository.Verify(r => r.GetDeliveryAreaIdsByOrganisationStandardId(_id1));
            _standardService.Verify(s => s.GetAllStandardsV2());
        }


        [Test]
        public void GetStandardsForOrganisationReturns3SetsOfStandards()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            standards.Count.Should().Be(3);
        }

        [Test]
        public void GetStandardsForOrganisationReturnsStandard1OfStandardsWithExpectedPeriodsAndStandardTitle()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.First(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode1);
            Assert.AreEqual(effectiveFrom1, standard.EffectiveFrom);
            Assert.AreEqual(null, standard.EffectiveTo);
            Assert.AreEqual(_expectedDeliveryAreas, standard.DeliveryAreas);
            Assert.AreEqual(_expectedTitle1, standard.StandardSummary.Title);
        }

        [Test]
        public void GetStandardsForOrganisationReturnsStandard2OfStandardsWithExpected1PeriodDetails()
        {
            var standards = GetStandardsByAssessmentOrganisationHandler.Handle(_request, new CancellationToken()).Result;
            var standard = standards.First(s => s.OrganisationId == _organisationId && s.StandardCode == _standardCode2);
            Assert.AreEqual(effectiveFrom2, standard.EffectiveFrom);
            Assert.AreEqual(effectiveTo2, standard.EffectiveTo);
            Assert.AreEqual(_expectedTitle2, standard.StandardSummary.Title);
        }
    }
}
