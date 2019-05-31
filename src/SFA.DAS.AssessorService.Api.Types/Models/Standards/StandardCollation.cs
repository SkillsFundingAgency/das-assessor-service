using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    //TODO: TECH DEBT: Move into SFA.DAS.AssessorService.Domain.Entities as it represents a database table
    public class StandardCollation
    {
        public int Id { get; set; }
        public int? StandardId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public StandardData StandardData { get; set; }

        [NotMapped]
        public string StandardDataJsonString { get => JsonConvert.SerializeObject(StandardData); }

        [NotMapped]
        public List<string> Options { get; set; }
    }
}
