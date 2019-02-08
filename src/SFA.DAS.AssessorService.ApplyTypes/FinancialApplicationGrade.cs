using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class FinancialApplicationGrade
    {
        public string SelectedGrade { get; set; }
        public string InadequateMoreInformation { get; set; }

        public FinancialDueDate OutstandingFinancialDueDate { get; set; }
        public FinancialDueDate GoodFinancialDueDate { get; set; }
        public FinancialDueDate SatisfactoryFinancialDueDate { get; set; }

        public DateTime? FinancialDueDate { get; set; }
        
        public string GradedBy { get; set; }
        public DateTime GradedDateTime { get; set; }
    }

    public class FinancialDueDate
    {
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public DateTime ToDateTime()
        {
            return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
        }
    }

    public class FinancialApplicationSelectedGrade
    {
        public const string Excellent = "Excellent";
        public const string Good = "Good";
        public const string Satisfactory = "Satisfactory";
        public const string Inadequate = "Inadequate";
        public const string Exempt = "Exempt";
    }
}