using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    public class When_organisation_applications_are_requested : GetApplicationsHandlerTestsBase
    {
        [Test]
        public async Task Then_organisation_applications_are_retrieved()
        {
            await Handler.Handle(new GetApplicationsRequest(new Guid(), ApplicationTypes.Organisation), new CancellationToken());
            ApplyRepository.Verify(r => r.GetOrganisationApplications(It.IsAny<Guid>()), Times.Once);
        }
    }
}