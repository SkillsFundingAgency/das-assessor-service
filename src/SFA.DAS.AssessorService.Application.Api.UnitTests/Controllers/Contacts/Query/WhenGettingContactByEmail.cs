using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query
{
    public class WhenGettingContactByEmail : ContactsQueryBase
    {
        private IActionResult _result;
        private string searchEmail = "test@email.com";
        private Contact _contact;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _contact = Builder<Contact>.CreateNew().Build();

            ContactQueryRepositoryMock.Setup(q => q.GetContactFromEmailAddress(searchEmail))
                .ReturnsAsync(_contact);

        }

        [Test]
        public async Task ThenTheResultReturnsOkStatus()
        {
            _result = await ContactQueryController.SearchContactByEmail(searchEmail);
            
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
            var actualModel = result!.Value as ContactResponse;
            actualModel.Should().NotBeNull();
            actualModel.Should().BeEquivalentTo(_contact, options=> options.ExcludingMissingMembers());
        }

        [Test]
        public void Then_If_Null_Returned_Exception_Thrown()
        {
            Assert.ThrowsAsync<ResourceNotFoundException>(()=> ContactQueryController.SearchContactByEmail("searchEmail"));
        }
    }    
}

