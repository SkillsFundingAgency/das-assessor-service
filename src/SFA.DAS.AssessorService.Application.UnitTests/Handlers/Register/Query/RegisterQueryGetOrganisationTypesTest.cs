using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
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
    public class RegisterQueryGetOrganisationTypesTest
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetOrganisationTypesHandler GetOrganisationTypesHandler;
        protected Mock<ILogger<GetOrganisationTypesHandler>> Logger;
        private List<OrganisationType> _expectedOrganisationTypes;
        private OrganisationType _organisationType1;
        private OrganisationType _organisationType2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _organisationType1 = new OrganisationType { Id = 1, Type = "Type 1" };
            _organisationType2 = new OrganisationType { Id = 2, Type = "Another Type" };

            Logger = new Mock<ILogger<GetOrganisationTypesHandler>>();
          
            _expectedOrganisationTypes = new List<OrganisationType>
            {
                _organisationType1,
                _organisationType2
            };

            RegisterQueryRepository.Setup(r => r.GetOrganisationTypes())
                .Returns(Task.FromResult(_expectedOrganisationTypes.AsEnumerable()));

            GetOrganisationTypesHandler = new GetOrganisationTypesHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void GetOrganisationTypesRepoIsCalledWhenHandlerInvoked()
        {
            GetOrganisationTypesHandler.Handle(new GetOrganisationTypesRequest(), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetOrganisationTypes());
        }

        [Test]
        public void GetOrganisationTypesReturnedExpectedResults()
        {
            var organisationTypes = GetOrganisationTypesHandler.Handle(new GetOrganisationTypesRequest(), new CancellationToken()).Result;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_organisationType1);
            organisationTypes.Should().Contain(_organisationType2);
        }
    }
}
