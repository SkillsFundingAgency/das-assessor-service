using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.UnitTests.Infrastructure.OuterApi
{
    public class WhenCreatingTheGetStandardsRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            //Act
            var actual = new GetStandardsRequest();
            
            //Assert
            actual.GetUrl.Should().Be("trainingcourses");
        }
    }
}