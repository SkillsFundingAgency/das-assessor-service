namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        CertificateDetails CertificateDetails { get; set; }
        SftpSettings Sftp { get; set; }
        string SqlConnectionString { get; set; }
    }
}