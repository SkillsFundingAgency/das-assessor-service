using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Option
    {
        public Guid Id { get; set; }

        public int StdCode { get; set; }
        
        public string OptionName { get; set; }
    }
}