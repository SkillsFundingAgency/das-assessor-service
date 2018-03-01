﻿namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiAuthentication
    {
        string ClientId { get; set; }
        string Instance { get; set; }
        string Domain { get; set; }
        string TenantId { get; set; }
    }
}