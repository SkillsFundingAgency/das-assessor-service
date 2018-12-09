using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class CompletionStatus
    {
        public const int Continuing = 1;
        public const int Completed = 2;
        public const int Withdrawn = 3;
        public const int TempWithdrawn = 6;
    }
}
