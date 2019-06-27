using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learner;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetLearnerExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new GetLearner
            {
                //RequestId = "3",
                //Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                //Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                //LearningDetails = new LearningDetails { CourseOption = null, OverallGrade = "Credit", AchievementDate = DateTime.UtcNow },
                //PostalContact = new PostalContact { ContactName = "Alan Brewer", Department = "Human Resources", Organisation = "Fabrikam Inc", AddressLine1 = "Outlook Place", City = "Lorem Ipsum", PostCode = "ZY9 9ZZ" }
            };
        }
    }
}
