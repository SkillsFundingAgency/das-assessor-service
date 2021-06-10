using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateCheckControllerTestBase
    {
        public Certificate Certificate;

        protected CertificateCheckViewModel _viewModel;

        protected CertificateCheckController _certificateCheckController;

        protected IHttpContextAccessor SetupHttpContextAssessor()
        {
            return MockedHttpContextAccessor.Setup().Object;
        }

        protected ICertificateApiClient SetUpCertificateApiClient()
        {
            return MockedCertificateApiClient.Setup(Certificate, new Mock<ILogger<CertificateApiClient>>());
        }

        protected CertificateCheckViewModelValidator SetupValidator()
        {
            var MockStringLocalizer = new MockStringLocaliserBuilder();

            var localiser = MockStringLocalizer.Build<CertificateCheckViewModelValidator>();

            return new CertificateCheckViewModelValidator(localiser.Object);
        }

        protected ISessionService SetupSessionService()
        {
            var MockSession = new Mock<ISessionService>();

            var certificateSession = Builder<CertificateSession>
                .CreateNew()
                .With(q => q.CertificateId = Certificate.Id)
                .With(q => q.Options = new List<string>())
                .Build();

            var serialisedCertificateSession = JsonConvert.SerializeObject(certificateSession);

            MockSession.Setup(q => q.Get(nameof(CertificateSession))).Returns(serialisedCertificateSession);
            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId")).Returns("EPA00001");

            return MockSession.Object;
        }

        protected CertificateCheckViewModel SetupViewModel()
        {
            var viewModel = new CertificateCheckViewModel();
            viewModel.FromCertificate(Certificate);
            return viewModel;
        }
    }
}
