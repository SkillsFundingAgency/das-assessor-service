using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetAddressesHandler : IRequestHandler<GetAddressesRequest, List<CertificateAddressResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;

        public GetAddressesHandler(
            IContactQueryRepository contactQueryRepository,
            ICertificateRepository certificateRepository, IIlrRepository ilrRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger)
        {
            _contactQueryRepository = contactQueryRepository;
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }

        //public async Task<Certificate> Handle(StartCertificateRequest request, CancellationToken cancellationToken)
        //{
        //    return await _certificateRepository.GetCertificate(request.Uln, request.StandardCode) ??
        //           await CreateNewCertificate(request);
        //}

        //private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request)
        //{
        //    _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
        //    var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
        //    _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
        //    var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);
        //    _logger.LogInformation("CreateNewCertificate Before Get Standard from API");
        //    var standard = await _assessmentOrgsApiClient.GetStandard(ilr.StdCode);
        //    var certData = new CertificateData()
        //    {
        //        LearnerGivenNames = ilr.GivenNames,
        //        LearnerFamilyName = ilr.FamilyName,
        //        StandardName = standard.Title,
        //        LearningStartDate = ilr.LearnStartDate, 
        //        StandardLevel = standard.Level,
        //        StandardPublicationDate = standard.EffectiveFrom,
        //        FullName = $"{ilr.GivenNames} {ilr.FamilyName}"
        //    };

        //    _logger.LogInformation("CreateNewCertificate Before create new Certificate");
        //    var newCertificate = await _certificateRepository.New(
        //        new Certificate()
        //        {
        //            Uln = request.Uln,
        //            StandardCode = request.StandardCode,
        //            ProviderUkPrn = ilr.UkPrn,
        //            OrganisationId = organisation.Id,
        //            CreatedBy = request.Username,
        //            CertificateData = JsonConvert.SerializeObject(certData),
        //            Status = Domain.Consts.CertificateStatus.Draft,
        //            CertificateReference = "",
        //            LearnRefNumber = ilr.LearnRefNumber
        //        });

        //    newCertificate.CertificateReference = newCertificate.CertificateReferenceId.ToString().PadLeft(8,'0');

        //    _logger.LogInformation("CreateNewCertificate Before Update Cert in db");
        //    await _certificateRepository.Update(newCertificate, request.Username, null);

        //    _logger.LogInformation(LoggingConstants.CertificateStarted);
        //    _logger.LogInformation($"Certificate with ID: {newCertificate.Id} Started with reference of {newCertificate.CertificateReference}");

        //    return newCertificate;
        //}
        public async Task<List<CertificateAddressResponse>> Handle(GetAddressesRequest request, CancellationToken cancellationToken)
        {
            var contact = await _contactQueryRepository.GetContact(request.Username);
            var organisation = await _organisationQueryRepository.Get(contact.EndPointAssessorOrganisationId);

            var certificateAddresses = await _certificateRepository.GetCertificateAddresses(organisation.Id);
            var addresses = Mapper.Map<List<CertificateAddressResponse>>(certificateAddresses);

            return addresses;
        }
    }
}


