namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Organisation : ApplyTypeBase
    {
        public string Name { get; set; }
        public string OrganisationType { get; set; }
        public int? OrganisationUkprn { get; set; }
        public OrganisationDetails OrganisationDetails { get; set; }

        public bool RoEPAOApproved { get; set; }
        public bool RoATPApproved { get; set; }
    }

   
}