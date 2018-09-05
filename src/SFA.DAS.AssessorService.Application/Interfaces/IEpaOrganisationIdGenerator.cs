using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationIdGenerator
    {
        string GetNextOrganisationId();
    }
}
