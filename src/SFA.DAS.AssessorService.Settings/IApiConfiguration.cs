using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiConfiguration
    {
        string Environment { get; set; }

        ApiAuthentication ApiAuthentication { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        QnaApiClientConfiguration QnaApiAuthentication { get; set; }
        
        RoatpApiClientConfiguration RoatpApiAuthentication { get; set; }
        ReferenceDataApiClientConfiguration ReferenceDataApiAuthentication { get; set; }
        CompaniesHouseApiClientConfiguration CompaniesHouseApiAuthentication { get; set; }
        CharityCommissionApiClientConfiguration CharityCommissionApiAuthentication { get; set; }

        OuterApiClientConfiguration OuterApi { get; set; }

        string SqlConnectionString { get; set; }
        string SandboxSqlConnectionString { get; set; }

        int PipelineCutoff { get; set; }
        string ServiceLink { get; set; }
        EmailTemplatesConfig EmailTemplatesConfig { get; set; }
    }
}