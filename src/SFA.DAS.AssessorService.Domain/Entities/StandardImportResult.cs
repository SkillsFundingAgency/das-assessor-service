namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class StandardImportResult
    {
        public int StandardsInserted { get; set; }
        public int StandardsUpdated { get; set; }
        public int OptionsInserted { get; set; }
        public int OptionsDeleted { get; set; }
    }
}
