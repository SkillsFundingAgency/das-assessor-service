using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ILocationsApiClient
    {
        Task<List<AddressResponse>> SearchLocations(string query);
    }
}