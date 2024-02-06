using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;
using SFA.DAS.AssessorService.Settings;
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{

    public class TestWebConfiguration : IWebConfiguration
    {
        public string Environment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ApiAuthentication ApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AzureApiClientConfiguration AzureApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SqlConnectionString { get; set; }
        public string SpecflowDBTestConnectionString { get; set; }
        public string SessionRedisConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ManagedIdentityClientConfiguration QnaApiAuthentication { get; set; }
        public string ServiceLink { get; set; }
        public LoginServiceConfig LoginService { get; set; }
        public AzureActiveDirectoryClientConfiguration RoatpApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AzureActiveDirectoryClientConfiguration ReferenceDataApiAuthentication { get; set; }
        public CompaniesHouseApiClientConfiguration CompaniesHouseApiAuthentication { get; set; }
        public CharityCommissionApiClientConfiguration CharityCommissionApiAuthentication { get; set; }
        public string ReferenceFormat { get; set; }
        public string FeedbackUrl { get; set; }
        public int PipelineCutoff { get; set; }

        #region For External API Sandbox
        public string SandboxSqlConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ApiAuthentication SandboxApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AzureActiveDirectoryClientConfiguration SandboxAssessorApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public string ZenDeskSnippetKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ZenDeskSectionId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ZenDeskCobrowsingSnippetKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OuterApiClientConfiguration OuterApi { get; set; }
        public bool UseGovSignIn { get; set; }
    }
}
