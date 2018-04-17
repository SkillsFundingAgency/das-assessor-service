namespace SFA.DAS.AssessorService.Functions.Settings
{
    public interface IAuthSettings
    {
        string WtRealm { get; set; }
        string MetadataAddress { get; set; }
        string Role { get; set; }
    }
}