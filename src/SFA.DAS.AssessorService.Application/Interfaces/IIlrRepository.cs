using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        IEnumerable<Ilr> Search(SearchRequest searchRequest);
    }
}