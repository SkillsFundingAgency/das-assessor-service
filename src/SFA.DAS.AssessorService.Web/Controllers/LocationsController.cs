using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("locations")]
    public class LocationsController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILocationsApiClient _locationsApiClient;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(
            IHttpContextAccessor contextAccessor,
            ILocationsApiClient locationsApiClient,
            ILogger<LocationsController> logger)
        {
            _contextAccessor = contextAccessor;
            _locationsApiClient = locationsApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<List<AddressResponse>> Index([FromQuery] string query, [FromQuery] bool includeOrganisations)
        {
            try
            {
                var locations = await _locationsApiClient.SearchLocations(query);
                if (!locations.Any())
                {
                    locations.Add(AddressResponse.NoResultsFound);
                }
                else if(!includeOrganisations)
                {
                    locations.ForEach(p => p.Organisation = string.Empty);
                }
                
                 return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error getting the locations for {query}");
            }

            return new List<AddressResponse>();
        }
    }
}