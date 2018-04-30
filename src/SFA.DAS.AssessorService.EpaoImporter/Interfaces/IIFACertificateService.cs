﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.EpaoImporter.Interfaces
{
    public interface IIFACertificateService
    {
        Task Create(int batchNumber, IEnumerable<CertificateResponse> certificates);
    }
}
