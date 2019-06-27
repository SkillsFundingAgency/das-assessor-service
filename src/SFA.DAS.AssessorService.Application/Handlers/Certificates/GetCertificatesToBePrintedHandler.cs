using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesToBePrintedHandler : IRequestHandler<GetToBePrintedCertificatesRequest, List<CertificateResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;

        public GetCertificatesToBePrintedHandler(ICertificateRepository certificateRepository, ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<List<CertificateResponse>> Handle(GetToBePrintedCertificatesRequest request, CancellationToken cancellationToken)
        {
            var statuses = new List<string>
            {
                Domain.Consts.CertificateStatus.Submitted,
                Domain.Consts.CertificateStatus.Reprint,
                Domain.Consts.CertificateStatus.Queued,
            };

            var certificates = await _certificateRepository.GetCertificates(statuses);

            var certificatesWithoutFails = new List<Certificate>();

            foreach(var cert in certificates)
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
                if (!string.IsNullOrEmpty(certData?.OverallGrade) && certData.OverallGrade != CertificateGrade.Fail)
                {
                    certificatesWithoutFails.Add(cert);
                }
            }

            var certificateResponses = Mapper.Map<List<Certificate>, List<CertificateResponse>>(certificatesWithoutFails);
            return certificateResponses;
        }
    }
}