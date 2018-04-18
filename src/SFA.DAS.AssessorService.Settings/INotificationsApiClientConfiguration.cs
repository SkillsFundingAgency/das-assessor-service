using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Settings
{
    public interface INotificationsApiClientConfiguration
    {
        string ApiBaseUrl { get; set; }
        string ClientToken { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string IdentifierUri { get; set; }
        string Tenant { get; set; }
    }
}
