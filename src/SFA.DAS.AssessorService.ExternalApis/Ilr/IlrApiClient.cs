using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;

namespace SFA.DAS.AssessorService.ExternalApis.Ilr
{
    public class IlrApiClient : IIlrApiClient
    {
        private List<IlrRecord> _records;

        public IlrApiClient()
        {
            _records = new List<IlrRecord>
            {
                new IlrRecord{Uln = "5407291429", Surname = "Hawkins", Firstname = "Karla", DateOfBirth = "15/04/2002", Standard = "Apprentice Horticulture Assistant", LearningStartDate = "08/09/2015", Provider = "Pearsons"},
                new IlrRecord{Uln = "2202213687", Surname = "Terry", Firstname = "Doris", DateOfBirth = "15/04/2002", Standard = "Digital Marketing Apprentice", LearningStartDate = "12/08/2015", Provider = "City and Guilds"},
                new IlrRecord{Uln = "5633467851", Surname = "Taylor", Firstname = "Peter Taylor", DateOfBirth = "15/04/2002", Standard = "Refrigeration air conditioning and heat pump engineering technician", LearningStartDate = "08/09/2015", Provider = "ACORN LEARNING SOLUTIONS LIMITED"},
                new IlrRecord{Uln = "9123671552", Surname = "James", Firstname = "Mildred", DateOfBirth = "15/04/2002", Standard = "Systems engineering masters level", LearningStartDate = "10/09/2015", Provider = "ARCHWAY ACADEMY"},
                new IlrRecord{Uln = "3400293847", Surname = "Boyd", Firstname = "Craig", DateOfBirth = "15/04/2002", Standard = "Gas network team leader", LearningStartDate = "01/10/2015", Provider = "ACCRINGTON AND ROSSENDALE COLLEGE"},
                new IlrRecord{Uln = "6488273664", Surname = "Robinson", Firstname = "Marie", DateOfBirth = "15/04/2002", Standard = "Insurance practitioner", LearningStartDate = "08/08/2015", Provider = "ADECCO UK LIMITED"},
            };
        }


        public async Task<SearchResponse> Search(SearchRequest request)
        {
            SearchResponse response;
            if (request.SearchType == SearchType.Uln)
            {
                response = new SearchResponse(_records.Where(r => r.Uln == request.Uln));
            }
            else
            {
                response = new SearchResponse(_records.Where(r =>
                    r.Surname == request.Surname && r.DateOfBirth == request.DateOfBirth));
            }
            return response;
        }
    }
}