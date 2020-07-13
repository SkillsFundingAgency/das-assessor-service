using System.Threading;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenSearchOrganisationSucceeds : OrganisationQueryBase
    {
        private const long Ukprn = 10000000;
        private OrganisationResponse _organisationResponse;
        private IActionResult _result;
      
        [SetUp]
        public void Arrange()
        {
            Setup();

            var fixture = new Fixture();
            _organisationResponse = fixture.Create<OrganisationResponse>();

            MediatorMock
                .Setup(q => q.Send(It.Is<GetOrganisationByUkprnRequest>(r => r.Ukprn.Equals(Ukprn)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_organisationResponse);

            _result = OrganisationQueryController.SearchOrganisation(Ukprn).Result;
        }

        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheQueryReturnsTheCorrectResult()
        {
            if(!(_result is OkObjectResult result))
                ((OkObjectResult) null).Should().NotBeNull();
            else 
                result.Value.Should().BeOfType<OrganisationResponse>();

        }  
    }
}