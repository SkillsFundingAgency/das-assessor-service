using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetAparSummaryTests
    {
        protected GetAparSummaryHandler _sut;
        protected Mock<ILogger<GetAparSummaryHandler>> _logger;
        protected Mock<IRegisterQueryRepository> _registerQueryRepository;

        private IEnumerable<AparSummary> _expectedAparSummary;
        private IEnumerable<AparSummary> _expectedAparSummaryForUkprn1111111;
        private IEnumerable<AparSummary> _expectedAparSummaryForUkprn2222222;
        private AparSummary _aparSummary1111111;
        private AparSummary _aparSummary2222222;

        [SetUp]
        public void Setup()
        {
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();
            _aparSummary1111111 = new AparSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111 };
            _aparSummary2222222 = new AparSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222 };

            _logger = new Mock<ILogger<GetAparSummaryHandler>>();

            _expectedAparSummary = new List<AparSummary>
                {
                    _aparSummary1111111,
                    _aparSummary2222222
                };

            _expectedAparSummaryForUkprn1111111 = new List<AparSummary>
                {
                    _aparSummary1111111
                };

            _expectedAparSummaryForUkprn2222222 = new List<AparSummary>
                {
                    _aparSummary2222222
                };

            _registerQueryRepository.Setup(r => r.GetAparSummary(null))
                .ReturnsAsync(_expectedAparSummary);

            _registerQueryRepository.Setup(r => r.GetAparSummary(1111111))
                .ReturnsAsync(_expectedAparSummaryForUkprn1111111);

            _registerQueryRepository.Setup(r => r.GetAparSummary(2222222))
                .ReturnsAsync(_expectedAparSummaryForUkprn2222222);

            _sut = new GetAparSummaryHandler(_registerQueryRepository.Object, _logger.Object);
        }

        [TestCase(1111111)]
        [TestCase(2222222)]
        public void Handle_ReturnExpectedListAparSummaryForSpecificUkprn_WhenHandlerIsInvoked(int ukprn)
        {
            var organisations = _sut.Handle(new GetAparSummaryRequest(ukprn), new CancellationToken()).Result;

            organisations.Should().HaveCount(1, "because there should be exactly one apar summary with a matching ukprn");
            organisations.Single().Ukprn.Should().Be(ukprn);
        }

        [TestCase(1111111)]
        [TestCase(2222222)]
        public void Handle_GetAparSummaryByUkprnRequestIsCalledForSpecificUkprn_WhenHandlerIsInvoked(int ukprn)
        {
            _sut.Handle(new GetAparSummaryRequest(ukprn), new CancellationToken()).Wait();
            _registerQueryRepository.Verify(r => r.GetAparSummary(ukprn));
        }

        [Test]
        public void Handle_ReturnExpectedListAparSummaryForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAparSummaryRequest(), new CancellationToken()).Result;

            organisations.Should().HaveCount(2, "because there should be exactly two apar summaries with any matching ukprn");
            organisations.First().Ukprn.Should().Be(1111111);
            organisations.Last().Ukprn.Should().Be(2222222);
        }

        [Test]
        public void Handle_GetAparSummaryByUkprnRequestIsCalledForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAparSummaryRequest(), new CancellationToken()).Wait();
            _registerQueryRepository.Verify(r => r.GetAparSummary(null));
        }
    }
}
