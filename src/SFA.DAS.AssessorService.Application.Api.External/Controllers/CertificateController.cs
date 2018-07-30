using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models;
using SFA.DAS.AssessorService.Application.Api.External.Validators;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using SearchQuery = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchQuery;
using SearchResult = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchResult;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/certificates")]
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

        [HttpPut(Name = "Put")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateDraft([FromBody] List<CertificateRequest> request)
        {
            List<CertificateResponse> response = new List<CertificateResponse>();

            foreach (var validatedCertificate in await ValidateCertificateRequest(request, _headerInfo.Ukprn, _headerInfo.Username))
            {
                CertificateResponse certificateResponse = new CertificateResponse
                {
                    Uln = validatedCertificate.Uln,
                    LastName = validatedCertificate.LastName,
                    StdCode = validatedCertificate.StdCode,
                    CertificateData = validatedCertificate.CertificateData,
                    Certificate = null,
                    Status = CertificateStatus.Error,
                    ValidationErrors = new List<string>()
                };

                if (!validatedCertificate.IsVaild)
                {
                    certificateResponse.ValidationErrors.AddRange(validatedCertificate.ValidationErrors);
                }
                else
                {
                    var scr = new AssessorService.Api.Types.Models.Certificates.StartCertificateRequest { Uln = validatedCertificate.Uln, StandardCode = validatedCertificate.StdCode, UkPrn = _headerInfo.Ukprn, Username = _headerInfo.Username };

                    Certificate startCertificate = await _apiClient.StartCertificate(scr);

                    CertificateResponse updateCertificateResponse = await UpdateDraftCertificate(startCertificate, validatedCertificate.CertificateData, _headerInfo.Username, CertificateStatus.Draft, CertificateActions.Start);

                    certificateResponse.Certificate = updateCertificateResponse.Certificate;
                    certificateResponse.Status = updateCertificateResponse.Status;
                    certificateResponse.ValidationErrors.AddRange(updateCertificateResponse.ValidationErrors);
                }

                response.Add(certificateResponse);
            }

            return Ok(response);
        }

        [HttpPost(Name = "Post")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateDraft([FromBody] List<CertificateRequest> request)
        {
            List<CertificateResponse> response = new List<CertificateResponse>();

            foreach (var validatedCertificate in await ValidateCertificateRequest(request, _headerInfo.Ukprn, _headerInfo.Username))
            {
                CertificateResponse certificateResponse = new CertificateResponse
                {
                    Uln = validatedCertificate.Uln,
                    LastName = validatedCertificate.LastName,
                    StdCode = validatedCertificate.StdCode,
                    CertificateData = validatedCertificate.CertificateData,
                    Certificate = null,
                    Status = CertificateStatus.Error,
                    ValidationErrors = new List<string>()
                };

                if (!validatedCertificate.IsVaild)
                {
                    certificateResponse.ValidationErrors.AddRange(validatedCertificate.ValidationErrors);
                }
                else
                {
                    var gcfuRequest = new AssessorService.Api.Types.Models.Certificates.GetCertificateForUlnRequest { Uln = validatedCertificate.Uln, StandardCode = validatedCertificate.StdCode };

                    Certificate certificate = await _apiClient.GetCertificateForUln(gcfuRequest);

                    CertificateResponse updateCertificateResponse = await UpdateDraftCertificate(certificate, validatedCertificate.CertificateData, _headerInfo.Username, CertificateStatus.Draft, CertificateActions.Start);

                    certificateResponse.Certificate = updateCertificateResponse.Certificate;
                    certificateResponse.Status = updateCertificateResponse.Status;
                    certificateResponse.ValidationErrors.AddRange(updateCertificateResponse.ValidationErrors);
                }

                response.Add(certificateResponse);
            }

            return Ok(response);
        }

        [HttpPost(Name = "Submit")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SubmitDraft([FromBody] List<CertificateRequest> request)
        {
            List<CertificateResponse> response = new List<CertificateResponse>();

            foreach (var cert in request)
            {
                var gcfuRequest = new AssessorService.Api.Types.Models.Certificates.GetCertificateForUlnRequest { Uln = cert.Uln, StandardCode = cert.StdCode };

                Certificate certificate = await _apiClient.GetCertificateForUln(gcfuRequest);

                CertificateResponse certificateResponse = new CertificateResponse
                {
                    Uln = cert.Uln,
                    LastName = cert.LastName,
                    StdCode = cert.StdCode,
                    CertificateData = null,
                    Certificate = null,
                    Status = CertificateStatus.Error,
                    ValidationErrors = new List<string>()
                };

                CertificateResponse updateCertificateResponse = await UpdateDraftCertificate(certificate, null, _headerInfo.Username, CertificateStatus.Submitted, CertificateActions.Submit);

                certificateResponse.Certificate = updateCertificateResponse.Certificate;
                certificateResponse.Status = updateCertificateResponse.Status;
                certificateResponse.ValidationErrors.AddRange(updateCertificateResponse.ValidationErrors);

                response.Add(certificateResponse);
            }

            return Ok(response);
        }

        [HttpDelete("{uln}/{lastname}/{stdCode}", Name = "Delete")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteDraft(long uln, string lastname, int stdCode)
        {
            var gcfuRequest = new AssessorService.Api.Types.Models.Certificates.GetCertificateForUlnRequest { Uln = uln, StandardCode = stdCode };

            Certificate cert = await _apiClient.GetCertificateForUln(gcfuRequest);

            if (cert == null)
            {
                return BadRequest("not found");
            }
            else if (cert.Status != CertificateStatus.Draft)  // TODO: Are there any other status which is valid?
            {
                return BadRequest("not applicable");
            }
            else
            {
                cert.Status = CertificateStatus.Deleted;

                var ucRequest = new AssessorService.Api.Types.Models.Certificates.UpdateCertificateRequest(cert);
                ucRequest.Action = CertificateActions.Start;
                ucRequest.Username = _headerInfo.Username;

                Certificate updatedCertificate = await _apiClient.UpdateCertificate(ucRequest);

                return Ok();
            }
        }

        private async Task<List<ValidatedCertificate>> ValidateCertificateRequest(List<CertificateRequest> certificateRequest, int ukPrn, string username)
        {
            List<ValidatedCertificate> response = new List<ValidatedCertificate>();

            CertificateDataValidator validator = new CertificateDataValidator();

            foreach (var request in certificateRequest)
            {
                List<string> validationErrors = new List<string>();

                SearchQuery searchQuery = new SearchQuery
                {
                    Uln = request.Uln,
                    Surname = request.LastName,
                    UkPrn = ukPrn,
                    Username = username
                };

                List<SearchResult> searchResults = await _apiClient.Search(searchQuery, request.StdCode);
                SearchResult learnerRecord = searchResults.FirstOrDefault();

                if (learnerRecord != null)
                {
                    ValidationResult validationResult = validator.Validate(request.CertificateData);

                    if (!validationResult.IsValid)
                    {
                        foreach (var error in validationResult.Errors)
                        {
                            validationErrors.Add($"{error.PropertyName} - {error.ErrorMessage}");
                        }
                    }
                }
                else
                {
                    validationErrors.Add("Not found");
                }

                ValidatedCertificate certResponse = new ValidatedCertificate
                {
                    Uln = request.Uln,
                    LastName = request.LastName,
                    StdCode = request.StdCode,
                    CertificateData = request.CertificateData,
                    ValidationErrors = validationErrors
                };

                response.Add(certResponse);
            }

            return response;
        }

        private async Task<CertificateResponse> UpdateDraftCertificate(Certificate cert, Domain.JsonData.CertificateData certificateData, string username, string newStatus, string updateAction)
        {
            CertificateResponse certResponse = new CertificateResponse
            {
                Certificate = cert,
                Status = CertificateStatus.Error,
                ValidationErrors = new List<string>()
            };

            if (cert == null)
            {
                certResponse.ValidationErrors.Add("not found");
            }
            else if (cert.Status != CertificateStatus.Draft)
            {
                // TODO: Are there any other status which is valid?
                certResponse.ValidationErrors.Add($"Cannot update certificate in '{cert.Status}' Status");
            }
            else
            {
                if (certificateData != null)
                {
                    cert.CertificateData = JsonConvert.SerializeObject(certificateData);
                }
                cert.Status = newStatus;

                var ucRequest = new AssessorService.Api.Types.Models.Certificates.UpdateCertificateRequest(cert);
                ucRequest.Action = updateAction;
                ucRequest.Username = username;

                // TODO: Will there be any exceptions/error response here?
                Certificate updatedCertificate = await _apiClient.UpdateCertificate(ucRequest);

                certResponse.Certificate = updatedCertificate;
                certResponse.Status = updatedCertificate.Status;
            }

            return certResponse;
        }
    }
}