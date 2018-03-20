using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        public Task Execute()
        {
            foreach (var certificate in CertificateData.GetData())
            {
                Console.WriteLine($"Processig Certificate - {certificate.Id}");
            }

            return Task.CompletedTask;
        }
    }
}
