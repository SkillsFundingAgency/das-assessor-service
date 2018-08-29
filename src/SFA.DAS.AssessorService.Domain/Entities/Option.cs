using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Option
    {
        public Guid Id { get; set; }

        public int LarsCode { get; set; }

        public string IfaStdCode { get; set; }

        public string StandardName { get; set; }

        public string OptionName { get; set; }
    }
}