using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query
{
    public class WhenSearchByOrganisationSucceeds : ContactsQueryBase
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();
            MappingBootstrapper.Initialize();
            var contacts = Builder<Contact>.CreateListOfSize(10).Build().AsEnumerable();

            ContactQueryRepositoryMock.Setup(q => q.GetContactsForEpao(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((contacts)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult<bool>(true));

            var endPointAssessorOrganisationId = "EPA1234";
            _result = ContactQueryController.SearchContactsForAnOrganisation(endPointAssessorOrganisationId).Result;
        }


        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }
    }
}
