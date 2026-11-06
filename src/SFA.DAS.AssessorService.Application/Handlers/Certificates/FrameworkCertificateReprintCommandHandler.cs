using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Extensions;
using SFA.DAS.AssessorService.Application.Validators;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class FrameworkCertificateReprintCommandHandler : IRequestHandler<FrameworkCertificateReprintCommand, FrameworkCertificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IValidator<PrintDeliveryAddress> _validator;

        public FrameworkCertificateReprintCommandHandler(ICertificateRepository certificateRepository, IFrameworkLearnerRepository frameworkLearnerRepository, 
            IDateTimeHelper dateTimeHelper, IValidator<PrintDeliveryAddress> validator)
        {
            _certificateRepository = certificateRepository;
            _frameworkLearnerRepository = frameworkLearnerRepository;
            _dateTimeHelper = dateTimeHelper;
            _validator = validator;
        }

        public async Task<FrameworkCertificate> Handle(FrameworkCertificateReprintCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetFrameworkCertificate(request.FrameworkLearnerId);

            var address = new PrintDeliveryAddress(
                CertificateSendTo.Apprentice,
                request.ContactName,
                OrganisationName: null,
                AddressLine1: request.ContactAddLine1,
                Postcode: request.ContactPostcode);

            var validation = _validator.Validate(address);

            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(error => error.ErrorMessage));
                throw new BadRequestException(message);
            }

            if (certificate == null)
            {
                var frameworkLearner = await _frameworkLearnerRepository.GetFrameworkLearner(request.FrameworkLearnerId);
                if (frameworkLearner != null)
                {
                    certificate = await CreateNewFrameworkCertificate(request, frameworkLearner);
                }
            }
            else
            {
                certificate = await ReprintFrameworkCertificate(request, certificate);
            }
            
            return certificate;
        }

        private async Task<FrameworkCertificate> CreateNewFrameworkCertificate(FrameworkCertificateReprintCommand request, FrameworkLearner frameworkLearner)
        {
            var certificate = new FrameworkCertificate
            {
                CertificateData = new CertificateData
                {
                    TrainingCode = frameworkLearner.TrainingCode,
                    LearnerGivenNames = frameworkLearner.ApprenticeForename,
                    LearnerFamilyName = frameworkLearner.ApprenticeSurname,
                    FullName = frameworkLearner.ApprenticeFullname,
                    LearningStartDate = frameworkLearner.ApprenticeStartdate,
                    AchievementDate = frameworkLearner.CertificationDate,
                    OverallGrade = CertificateGrade.Pass,
                    SendTo = CertificateSendTo.Apprentice,
                    PathwayName = frameworkLearner.PathwayName,
                    FrameworkName = frameworkLearner.FrameworkName,
                    FrameworkLevelName = frameworkLearner.ApprenticeshipLevelName,
                    FrameworkCertificateNumber = frameworkLearner.FrameworkCertificateNumber,
                    ReprintReasons = request.Reasons.ToFlagsList(),
                    IncidentNumber = request.IncidentNumber,
                    ContactName = request.ContactName,
                    ContactAddLine1 = request.ContactAddLine1,
                    ContactAddLine2 = request.ContactAddLine2,
                    ContactAddLine3 = request.ContactAddLine3,
                    ContactAddLine4 = request.ContactAddLine4,
                    ContactPostCode = request.ContactPostcode,
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
                },
                Status = CertificateStatus.Reprint,
                CreatedBy = request.Username,
                CertificateReference = string.Empty,
                FrameworkLearnerId = request.FrameworkLearnerId,
                DateOfBirth = frameworkLearner.ApprenticeDoB,
                ProviderUkPrn = int.TryParse(frameworkLearner.Ukprn, out int result ) ? result : null,
                PrintRequestedAt = _dateTimeHelper.DateTimeUtcNow,
                PrintRequestedBy = request.Username,
            };

            certificate = await _certificateRepository.NewFrameworkCertificate(certificate);

            return certificate;
        }

        private async Task<FrameworkCertificate> ReprintFrameworkCertificate(FrameworkCertificateReprintCommand request, FrameworkCertificate certificate)
        {
            certificate.Status = CertificateStatus.Reprint;
            certificate.CertificateData.ReprintReasons = request.Reasons.ToFlagsList();
            certificate.CertificateData.IncidentNumber = request.IncidentNumber;
            certificate.CertificateData.ContactName = request.ContactName;
            certificate.CertificateData.ContactAddLine1 = request.ContactAddLine1;
            certificate.CertificateData.ContactAddLine2 = request.ContactAddLine2;
            certificate.CertificateData.ContactAddLine3 = request.ContactAddLine3;
            certificate.CertificateData.ContactAddLine4 = request.ContactAddLine4;
            certificate.CertificateData.ContactPostCode = request.ContactPostcode;

            certificate = await _certificateRepository.UpdateReprintRequested<FrameworkCertificate>(certificate.Id, _dateTimeHelper.DateTimeUtcNow, request.Username);

            return certificate;
        }
    }
}