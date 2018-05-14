using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface IDocumentTemplateDataStream
    {
        Task<MemoryStream> Get();
    }
}