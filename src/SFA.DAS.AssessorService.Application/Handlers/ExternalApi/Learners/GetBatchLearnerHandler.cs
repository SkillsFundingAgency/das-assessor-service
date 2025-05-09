﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Learners
{
    public class GetBatchLearnerHandler : IRequestHandler<GetBatchLearnerRequest, GetBatchLearnerResponse>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetBatchLearnerHandler> _logger;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IStandardService _standardService;
        private readonly ICertificateRepository _certificateRepository;

        public GetBatchLearnerHandler(IMediator mediator, ILogger<GetBatchLearnerHandler> logger, ILearnerRepository learnerRepository, IOrganisationQueryRepository organisationRepository, IStandardService standardService, ICertificateRepository certificateRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _learnerRepository = learnerRepository;
            _organisationRepository = organisationRepository;
            _standardService = standardService;
            _certificateRepository = certificateRepository;
        }

        public async Task<GetBatchLearnerResponse> Handle(GetBatchLearnerRequest request, CancellationToken cancellationToken)
        {
            // Get certificate will be called twice. 
            // The Get Certificate Handler contains will not return a certificate in certain scenarios
            // but there is information stored in the certificate that we need for learner details

            var standard = await _standardService.GetStandardVersionById(request.Standard);

            if (standard != null)
            {
                var certificateFromRepository = await _certificateRepository.GetCertificate(request.Uln, standard.LarsCode, request.FamilyName);

                var learner = await GetLearnerDetails(request, standard, certificateFromRepository);
                var epaDetails = GetEpaDetailsFromCertificate(certificateFromRepository);
                var certificateFromHandler = await GetCertificate(request, standard);

                return new GetBatchLearnerResponse { Learner = learner, Certificate = certificateFromHandler, EpaDetails = epaDetails };
            }

            return new GetBatchLearnerResponse();
        }

        private async Task<LearnerDetailForExternalApi> GetLearnerDetails(GetBatchLearnerRequest request, Standard standard, Certificate certFromRepository)
        {
            LearnerDetailForExternalApi learnerDetail = null;

            if (standard != null)
            {
                var learner = await _learnerRepository.Get(request.Uln, standard.LarsCode);

                if (learner != null)
                {
                    var epao = await _organisationRepository.GetByUkPrn(learner.UkPrn);

                    learnerDetail = new LearnerDetailForExternalApi()
                    {
                        Uln = learner.Uln,
                        GivenNames = learner.GivenNames,
                        FamilyName = learner.FamilyName,
                        LearnerStartDate = learner.LearnStartDate,
                        LearnerReferenceNumber = learner.LearnRefNumber,
                        PlannedEndDate = learner.PlannedEndDate,
                        CompletionStatus = learner.CompletionStatus,
                        Standard = standard,
                        EndPointAssessorOrganisationId = epao?.EndPointAssessorOrganisationId ?? learner.EpaOrgId,
                        UkPrn = epao?.EndPointAssessorUkprn ?? learner.UkPrn,
                        OrganisationName = epao?.EndPointAssessorName ?? "Unknown",
                        Version = certFromRepository?.CertificateData?.Version,
                        CourseOption = certFromRepository?.CertificateData?.CourseOption
                    };
                }
                else
                {
                    _logger.LogError($"Could not find learner for ULN {request.Uln} and StandardCode {standard.LarsCode}");
                }
            }

            return learnerDetail;
        }

        private async Task<Certificate> GetCertificate(GetBatchLearnerRequest request, Standard standard)
        {
            Certificate certificate = null;

            if (standard != null && request.IncludeCertificate)
            {
                var certificateRequest = new GetBatchCertificateRequest
                {
                    Uln = request.Uln,
                    FamilyName = request.FamilyName,
                    StandardCode = standard.LarsCode,
                    StandardReference = standard.IfateReferenceNumber,
                    UkPrn = request.UkPrn
                };

                certificate = await _mediator.Send(certificateRequest);
            }

            return certificate;
        }

        private EpaDetails GetEpaDetailsFromCertificate(Certificate certificate)
        {
            EpaDetails epaDetails = null;

            if (certificate != null && certificate.Status != CertificateStatus.Deleted)
            {
                epaDetails = certificate.CertificateData?.EpaDetails;
            }

            return epaDetails;
        }
    }
}
