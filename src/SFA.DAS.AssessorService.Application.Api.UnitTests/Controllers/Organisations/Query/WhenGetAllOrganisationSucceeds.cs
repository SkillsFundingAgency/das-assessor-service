using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Models = SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

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

                for (int i = 0; i < _organisations.Count; i++)
                {
                    var expected = _organisations[i];
                    var actual = organisationResponses[i];

                    actual.Id.Should().Be(expected.Id);
                    actual.PrimaryContact.Should().Be(expected.PrimaryContact);
                    actual.Status.Should().Be(expected.Status);
                    actual.EndPointAssessorName.Should().Be(expected.EndPointAssessorName);
                    actual.EndPointAssessorOrganisationId.Should().Be(expected.EndPointAssessorOrganisationId);
                    actual.EndPointAssessorUkprn.Should().Be(expected.EndPointAssessorUkprn);
                    actual.RoATPApproved.Should().Be(expected.OrganisationData.RoATPApproved);
                    actual.RoEPAOApproved.Should().Be(expected.OrganisationData.RoEPAOApproved);
                    actual.OrganisationType.Should().Be(expected.OrganisationType?.Type);
                    actual.CompanySummary.Should().Be(expected.OrganisationData?.CompanySummary);
                    actual.CharitySummary.Should().Be(expected.OrganisationData?.CharitySummary);

                    var properties = typeof(Models.OrganisationResponse).GetProperties();
                    foreach (var property in properties)
                    {
                        var mappedFields = new HashSet<string>
                        {
                            "Id", "PrimaryContact", "Status", "EndPointAssessorName",
                            "EndPointAssessorOrganisationId", "EndPointAssessorUkprn",
                            "RoATPApproved", "RoEPAOApproved", "OrganisationType",
                            "CompanySummary", "CharitySummary"
                        };

                        if (!mappedFields.Contains(property.Name))
                        {
                            var value = property.GetValue(actual);
                            value.Should().BeNull($"Property {property.Name} should not be mapped and should be null");
                        }
                    }

                }
            }
        }
    }
}