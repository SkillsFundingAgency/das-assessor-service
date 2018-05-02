using System.Collections.Generic;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public class CoverLettersProduced
    {
        public List<string> CoverLetterFileNames { get; set; } = new List<string>();
        public Dictionary<string, string> CoverLetterCertificates { get; set; } = new Dictionary<string, string>();
    }
}
