﻿namespace SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types
{
    public class StandardCollation
    {
        public int? StandardId { get; set; }
        public string ReferenceNumber { get; set; }
        public  string Title { get; set; }
        public StandardData StandardData { get; set; }
    }
}
