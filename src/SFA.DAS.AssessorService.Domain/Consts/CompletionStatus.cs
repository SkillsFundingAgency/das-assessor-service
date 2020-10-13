using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Consts
{
    public enum CompletionStatus
    {
        Continuing = 1,
        Complete = 2,
        Withdrawn = 3,
        TemporarilyWithdrawn = 6
    }
}
