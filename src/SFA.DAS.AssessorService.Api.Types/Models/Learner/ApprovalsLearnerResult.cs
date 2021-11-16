namespace SFA.DAS.AssessorService.Api.Types.Models.Learner
{
    public class ApprovalsLearnerResult
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }
        public string Version { get; set; }
        public bool VersionConfirmed { get; set; }
        public string CourseOption { get; set; }
    }
}
