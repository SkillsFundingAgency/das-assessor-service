
namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class Privileges
    {
        // This MUST be kept in line with dbo.Privileges
        public const string ViewCompletedAssessments = "ViewCompletedAssessments";
        public const string ApplyForStandard = "ApplyForStandard";
        public const string ManageAPISubscription = "ManageAPISubscription";
        public const string RecordGrades = "RecordGrades";
        public const string ManageUsers = "ManageUsers";
        public const string ViewPipeline = "ViewPipeline";
    }
}
