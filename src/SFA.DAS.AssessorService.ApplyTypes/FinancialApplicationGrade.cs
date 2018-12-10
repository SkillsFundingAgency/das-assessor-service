using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class FinancialApplicationGrade
    {
        public string SelectedGrade { get; set; }
        public string InadequateMoreInformation { get; set; }
        public string SatisfactoryMoreInformation { get; set; }
        public string GradedBy { get; set; }
        public DateTime GradedDateTime { get; set; }
    }
}