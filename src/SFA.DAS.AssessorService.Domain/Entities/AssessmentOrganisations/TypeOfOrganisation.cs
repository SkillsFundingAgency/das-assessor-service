namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    // TODO: TECH DEBT: This looks a lot like SFA.DAS.AssessorService.Domain.Entities.OrganisationType but is a struct type instead
    public struct TypeOfOrganisation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string TypeDescription { get; set; }
    }
}