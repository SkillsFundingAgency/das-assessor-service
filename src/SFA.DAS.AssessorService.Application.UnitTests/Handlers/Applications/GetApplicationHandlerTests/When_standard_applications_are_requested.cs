using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    public class When_standard_applications_are_requested : GetApplicationsHandlerTestsBase
    {
        [Test]
        public async Task Then_standard_applications_are_retrieved()
        {
            await Handler.Handle(new GetApplicationsRequest(new Guid(), ApplicationTypes.Standard), new CancellationToken());
            ApplyRepository.Verify(r => r.GetStandardApplications(It.IsAny<Guid>()), Times.Once);
        }
    }
}