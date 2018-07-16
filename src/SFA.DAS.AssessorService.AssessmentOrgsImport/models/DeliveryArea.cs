using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public struct DeliveryArea
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Status {get; set;}
    }
}