using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Command
{
    public class WhenUpdateEmailSucceeds : ContactTestBase
    {
        private static UpdateEmailRequest _updateEmailRequest;

        [SetUp]
        public async Task Arrange()
        {
            Setup();

            Builder<ContactResponse>.CreateNew().Build();

            _updateEmailRequest = Builder<UpdateEmailRequest>.CreateNew().Build();

            await ContactController.UpdateEmail(_updateEmailRequest);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            Mediator.Verify(q => q.Send(It.IsAny<UpdateEmailRequest>(), new CancellationToken()), Times.Once);
        }
    }
}
