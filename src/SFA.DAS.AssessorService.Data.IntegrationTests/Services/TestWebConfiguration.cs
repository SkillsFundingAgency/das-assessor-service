﻿using System;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{

    public class TestWebConfiguration : IWebConfiguration
    {
        public AuthSettings Authentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ApiAuthentication ApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AzureApiAuthentication AzureApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ClientApiAuthentication ClientApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CertificateDetails CertificateDetails { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SftpSettings Sftp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AssessmentOrgsApiClientBaseUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string IfaApiClientBaseUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string IFATemplateStorageConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SqlConnectionString { get; set; }
        public string SpecflowDBTestConnectionString { get; set; }
        public string SessionRedisConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AuthSettings StaffAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ClientApiAuthentication ApplyApiAuthentication { get; set; }
        public string ApplyBaseAddress { get; set; }
        public string ServiceLink { get; set; }
        public DfeSignInConfig DfeSignIn { get; set; }
        public string RoatpApiClientBaseUrl { get; set; }
        public ClientApiAuthentication RoatpApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #region For External API Sandbox
        public string SandboxSqlConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ApiAuthentication SandboxApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ClientApiAuthentication SandboxClientApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion
    }
}
