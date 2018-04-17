using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices
{
    public class UpdateStatusService
    {
        private readonly HttpClient _httpClient;

        public UpdateStatusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

      
    }
}