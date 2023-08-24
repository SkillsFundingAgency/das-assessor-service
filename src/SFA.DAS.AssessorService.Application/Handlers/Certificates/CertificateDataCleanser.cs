using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    internal class CertificateDataCleanser
    {
        internal static CertificateData HandleSendToUpdate(Certificate currentCertificate, CertificateData currentData, CertificateData updatedData)
        {
            if (ShouldClearAddressFields(currentCertificate, currentData, updatedData))
            {
                updatedData = ClearAddressFields(updatedData);
            }

            if (updatedData.SendTo == CertificateSendTo.Apprentice)
            {
                // when sending to the apprentice use the apprentice name for the contact
                updatedData.ContactName = updatedData.FullName;
            }

            if (updatedData.ContactName != null) { updatedData.ContactName = updatedData.ContactName.ToUpper(); }

            return updatedData;
        }

        private static bool ShouldClearAddressFields(Certificate currentCertificate, CertificateData currentCertificateData, CertificateData updatedCertificateData)
        {
            var shouldClearAddressFields = (updatedCertificateData.SendTo != currentCertificateData.SendTo);

            // a certificate which has been created by the API will not have a SendTo but does have valid employer details
            // so retain these employer details when the new SendTo is for an employer
            if (currentCertificate.CreatedBy == "API" && currentCertificateData.SendTo == CertificateSendTo.None && updatedCertificateData.SendTo == CertificateSendTo.Employer)
            {
                shouldClearAddressFields = false;
            }

            return shouldClearAddressFields;
        }

        private static CertificateData ClearAddressFields(CertificateData certificateData)
        {
            certificateData.ContactName = null;
            certificateData.Department = null;
            certificateData.ContactOrganisation = null;
            certificateData.ContactAddLine1 = null;
            certificateData.ContactAddLine2 = null;
            certificateData.ContactAddLine3 = null;
            certificateData.ContactAddLine4 = null;
            certificateData.ContactPostCode = null;

            return certificateData;
        }
    }
}
