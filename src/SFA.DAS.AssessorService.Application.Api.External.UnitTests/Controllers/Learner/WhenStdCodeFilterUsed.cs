using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.External.Models.Search;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers.Learner
{
    public class WhenStdCodeFilterUsed : LearnerTestBase
    {
        private List<SearchResult> _items;

        [SetUp]
        public void Arrange()
        {
            base.Setup();

            _items = new List<SearchResult>
            {
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 1234, UkPrn = 0 },
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 4321, UkPrn = 0 },
                new SearchResult { Uln = 1234, FamilyName = "test", StdCode = 9999, UkPrn = 0 }
            };

            MockHttp.When(HttpMethod.Post, "http://localhost:12726/api/v1/search")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(_items));
        }

        [Test]
        public void ThenReturnsOnlyOneResult()
        {
            // arrange
            long uln = 1234;
            string familyName = "test";
            int stdCode = 1234;
            var expectedItems = _items.Where(sr => sr.Uln == uln && sr.FamilyName == familyName && sr.StdCode == stdCode);

            // act
            var actionResult = ControllerMock.GetByStandardCode(uln, familyName, stdCode).Result as ObjectResult;
            var actualItems = actionResult.Value as List<SearchResult>;

            // assert
            CollectionAssert.AreEquivalent(expectedItems, actualItems);
            Assert.That(actualItems, Has.Count.EqualTo(1));
        }
    }
}
