﻿using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    internal static class CertificateDataVersionChangeUpdater
    {
        internal static async Task<CertificateData> UpdateCoronationEmblemAndStandardIfNeeded(CertificateData currentData, CertificateData updatedData, IStandardRepository standardRepository)
        {
            Guard.NotNullOrWhiteSpace(updatedData.StandardReference, nameof(updatedData.StandardReference));

            if (updatedData.Version != currentData.Version) 
            {
                updatedData.CoronationEmblem = await standardRepository.GetCoronationEmblemForStandardReferenceAndVersion(updatedData.StandardReference, updatedData.Version);
                updatedData.StandardName = await standardRepository.GetTitleForStandardReferenceAndVersion(updatedData.StandardReference, updatedData.Version);
            }

            return updatedData;
        }
    }
}
