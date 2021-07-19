namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EPORegisteredStandards
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string ReferenceNumber { get; set; }
        public bool NewVersionAvailable { get; set; }
        public int NumberOfVersions { get; set; }
    }
}
