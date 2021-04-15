using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Web.ViewModels.Shared
{
    public class StandardVersionViewModel
    {
        public string Title { get; set; }
        public string StandardUId { get; set; }
        public string Version { get; set; }

        public static implicit operator StandardVersionViewModel(StandardVersion version)
        {
            return new StandardVersionViewModel
            {
                StandardUId = version.StandardUId,
                Title = version.Title,
                Version = version.Version
            };
        }
    }
}
