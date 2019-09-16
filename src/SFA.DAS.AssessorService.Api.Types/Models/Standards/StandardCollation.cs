using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardCollation
    {
        public int Id { get; set; }
        public int? StandardId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public StandardData StandardData { get; set; }
        public List<string> Options { get; set; }
    }
}
