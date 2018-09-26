﻿using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers.Certificate
{
    public class CertificateTestBase
    {
        protected Mock<ILogger<CertificateController>> LoggerMock;
        protected ApiClient ApiClientMock;
        protected HeaderInfo HeaderInfoMock;
        protected CertificateController ControllerMock;
        protected MockHttpMessageHandler MockHttp;

        protected void Setup()
        {
            BuildAutoMapper();
            BuildLoggerMock();
            BuildApiClientMock();
            BuildHeaderInfoMock();

            ControllerMock = new CertificateController(LoggerMock.Object, HeaderInfoMock, ApiClientMock);
        }

        private static void BuildAutoMapper()
        {
            AutoMapper.Mapper.Reset();
            StartupConfiguration.MappingStartup.AddMappings();
        }

        private void BuildLoggerMock()
        {
            LoggerMock = new Mock<ILogger<CertificateController>>();
        }

        private void BuildApiClientMock()
        {
            MockHttp = new MockHttpMessageHandler();

            var apiClientLogger = new Mock<ILogger<ApiClient>>();
            var tokenService = new Mock<ITokenService>();

            var httpClient = MockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:12726/");

            ApiClientMock = new ApiClient(httpClient, apiClientLogger.Object, tokenService.Object);
        }

        private void BuildHeaderInfoMock()
        {
            HeaderInfoMock = new HeaderInfo
            {
                Email = string.Empty,
                Ukprn = 0
            };
        }
    }
}
