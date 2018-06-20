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

                //_aggregateLogger.LogInfo($"Processing Certificate for Cover Letter - {certificate.CertificateReference} - {wordDocumentFileName}");
                //var wordStream = CreateWordDocumentStream(wordDocumentFileName, certificate, documentTemplateDataStream);
                //_aggregateLogger.LogInfo($"Converted Certificate data - Contact Name = {certificate.CertificateData.ContactName}");

                //await _fileTransferClient.Send(wordStream, wordDocumentFileName);

                //wordStream.Close();
            }         

            documentTemplateDataStream.Close();

            return coverLettersProduced;
        }

        //private MemoryStream CreateWordDocumentStream(string mergedFileName, CertificateResponse certificateResponse, MemoryStream documentTemplateStream)
        //{
        //    _aggregateLogger.LogInfo("Merging fields in document ...");
        //    var document = MergeFieldsInDocument(certificateResponse, documentTemplateStream);
        //    _aggregateLogger.LogInfo("Merged fields in Document");

        //    return ConvertDocumentToStream(document);
        //}

        //private Document MergeFieldsInDocument(CertificateResponse certificateResponse, MemoryStream documentTemplateStream)
        //{
        //    var document = new Document();

        //    _aggregateLogger.LogInfo("Load Document from Stream ...");
        //    document.LoadFromStream(documentTemplateStream, FileFormat.Docx);
        //    _aggregateLogger.LogInfo($"Document Length = {document.Count}");

        //    _aggregateLogger.LogInfo($"Document Template Stream = {documentTemplateStream.Length}");

        //    var certificateData = certificateResponse.CertificateData;

        //    var contactDetails = "";
        //    if (!string.IsNullOrEmpty(certificateData.ContactOrganisation))
        //    {
        //        contactDetails += certificateData.ContactOrganisation + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.Department))
        //    {
        //        contactDetails += certificateData.Department + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.ContactAddLine1))
        //    {
        //        contactDetails += certificateData.ContactAddLine1 + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.ContactAddLine2))
        //    {
        //        contactDetails += certificateData.ContactAddLine2 + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.ContactAddLine3))
        //    {
        //        contactDetails += certificateData.ContactAddLine3 + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.ContactAddLine4))
        //    {
        //        contactDetails += certificateData.ContactAddLine4 + System.Environment.NewLine;
        //    }

        //    if (!string.IsNullOrEmpty(certificateData.ContactPostCode))
        //    {
        //        contactDetails += certificateData.ContactPostCode + System.Environment.NewLine;
        //    }

        //    document.Replace("[Addressee Name]", string.IsNullOrEmpty(certificateData.ContactName) ? "" : certificateData.ContactName, false, true);
        //    document.Replace("[ContactDetails]", contactDetails, false, true);

        //    var utcNow = DateTime.UtcNow;
        //    var gmtNow = utcNow.UtcToTimeZoneTime(TimezoneNames.GmtStandardTimeZone);
        //    document.Replace("[TodaysDate]", gmtNow.ToString("dd MMMM yyyy"), false, true);

        //    document.Replace("[Inset employer name?]", certificateData.ContactName, false, true);
        //    return document;
        //}

        //private MemoryStream ConvertDocumentToStream(Document document)
        //{
        //    var wordDocumentStream = new MemoryStream();
        //    _aggregateLogger.LogInfo("Saving document to stream ...");
        //    document.SaveToStream(wordDocumentStream, FileFormat.Docx);
        //    _aggregateLogger.LogInfo("Saved document to stream ...");
        //    return wordDocumentStream;
        //}
    }
}
