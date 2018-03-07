using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Put.Controller
{
    public class WhenUpdateContactSucceeds : ContactTestBase
    {
        private static UpdateContactRequest _updateContactRequest;
        private static ContactResponse _contactResponseQueryViewModel;
        private ActionResult _result;

        [SetUp]
        public async Task Arrange()
        {
            Setup();

            _contactResponseQueryViewModel = Builder<ContactResponse>.CreateNew().Build();

            _updateContactRequest = Builder<UpdateContactRequest>.CreateNew()
                .Build();

            await ContactController.UpdateContact(_updateContactRequest);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            Mediator.Verify(q => q.Send(Moq.It.IsAny<UpdateContactRequest>(), new CancellationToken()), Times.Once);
        }
    }
}