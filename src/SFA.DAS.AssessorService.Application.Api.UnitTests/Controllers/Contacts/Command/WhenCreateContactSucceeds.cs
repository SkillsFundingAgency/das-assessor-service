using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Command
{
    public class WhenCreateContactSucceeds : ContactTestBase
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            MappingBootstrapper.Initialize();

            var contact = Builder<Contact>.CreateNew().Build();

            Mediator.Setup(q => q.Send(Moq.It.IsAny<CreateContactRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((contact)));

            var contactRequest = Builder<CreateContactRequest>.CreateNew()
                .Build();

            _result = ContactController.CreateContact(contactRequest).Result;
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            var result = _result as CreatedAtRouteResult;
            result.Should().NotBeNull();
        }
    }
}
