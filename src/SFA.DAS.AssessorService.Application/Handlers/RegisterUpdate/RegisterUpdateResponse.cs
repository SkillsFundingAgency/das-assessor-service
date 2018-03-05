namespace SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate
{
    public class RegisterUpdateResponse
    {
        public int EpaosOnRegister { get; set; }
        public int OrganisationsInDatabase { get; set; }
        public int DeletedOrganisationsInDatabase { get; set; }

        public int OrganisationsCreated { get; set; }
        public int OrganisationsDeleted { get; set; }
        public int OrganisationsUpdated { get; set; }
        public int OrganisationsUnDeleted { get; set; }
    }
}