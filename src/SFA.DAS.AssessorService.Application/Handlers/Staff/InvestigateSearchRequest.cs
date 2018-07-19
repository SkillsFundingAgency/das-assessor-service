using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class InvestigateSearchRequest: IRequest<InvestigationResult>
    {
        public string Surname { get; }
        public long Uln { get; }
        public string Epaorgid { get; }
        public string Username { get; }

        public InvestigateSearchRequest(string surname, long uln, string epaorgid, string username)
        {
            Surname = surname;
            Uln = uln;
            Epaorgid = epaorgid;
            Username = username;
        }
    }

    public class InvestigationResult
    {
        public string Explanation { get; set; }
        public List<SearchResult> Ilr { get; set; }
    }
}