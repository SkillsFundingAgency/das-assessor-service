using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Models.Search;
using System.Collections.Generic;
using System.Linq;

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

            base.SetFakeHttpMessageHandlerResponse(System.Net.HttpStatusCode.OK, _items);
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
            var actionResult = ControllerMock.Get(uln, familyName, stdCode).Result as ObjectResult;
            var actualItems = actionResult.Value as List<SearchResult>;

            // assert
            CollectionAssert.AreEquivalent(expectedItems, actualItems);
            Assert.That(actualItems, Has.Count.EqualTo(1));
        }
    }
}
