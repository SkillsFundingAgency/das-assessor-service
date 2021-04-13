namespace SFA.DAS.AssessorService.Web.ViewModels.Shared
{
    public class StandardVersion
    {
        public string Title { get; set; }
        public string StandardUId { get; set; }
        public string Version { get; set; }

        public static implicit operator StandardVersion(Api.Types.Models.Standards.StandardVersion version)
        {
            return new StandardVersion
            {
                StandardUId = version.StandardUId,
                Title = version.Title,
                Version = version.Version
            };
        }
    }
}
