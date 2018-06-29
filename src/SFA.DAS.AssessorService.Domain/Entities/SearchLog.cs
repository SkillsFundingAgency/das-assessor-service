using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class SearchLog
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public long Uln { get; set; }
        public DateTime SearchTime { get; set; }
        public int NumberOfResults { get; set; }
        public string Username { get; set; }
    }
}