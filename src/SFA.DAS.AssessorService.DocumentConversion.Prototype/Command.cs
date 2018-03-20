using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;

        public Command(DocumentTemplateDataStream documentTemplateDataStream)
        {
            _documentTemplateDataStream = documentTemplateDataStream;
        }

        public async Task Execute()
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            foreach (var certificate in CertificateData.GetData())
            {
                Console.WriteLine($"Processig Certificate - {certificate.Id}");
            }
        }
    }
}
