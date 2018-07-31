using System;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public interface ICertificateViewModel
    {
        Guid Id { get; set; }
        string FamilyName { get; set; }
        string GivenNames { get; set; }
        void FromCertificate(Domain.Entities.Certificate cert);
        bool BackToCheckPage { get; set; }
        Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData);
    }
}