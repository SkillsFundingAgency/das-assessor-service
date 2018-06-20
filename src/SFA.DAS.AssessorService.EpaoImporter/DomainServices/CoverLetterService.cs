using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class CoverLetterService : ICoverLetterService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IFileTransferClient _fileTransferClient;
        private readonly IDocumentTemplateDataStream _documentTemplateDataStream;        

        public CoverLetterService(
            IAggregateLogger aggregateLogger,
            IFileTransferClient fileTransferClient,
            IDocumentTemplateDataStream documentTemplateDataStream)            
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _documentTemplateDataStream = documentTemplateDataStream;            
        }

        public async Task<CoverLettersProduced> Create(int batchNumber, IEnumerable<CertificateResponse> certificates)
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            var certificateResponses = certificates as CertificateResponse[] ?? certificates.ToArray();
            var groupedCertificates = certificateResponses.ToArray().GroupBy(
                x => new
                {
                    x.CertificateData.ContactOrganisation,
                    x.CertificateData.Department,
                    x.CertificateData.ContactName,
                    x.CertificateData.ContactPostCode
                },
                (key, group) => new
                {
                    key1 = key.ContactOrganisation,
                    key2 = key.Department,
                    key3 = key.ContactName,
                    key4 = key.ContactPostCode,
                    Result = group.ToList()
                }).OrderBy(q => q.key1);

            var coverLettersProduced = new CoverLettersProduced();
            var sequenceNumber = 0;
            var contactOrganisationsAlreadyProceseed = new List<string>();

            var utcNow = DateTime.UtcNow;
            var gmtNow = utcNow.UtcToTimeZoneTime(TimezoneNames.GmtStandardTimeZone);

            foreach (var groupedCertificate in groupedCertificates)
            {
                if (contactOrganisationsAlreadyProceseed.FirstOrDefault(q => q == groupedCertificate.key1.ToLower()) == null)
                {
                    sequenceNumber = 0;
                    contactOrganisationsAlreadyProceseed.Add(groupedCertificate.key1.ToLower());
                }
                sequenceNumber++;

                var certificate = groupedCertificate.Result[0];
                var wordDocumentFileName =
                    $"IFA-Certificate-{gmtNow:MMyy}-{batchNumber.ToString().PadLeft(3, '0')}-{certificate.CertificateData.ContactOrganisation.ToLower()}-{sequenceNumber}.docx".Replace(' ', '-');
                coverLettersProduced.CoverLetterFileNames.Add(wordDocumentFileName);

                foreach (var groupCertificateResult in groupedCertificate.Result)
                {
                    coverLettersProduced.CoverLetterCertificates.Add(groupCertificateResult.CertificateReference, wordDocumentFileName);
                }              
            }         

            documentTemplateDataStream.Close();

            return coverLettersProduced;
        }       
    }
}
