
namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class Privileges
    {
        // This MUST be kept in line with dbo.Privileges
        public const string ViewCompletedAssessments = "View completed assessments";
        public const string ApplyForStandard = "Apply for a Standard";
        public const string ManageAPISubscription = "Manage API subscription";
        public const string RecordGrades = "Record grades and issue certificates";
        public const string ManageUsers = "Manage users";
        public const string ViewPipeline = "View pipeline";
    }
}
