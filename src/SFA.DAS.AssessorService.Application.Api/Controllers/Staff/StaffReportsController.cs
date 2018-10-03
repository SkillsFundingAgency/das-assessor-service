using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/staffreports/")]
    [ValidateBadRequest]
    public class StaffReportsController : Controller
    {
        private readonly IStaffReportRepository _staffReportRepository;
        private readonly ILogger<StaffReportsController> _logger;

        public StaffReportsController(IStaffReportRepository staffReportingRepository, ILogger<StaffReportsController> logger)
        {
            _staffReportRepository = staffReportingRepository;
            _logger = logger;
        }

        [HttpGet()]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StaffReport>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetReportList()
        {
            _logger.LogInformation($"Received request to get list of reports");

            var reportList = await _staffReportRepository.GetReportList();

            return Ok(reportList);
        }

        [HttpGet("{reportId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetReport(Guid reportId)
        {
            _logger.LogInformation($"Received request to generate report : {reportId}");

            try
            {
                var result = await _staffReportRepository.GetReport(reportId);

                return Ok(result);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate report due to : {sqlEx.Message}");

                return NoContent();
            }
        }
    }
}