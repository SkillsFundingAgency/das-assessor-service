using System;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public interface ICertificateViewModel
    {
        Guid Id { get; set; }
        string FamilyName { get; set; }
        string GivenNames { get; set; }
        void FromCertificate(Domain.Entities.Certificate cert);
        CertificateData GetCertificateDataFromViewModel(CertificateData data);
    }
}