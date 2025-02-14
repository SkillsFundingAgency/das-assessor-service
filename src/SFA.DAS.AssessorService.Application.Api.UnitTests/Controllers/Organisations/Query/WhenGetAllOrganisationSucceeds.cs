using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using Moq;
using Models = SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenGetAllOrganisationsSucceeds : OrganisationQueryBase
    {
        private List<Organisation> _organisations = new List<Organisation>();
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisations = Builder<Organisation>
                .CreateListOfSize(3)
                .All()
                .With(o => o.OrganisationData = Builder<OrganisationData>.CreateNew().Build()) 
                .With(o => o.OrganisationType = Builder<OrganisationType>.CreateNew().Build()) 
                .Build()
                .ToList();

            OrganisationQueryRepositoryMock.Setup(q => q.GetAllOrganisations())
                .Returns(Task.FromResult((_organisations.AsEnumerable())));

            var mappedOrganisations = Builder<Models.OrganisationResponse>
                .CreateListOfSize(3)
                .Build()
                .ToList();

            MapperMock.Setup(m => m.Map<List<Models.OrganisationResponse>>(_organisations)) 
                .Returns(mappedOrganisations); 

            _result = OrganisationQueryController.GetAllOrganisations().Result;
        }

        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheQueryReturnsTheCorrectResultType()
        {
            if (!(_result is OkObjectResult result))
                ((OkObjectResult)null).Should().NotBeNull();
            else
                result.Value.Should().BeOfType<List<Models.OrganisationResponse>>();

        }

        [Test]
        public void ThenTheQueryReturnsTheCorrectResultContent()
        {
            if (!(_result is OkObjectResult result))
            {
                ((OkObjectResult)null).Should().NotBeNull();
            }
            else
            {
                var organisationResponses = result.Value as List<Models.OrganisationResponse>;
                organisationResponses.Should().NotBeNull();
                organisationResponses.Count.Should().Be(_organisations.Count);

                MapperMock.Verify(m => m.Map<List<Models.OrganisationResponse>>(_organisations), Times.Once); 
            }
        }
    }
}