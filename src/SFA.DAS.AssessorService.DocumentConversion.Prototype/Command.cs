using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly LetterHead _letterHead;

        public Command(DocumentTemplateDataStream documentTemplateDataStream,
            LetterHead letterHead)
        {
            _documentTemplateDataStream = documentTemplateDataStream;
            _letterHead = letterHead;
        }

        public async Task Execute()
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            foreach (var certificate in CertificateData.GetData())
            {
                Console.WriteLine($"Processing Certificate - {certificate.Id}");
                var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);
                _letterHead.Create(certificateData, documentTemplateDataStream);
            }

            documentTemplateDataStream.Close();
        }
    }
}
