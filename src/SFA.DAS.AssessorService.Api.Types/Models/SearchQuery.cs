﻿namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchQuery
    {
        public long Uln { get; set; }
        public string Surname { get; set; }
        public int UkPrn { get; set; }
        public string Username { get; set; }
    }
}