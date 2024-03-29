using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FinancialGrade
    {
        public string ApplicationReference { get; set; }
        public string SelectedGrade { get; set; }
        public string InadequateMoreInformation { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        public string GradedBy { get; set; }
        public DateTime GradedDateTime { get; set; }
        public List<FinancialEvidence> FinancialEvidences { get; set; }
    }

    public class FinancialEvidence
    {
        public string Filename { get; set; }
    }

    public class FinancialApplicationSelectedGrade
    {
        public const string Outstanding = "Outstanding";
        public const string Good = "Good";
        public const string Satisfactory = "Satisfactory";
        public const string Monitoring = "Monitoring";
        public const string Inadequate = "Inadequate";
        public const string Exempt = "Exempt";
    }
}