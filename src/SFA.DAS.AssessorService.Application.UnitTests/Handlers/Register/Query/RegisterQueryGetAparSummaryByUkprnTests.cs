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
    public class RegisterQueryGetAparSummaryByUkprnTests
    {
        protected GetAparSummaryByUkprnHandler _sut;
        protected Mock<ILogger<GetAparSummaryByUkprnHandler>> _logger;
        protected Mock<IRegisterQueryRepository> _registerQueryRepository;

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

            _logger = new Mock<ILogger<GetAparSummaryByUkprnHandler>>();

            _expectedAparSummaryForUkprn1111111 = new List<AparSummary>
                {
                    _aparSummary1111111
                };

            _expectedAparSummaryForUkprn2222222 = new List<AparSummary>
                {
                    _aparSummary2222222
                };

            _registerQueryRepository.Setup(r => r.GetAparSummary(1111111))
                .ReturnsAsync(_expectedAparSummaryForUkprn1111111);

            _registerQueryRepository.Setup(r => r.GetAparSummary(2222222))
                .ReturnsAsync(_expectedAparSummaryForUkprn2222222);

            _sut = new GetAparSummaryByUkprnHandler(_registerQueryRepository.Object, _logger.Object);
        }

        [TestCase(1111111)]
        [TestCase(2222222)]
        public void Handle_ReturnExpectedListAparSummaryForSpecificUkprn_WhenHandlerIsInvoked(int ukprn)
        {
            var organisations = _sut.Handle(new GetAparSummaryByUkprnRequest(ukprn), new CancellationToken()).Result;

            organisations.Should().HaveCount(1, "because there should be exactly one apar summary with a matching ukprn");
            organisations.Single().Ukprn.Should().Be(ukprn);
        }

        [TestCase(1111111)]
        [TestCase(2222222)]
        public void Handle_GetAparSummaryByUkprnRequestIsCalledForSpecificUkprn_WhenHandlerIsInvoked(int ukprn)
        {
            _sut.Handle(new GetAparSummaryByUkprnRequest(ukprn), new CancellationToken()).Wait();
            _registerQueryRepository.Verify(r => r.GetAparSummary(ukprn));
        }
    }
}
