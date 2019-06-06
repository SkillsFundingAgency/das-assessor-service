using System;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class Option
    {
        public Guid Id { get; set; }

        public int StdCode { get; set; }

        public string OptionName { get; set; }
    }
}
