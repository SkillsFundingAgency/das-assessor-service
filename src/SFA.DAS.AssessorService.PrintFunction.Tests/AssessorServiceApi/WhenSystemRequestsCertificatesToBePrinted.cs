﻿using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunction.Tests.AssessorServiceApi
{
    public class WhenSystemRequestsCertificatesToBePrinted : AssessorServiceTestBase
    {
        private IEnumerable<CertificateResponse> _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();

            MockHttp.When("http://localhost:59022/api/v1/certificates?statuses=Submitted&statuses=Reprint")
                .Respond("application/json", JsonConvert.SerializeObject(certificateResponses)); // Respond with JSON

            _result = AssessorServiceApi.GetCertificatesToBePrinted().Result;
        }

        [Test]
        public void ThenItShouldReturnValidCertificates()
        {
            _result.Count().Should().Be(10);
        }
    }
}
