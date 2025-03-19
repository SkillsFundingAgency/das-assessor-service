using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Extensions;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch
{
    public class GetFrameworkLearnerHandler : BaseHandler, IRequestHandler<GetFrameworkLearnerRequest, GetFrameworkLearnerResponse>
    {
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IStaffCertificateRepository _staffCertificateRepository;

        public GetFrameworkLearnerHandler(IMapper mapper, IFrameworkLearnerRepository frameworkLearnerRepository, ICertificateRepository certificateRepository, IStaffCertificateRepository staffCertificateRepository)
            : base(mapper)
        {
            _frameworkLearnerRepository = frameworkLearnerRepository;
            _certificateRepository = certificateRepository;
            _staffCertificateRepository = staffCertificateRepository;
        }

        public async Task<GetFrameworkLearnerResponse> Handle(GetFrameworkLearnerRequest request, CancellationToken cancellationToken)
        {
            var frameworkLearner =  await _frameworkLearnerRepository.GetFrameworkLearner(request.Id);
            var frameworkLearnerResponse = _mapper.Map<GetFrameworkLearnerResponse>(frameworkLearner);

            var certificate = await _certificateRepository.GetFrameworkCertificate(request.Id, request.AllLogs);
            if (certificate != null)
            { 
                var logs = new List<CertificateLogSummary>();

                if (request.AllLogs)
                {
                    var allLogs = await _staffCertificateRepository.GetAllCertificateLogs(certificate.Id);
                    if (allLogs != null)
                    {
                        logs.AddRange(allLogs);
                    }
                }
                else
                {
                    var latestCertificateLogs = await _staffCertificateRepository.GetLatestCertificateLogs(certificate.Id, 3);
                    if (latestCertificateLogs != null)
                    {
                        logs.AddRange(latestCertificateLogs);
                    }
                }

                if (logs.Count > 1)
                {
                    logs.CalculateDifferences();
                }

                frameworkLearnerResponse.CertificateLogs = logs;
                frameworkLearnerResponse.CertificateReference = certificate.CertificateReference;
                frameworkLearnerResponse.CertificateStatus = certificate.Status;
                frameworkLearnerResponse.CertificateStatusDate = certificate.UpdatedAt;
            }

            return frameworkLearnerResponse;
        }
    }
}