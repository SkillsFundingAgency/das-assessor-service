    using System.Collections.Generic;
    using System.Linq;
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

    namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
    {
        [TestFixture]
        public class RegisterQueryGetOrganisationsTests
        {
            protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
            protected GetAssessmentOrganisationsHandler GetAssessmentOrganisationSummarysHandler;
            protected Mock<ILogger<GetAssessmentOrganisationsHandler>> Logger;
            private List<AssessmentOrganisationSummary> _expectedOrganisationSummaries;
            private AssessmentOrganisationSummary _assessmentOrganisationSummary1;
            private AssessmentOrganisationSummary _assessmentOrganisationSummary2;

            [SetUp]
            public void Setup()
            {
                RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
                _assessmentOrganisationSummary1 = new AssessmentOrganisationSummary { Id = "EPA9999", Name = "Name 100", Ukprn = 777777 };
                _assessmentOrganisationSummary2 = new AssessmentOrganisationSummary { Id = "EPA8888", Name = "Name 10" };

                Logger = new Mock<ILogger<GetAssessmentOrganisationsHandler>>();

                _expectedOrganisationSummaries = new List<AssessmentOrganisationSummary>
                {
                    _assessmentOrganisationSummary1,
                    _assessmentOrganisationSummary2
                };

                RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisations())
                    .Returns(Task.FromResult(_expectedOrganisationSummaries.AsEnumerable()));

                GetAssessmentOrganisationSummarysHandler = new GetAssessmentOrganisationsHandler(RegisterQueryRepository.Object, Logger.Object);
            }

            [Test]
            public void GetAssessmentOrganisationSummarysRepoIsCalledWhenHandlerInvoked()
            {
                GetAssessmentOrganisationSummarysHandler.Handle(new GetAssessmentOrganisationsRequest(), new CancellationToken()).Wait();
                RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisations());
            }

            [Test]
            public void GetAssessmentOrganisationSummarysReturnedExpectedResults()
            {
                var organisations = GetAssessmentOrganisationSummarysHandler.Handle(new GetAssessmentOrganisationsRequest(), new CancellationToken()).Result;
                organisations.Count.Should().Be(2);
                organisations.Should().Contain(_assessmentOrganisationSummary1);
                organisations.Should().Contain(_assessmentOrganisationSummary2);
            }
        }
    }
