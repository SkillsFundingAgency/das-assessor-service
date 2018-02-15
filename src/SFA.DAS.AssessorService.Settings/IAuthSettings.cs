namespace SFA.DAS.AssessorService.Settings
{
    public interface IAuthSettings
    {
        string WtRealm { get; set; }
        string MetadataAddress { get; set; }
    }
}