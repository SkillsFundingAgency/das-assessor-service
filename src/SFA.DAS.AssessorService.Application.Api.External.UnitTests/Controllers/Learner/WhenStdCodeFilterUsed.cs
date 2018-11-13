using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers.Learner
{
    public class WhenStdCodeFilterUsed : LearnerTestBase
    {
        private List<Models.Certificates.Certificate> _items;

        [SetUp]
        public void Arrange()
        {
            base.Setup();

            // NOTE: The API call results List<SearchResult>
            List<SearchResult> _apiItems = new List<SearchResult>
            {
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 1234, UkPrn = 0 },
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 4321, UkPrn = 0 },
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 9999, UkPrn = 0 }
            };

            // NOTE: The end result maps these to List<Certificate>
            _items = _apiItems.Select(sr => base.BuildCertificate(sr.Uln, sr.FamilyName, sr.StdCode, sr.UkPrn)).ToList();

            MockHttp.When(HttpMethod.Post, "http://localhost:12726/api/v1/search")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(_apiItems));
        }

        [Test]
        public void ThenReturnsOnlyOneResult()
        {
            // arrange
            long uln = 1234;
            string familyName = "test";
            int stdCode = 1234;
            var expectedItems = _items.Where(sr => sr.CertificateData.Learner.Uln == uln && sr.CertificateData.Learner.FamilyName == familyName && sr.CertificateData.Standard.StandardCode == stdCode);

            // act
            var actionResult = ControllerMock.GetByStandardCode(uln, familyName, stdCode).Result as ObjectResult;
            var actualItems = actionResult.Value as List<Models.Certificates.Certificate>;

            // assert
            CollectionAssert.AreEquivalent(expectedItems, actualItems);
            Assert.That(actualItems, Has.Count.EqualTo(1));
        }
    }
}
