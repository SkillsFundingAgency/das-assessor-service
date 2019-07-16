using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class SearchLog
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public long Uln { get; set; }
        public DateTime SearchTime { get; set; }
        public SearchData SearchData { get; set; }
        [NotMapped]
        public string CertificateDataJsonString { get => JsonConvert.SerializeObject(SearchData); }
        public int NumberOfResults { get; set; }
        public string Username { get; set; }
    }
}