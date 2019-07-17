﻿namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }
        CertificateDetails CertificateDetails { get; set; }
        SftpSettings Sftp { get; set; }
        string AssessmentOrgsApiClientBaseUrl { get; set; }
        string IfaApiClientBaseUrl { get; set; }
        string IFATemplateStorageConnectionString { get; set; }
        string SqlConnectionString { get; set; }
        string SpecflowDBTestConnectionString { get; set; }
        string SessionRedisConnectionString { get; set; }
        AuthSettings StaffAuthentication { get; set; }
        ClientApiAuthentication ApplyApiAuthentication { get; set; }
        string ApplyBaseAddress { get; set; }
        string ServiceLink { get; set; }
        DfeSignInConfig DfeSignIn { get; set; }

        string RoatpApiClientBaseUrl { get; set; }
        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        #region For External API Sandbox
        string SandboxSqlConnectionString { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        ClientApiAuthentication SandboxClientApiAuthentication { get; set; }
        #endregion
    }
}