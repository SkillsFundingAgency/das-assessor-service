using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Queries.Get
{
    public class WhenSearchByOrganisationSucceeds : ContactsQueryBase
    {      
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var contacts = Builder<ContactResponse>.CreateListOfSize(10).Build().AsEnumerable();

            ContactQueryRepositoryMock.Setup(q => q.GetContacts(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((contacts)));

            var endPointAssessorOrganisationId = "1234";
            _result = ContactQueryController.SearchContactsForAnOrganisation(endPointAssessorOrganisationId).Result;
        }


        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            result.Should().NotBeNull();
        }
    }
}
