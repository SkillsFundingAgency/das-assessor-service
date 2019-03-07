using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class ApplyTokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;

        public ApplyTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken()
        {
            return _configuration.ApplyApiAuthentication.ClientSecret;
        }
    }
}
