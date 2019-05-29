using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    //TODO: Move into SFA.DAS.AssessorService.Domain.Entities
    public class StandardCollation
    {
        public int Id { get; set; }
        public int? StandardId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public StandardData StandardData { get; set; }

        [NotMapped]
        public string StandardDataJsonString { get => JsonConvert.SerializeObject(StandardData); }

        public List<string> Options { get; set; }
    }
}
