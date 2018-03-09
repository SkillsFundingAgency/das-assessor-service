﻿using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Post.Handlers
{
 public class WhenCreateOrganisationHandlerSucceeds
    {
        private Mock<IOrganisationRepository> _organisationRepositoryMock;
        private OrganisationResponse _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            CreateOrganisationRepositoryMock();

            var createOrganisationRequest = Builder<CreateOrganisationRequest>.CreateNew().Build();        
            var organisationQueryRepository = CreateOrganisationQueryRepository();

            var createOrganisationHandler = new CreateOrganisationHandler(_organisationRepositoryMock.Object, organisationQueryRepository.Object);
            _result = createOrganisationHandler.Handle(createOrganisationRequest, new CancellationToken()).Result;
        }        

        [Test]
        public void ItShouldReturnAResult()
        { 
            var result = _result as OrganisationResponse;
            result.Should().NotBeNull();
        }


        [Test]
        public void ItShouldReturnAnOrganisation()
        {
            _result.Should().BeOfType<OrganisationResponse>();
        }

        [Test]
        public void ItShouldPersistTheData()
        {
            _organisationRepositoryMock.Verify(q => q.CreateNewOrganisation(Moq.It.IsAny<CreateOrganisationDomainModel>()), Times.Once);
        }

        private static Mock<IOrganisationQueryRepository> CreateOrganisationQueryRepository()
        {
            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            organisationQueryRepository.Setup(r => r.GetByUkPrn(Moq.It.IsAny<int>())).ReturnsAsync(default(OrganisationResponse));
            return organisationQueryRepository;
        }

        private void CreateOrganisationRepositoryMock()
        {
            _organisationRepositoryMock = new Mock<IOrganisationRepository>();
            var organisation = Builder<OrganisationResponse>.CreateNew().Build();
            _organisationRepositoryMock.Setup(q => q.CreateNewOrganisation(Moq.It.IsAny<CreateOrganisationDomainModel>()))
                .Returns(Task.FromResult((organisation)));
        }
    }
}

