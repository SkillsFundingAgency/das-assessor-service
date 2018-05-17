using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_search_contains_any_type_of_special_character : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
        }
        
        [TestCase("o`neill", "o_neill")]
        [TestCase("o'neill", "o_neill")]
        [TestCase("o’neill", "o_neill")]
        [TestCase("Oxlade-Chamberlain", "Oxlade_Chamberlain")]
        [TestCase("Oxlade–Chamberlain", "Oxlade_Chamberlain")]
        [TestCase("Oxlade – o`neill", "Oxlade _ o_neill")]
        public void Then_search_is_swapped_to_a_LIKE_with_the_character_replaced(string surname, string likedSurname)
        {
            SearchHandler.Handle(new SearchQuery(){Surname = surname, UkPrn = 12345, Uln = 12345, Username = "dave"}, new CancellationToken()).Wait();
            
            IlrRepository.Verify(r => r.SearchForLearnerLike(It.Is<SearchRequest>(sr => sr.FamilyName == likedSurname)));
        }
    }
}