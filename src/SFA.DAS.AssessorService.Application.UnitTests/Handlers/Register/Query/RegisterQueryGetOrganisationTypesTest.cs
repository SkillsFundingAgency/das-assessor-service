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
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.ao;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetOrganisationTypesTest
    {
        protected Mock<IRegisterRepository> RegisterRepository;
        protected GetOrganisationTypesHandler GetOrganisationTypesHandler;
        protected Mock<ILogger<GetOrganisationTypesHandler>> Logger;
        private List<EpaOrganisationType> _expectedOrganisationTypes;
        private EpaOrganisationType _epaOrganisationType1;
        private EpaOrganisationType _epaOrganisationType2;

        [SetUp]
        public void Setup()
        {
            RegisterRepository = new Mock<IRegisterRepository>();
            _epaOrganisationType1 = new EpaOrganisationType { Id = 1, OrganisationType = "Type 1" };
            _epaOrganisationType2 = new EpaOrganisationType { Id = 2, OrganisationType = "Another Type" };

            Logger = new Mock<ILogger<GetOrganisationTypesHandler>>();
          
            _expectedOrganisationTypes = new List<EpaOrganisationType>
            {
                _epaOrganisationType1,
                _epaOrganisationType2
            };

            RegisterRepository.Setup(r => r.GetOrganisationTypes())
                .Returns(Task.FromResult(_expectedOrganisationTypes.AsEnumerable()));

            GetOrganisationTypesHandler = new GetOrganisationTypesHandler(RegisterRepository.Object, Logger.Object);
        }

        [Test]
        public void GetOrganisationTypesIsCalled()
        {
            GetOrganisationTypesHandler.Handle(new GetOrganisationTypesRequest(), new CancellationToken()).Wait();
            RegisterRepository.Verify(r => r.GetOrganisationTypes());
        }

        [Test]
        public void GetOrganisationTypesReturnedExpectedResults()
        {
            var organisationTypes = GetOrganisationTypesHandler.Handle(new GetOrganisationTypesRequest(), new CancellationToken()).Result;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_epaOrganisationType1);
            organisationTypes.Should().Contain(_epaOrganisationType2);
        }
    }
}
