using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_returned_learners_do_not_match_users_epaorgid : MapperBase
    {
        [Test]
        public void Then_non_matching_are_not_returned_if_not_valid_for_epao()
        {
            var learnerRepository = new Mock<ILearnerRepository>();

            learnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner>
                {
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James"},
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0002", StdCode = 2, FamilyName = "James"},
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James"}
                });

            var standardService = new Mock<IStandardService>();
            standardService.Setup(c => c.GetEPAORegisteredStandardVersions(It.IsAny<string>(), null))
                .ReturnsAsync(new List<OrganisationStandardVersion> { new OrganisationStandardVersion { Title = "Standard One", Version = "1.0", LarsCode = 1 },
                                                            new OrganisationStandardVersion { Title = "Standard Two", Version = "1.0", LarsCode = 2 } });
            standardService.Setup(c => c.GetStandardVersionById(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Standard { Title = "Standard Title", Level = 2 });


            var organisationRepository = new Mock<IOrganisationQueryRepository>();
            organisationRepository.Setup(r => r.Get("12345")).ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0001" });

            var certificateRepository = new Mock<ICertificateRepository>();
            certificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());


            var handler = new SearchHandler(organisationRepository.Object, learnerRepository.Object,
                certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object, standardService.Object, Mapper);

            var result = handler.Handle(new LearnerSearchRequest { Surname = "James", Uln = 1111111111, EpaOrgId = "12345", Username = "user@name" }, new CancellationToken()).Result;

            result.Count.Should().Be(2);
        }

        [Test]
        public void Then_non_matching_are_returned_if_valid_for_epao()
        {
            var learnerRepository = new Mock<ILearnerRepository>();

            learnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner>
                {
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James"},
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0002", StdCode = 2, FamilyName = "James"},
                    new Domain.Entities.Learner{ EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James"}
                });

            var assessmentOrgsApiClient = new Mock<IRegisterQueryRepository>();
            var standardService = new Mock<IStandardService>();

            standardService.Setup(c => c.GetEPAORegisteredStandardVersions(It.IsAny<string>(), null))
                .ReturnsAsync(new List<OrganisationStandardVersion> { new OrganisationStandardVersion { Title = "Standard One", Version = "1.0", LarsCode = 1 },
                                                          new OrganisationStandardVersion { Title = "Standard Two", Version = "1.0", LarsCode = 2 },
                                                          new OrganisationStandardVersion { Title = "Standard Three", Version = "1.0", LarsCode = 3 }});

            standardService.Setup(c => c.GetStandardVersionById(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Standard { Title = "Standard Title", Level = 2 });


            var organisationRepository = new Mock<IOrganisationQueryRepository>();
            organisationRepository.Setup(r => r.Get("12345")).ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0001" });

            var certificateRepository = new Mock<ICertificateRepository>();
            certificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());


            var handler = new SearchHandler(organisationRepository.Object, learnerRepository.Object,
                certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object, standardService.Object, Mapper);

            var result = handler.Handle(new LearnerSearchRequest { Surname = "James", Uln = 1111111111, EpaOrgId = "12345", Username = "user@name" }, new CancellationToken()).Result;

            result.Count.Should().Be(3);
        }
    }
}