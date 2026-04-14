using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/certificates")]
    [ValidateBadRequest]
    public class CertificateQueryController : Controller
    {
        private readonly IMediator _mediator;

        public CertificateQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetCertificate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificate(Guid id, bool includeLogs = false)
        {
            return Ok(await _mediator.Send(new GetCertificateRequest(id, includeLogs)));
        }

        [HttpGet("{uln}/{standardCode}", Name = "GetCertificateUlnAndStandardCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificateUlnAndStandardCode(long uln, int standardCode)
        {
            return Ok(await _mediator.Send(new GetCertificateUlnAndStandardCodeRequest { Uln = uln, StandardCode = standardCode }));
        }

        [HttpGet("uln/{uln}", Name = "GetCertificatesUln")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetCertificatesUlnResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificatesUln(long uln)
        {
            return Ok(await _mediator.Send(new GetCertificatesUlnRequest { Uln = uln }));
        }

        [HttpGet("contact/previousaddress", Name = "GetContactPreviousAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(CertificateAddress))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = null)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactPreviousAddress([FromQuery] string epaOrgId, [FromQuery] long? employerAccountId)
        {
            var address = await _mediator.Send(new GetContactPreviousAddressesRequest { EpaOrgId = epaOrgId, EmployerAccountId = employerAccountId });
            return Ok(address);
        }

        [HttpGet("history", Name = "GetCertificatesHistory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PaginatedList<CertificateSummaryResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificatesHistory(int pageIndex, string endPointAssessorOrganisationId, string searchTerm, string sortColumn, int sortDescending)
        {
            return Ok(await _mediator.Send(
                new GetCertificateHistoryRequest
                {
                    PageIndex = pageIndex,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    SortDescending = sortDescending == 0,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn
                }));
        }

        [HttpGet("standard/masks", Name = "GetStandardCertificateMasks")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(object))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardCertificateMasks([FromQuery(Name = "exclude")] long[] exclude)
        {
            if (exclude != null && exclude.Any(e => e <= 0))
            {
                return BadRequest(new { error = "Exclude ULN values must be positive integers." });
            }
            var request = new GetStandardCertificateMasksRequest { Exclude = exclude };
            var response = await _mediator.Send(request);
            return Ok(new { masks = response.Masks });
        }

        [HttpGet("framework/masks", Name = "GetFrameworkCertificateMasks")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(object))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetFrameworkCertificateMasks([FromQuery(Name = "exclude")] long[] exclude)
        {
            if (exclude != null && exclude.Any(e => e <= 0))
            {
                return BadRequest(new { error = "Exclude ULN values must be positive integers." });
            }
            var request = new GetFrameworkCertificateMasksRequest { Exclude = exclude };
            var response = await _mediator.Send(request);
            return Ok(new { masks = response.Masks });
        }

        [HttpGet("ready-to-print/count", Name = "GetCertificatesReadyToPrintCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(CertificatesForBatchNumberResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificatesReadyToPrintCount()
        {
            return Ok(await _mediator.Send(new GetCertificatesReadyToPrintCountRequest()));
        }

        [HttpGet("batch/{batchNumber}", Name = "GetCertificatesForBatchNumber")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(CertificatesForBatchNumberResponse))]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificatesForBatchNumber(int batchNumber)
        {
            var request = new GetCertificatesForBatchNumberRequest()
            {
                BatchNumber = batchNumber
            };

            var response = await _mediator.Send(request);
            if (response.Certificates.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }
    }
}