using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    public class When_applications_created_by_user_are_requested : GetApplicationsHandlerTestsBase
    {
        [Test]
        public async Task Then_application_created_by_orgainisation_are_returned()
        {
            //await Handler.Handle(new GetApplicationsRequest(new Guid(), true), new CancellationToken());
            //ApplyRepository.Verify(r => r.GetUserApplications(It.IsAny<Guid>()), Times.Once);
        }
    }
}