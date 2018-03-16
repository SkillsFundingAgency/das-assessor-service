using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificateHandler : IRequestHandler<GetCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;

        public GetCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
        }

        public async Task<Certificate> Handle(GetCertificateRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.GetCertificate(request.Uln, request.StandardCode) ??
                   await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(GetCertificateRequest request)
        {
            var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
            
            var certData = new CertificateData()
            {
                LearnerGivenNames = ilr.GivenNames,
                LearnerFamilyName = ilr.FamilyName,
                LearnerDateofBirth = ilr.DateOfBirth,
                LearnerSex = ilr.Sex,
                StandardName = (await _assessmentOrgsApiClient.GetStandard(ilr.StdCode)).Title,
                LearningStartDate = ilr.LearnStartDate
            };

            var newCertificate = await _certificateRepository.New(
                new Certificate()
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    ProviderUkPrn = ilr.UkPrn,
                    OrganisationId = request.OrganisationId,
                    CreatedBy = request.Username,
                    CertificateData = JsonConvert.SerializeObject(certData),
                    CertificateReference = Guid.NewGuid().ToString(),
                    Status = CertificateStatus.Draft
                });

            return newCertificate;
        }
    }
}