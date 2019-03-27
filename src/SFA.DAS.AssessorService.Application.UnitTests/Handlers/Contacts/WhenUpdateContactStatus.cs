using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts
{
    public class WhenUpdateContactStatus
    {
        private Task _result;

        [SetUp]
        public void Arrange()
        {
            var contactRepositoryMock = new Mock<IContactRepository>();

            var updateContactStatusRequestMock =
                Builder<UpdateContactStatusRequest>.CreateNew()
                    .WithConstructor(() => new UpdateContactStatusRequest(It.IsAny<string>(), It.IsAny<string>())).Build();

            contactRepositoryMock.Setup(x => x.UpdateStatus(updateContactStatusRequestMock))
                .Returns(Task.CompletedTask);
            var updateContactStatusHandler = new UpdateContactStatusHandler(contactRepositoryMock.Object);
            _result = updateContactStatusHandler.Handle(updateContactStatusRequestMock, new CancellationToken());
        }

        [Test]
        public void Should_Update_Status()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeEquivalentTo(TaskStatus.RanToCompletion);
        }
    }
}
