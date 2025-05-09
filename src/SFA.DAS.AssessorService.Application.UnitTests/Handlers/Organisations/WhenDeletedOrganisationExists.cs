﻿using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
    [TestFixture]
    public class WhenDeletedOrganisationExists : MapperBase
    {
        private CreateOrganisationHandler _handler;
        private Mock<IOrganisationRepository> _orgRepos;
        private Mock<IContactRepository> _contactRepository;

        [SetUp]
        public void Arrange()
        {
            _orgRepos = new Mock<IOrganisationRepository>();
            _contactRepository = new Mock<IContactRepository>();

            var orgQueryRepos = new Mock<IOrganisationQueryRepository>();
            orgQueryRepos.Setup(r => r.GetByUkPrn(It.IsAny<long>())).ReturnsAsync(new Organisation()
            {
                Status = OrganisationStatus.Deleted,
                EndPointAssessorOrganisationId = "12345"
            });

            _orgRepos.Setup(r => r.UpdateOrganisation(It.IsAny<Organisation>()))
                .ReturnsAsync(new Organisation());

            _handler = new CreateOrganisationHandler(_orgRepos.Object,             
                orgQueryRepos.Object,
                _contactRepository.Object,
                Mapper);
        }

        [Test]
        public void ThenNewOrgIsNotCreated()
        {
            _orgRepos.Setup(r => r.CreateNewOrganisation(It.IsAny<Organisation>()))
                .Throws(new Exception("Should not be called"));
            _handler.Handle(new CreateOrganisationRequest(){EndPointAssessorOrganisationId = "12345", EndPointAssessorUkprn = 123}, new CancellationToken()).Wait();
        }

        [Test]
        public void ThenExistingOrgIsUpdated()
        {
            _handler.Handle(new CreateOrganisationRequest(){ EndPointAssessorOrganisationId = "12345", EndPointAssessorUkprn = 123 }, new CancellationToken()).Wait();
            _orgRepos.Verify(r =>
                r.UpdateOrganisation(
                    It.Is<Organisation>(m => m.EndPointAssessorOrganisationId == "12345")));
        }
    }
}