using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class MigrateContactOrganisation
    {
        public Contact contact { get; set; }

        public Organisation organisation { get; set; }
    }
}
