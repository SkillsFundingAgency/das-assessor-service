﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/certificate")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly ApiClient _apiClient;

        public CertificateController(ILogger<CertificateController> logger, IHeaderInfo headerInfo, ApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateCertificates([FromBody] IEnumerable<CertificateData> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req => new BatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Username = _headerInfo.Username, CertificateData = req });

            var results = await _apiClient.CreateCertificates(bcRequest);

            return Ok(results);
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateCertificates([FromBody] IEnumerable<CertificateData> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req => new BatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Username = _headerInfo.Username, CertificateData = req });

            var results = await _apiClient.UpdateCertificates(bcRequest);

            return Ok(results);
        }

        [HttpPost("submit")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<SubmitBatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SubmitCertificates([FromBody] IEnumerable<SubmitCertificate> request)
        {
            IEnumerable<SubmitBatchCertificateRequest> scRequest = request.Select(req => new SubmitBatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Username = _headerInfo.Username, Uln = req.Uln, StandardCode = req.StandardCode, FamilyName = req.FamilyName, CertificateReference = req.CertificateReference });

            var results = await _apiClient.SubmitCertificates(scRequest);

            return Ok(results);
        }

        [HttpDelete("{uln}/{lastname}/{stdCode}/{certificateReference}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteCertificate(long uln, string lastname, int stdCode, string certificateReference)
        {
            DeleteCertificateRequest deleteRequest = new DeleteCertificateRequest { UkPrn = _headerInfo.Ukprn, Username = _headerInfo.Username, Uln = uln, Lastname = lastname, StandardCode = stdCode, CertificateReference = certificateReference};
            var error = await _apiClient.DeleteCertificate(deleteRequest);

            if (error is null)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}