using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Queries.Get
{
    public class WhenSearchByUserNameSucceeds : ContactsQueryBase
    {
        private static ContactResponse _contactResponse;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _contactResponse = Builder<ContactResponse>.CreateNew().Build();

            ContactQueryRepositoryMock.Setup(q => q.GetContact(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((_contactResponse)));

            _result = ContactQueryController.SearchContactByUserName("TestUser").Result;
        }

        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheResultReturnsValidData()
        {
            var result = _result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            (result.Value is ContactResponse).Should().BeTrue();
        }
    }
}
