using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateEmail;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts
{
    public class WhenUpdateEmailHandler
    {
        private Task _result;

        [SetUp]
        public void Arrange()
        {
            var contactRepositoryMock = new Mock<IContactRepository>();

            var updateEmailRequestMock =
                Builder<UpdateEmailRequest>.CreateNew()
                    .WithFactory(() => new UpdateEmailRequest()).Build();

            contactRepositoryMock.Setup(x => x.UpdateEmail(updateEmailRequestMock))
                .Returns(Task.CompletedTask);
            var updateEmailHandler = new UpdateEmailHandler(contactRepositoryMock.Object);
            _result = updateEmailHandler.Handle(updateEmailRequestMock, new CancellationToken());
        }

        [Test]
        public void Should_Update_Email()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().Be(TaskStatus.RanToCompletion);
        }
    }
}
