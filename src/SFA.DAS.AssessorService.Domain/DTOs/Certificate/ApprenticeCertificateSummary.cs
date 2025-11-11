using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Domain.DTOs.Certificate
{
    public class ApprenticeCertificateSummary
    {
        public Guid Id { get; set; }
        public string CourseType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; } 
        public string CourseLevel { get; set; }
        public DateTime DateAwarded { get; set; }
    }
}
