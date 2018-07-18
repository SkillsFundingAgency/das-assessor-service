using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }
    }
}
