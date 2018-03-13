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
                new IlrRecord{Uln = "1111111111", Surname = "Hawkins", Firstname = "Karla", DateOfBirth = "15/04/2002", StandardId = "90", LearningStartDate = "08/09/2015", Provider = "City and Guilds"},
                new IlrRecord{Uln = "1111111111", Surname = "Hawkins", Firstname = "Karla", DateOfBirth = "15/04/2002", StandardId = "93", LearningStartDate = "12/08/2015", Provider = "City and Guilds"},
                new IlrRecord{Uln = "2222222222", Surname = "Taylor", Firstname = "Peter", DateOfBirth = "15/04/2002", StandardId = "33", LearningStartDate = "08/09/2015", Provider = "City and Guilds"},
                new IlrRecord{Uln = "9123671552", Surname = "James", Firstname = "Mildred", DateOfBirth = "15/04/2002", StandardId = "206", LearningStartDate = "10/09/2015", Provider = "ARCHWAY ACADEMY"},
                new IlrRecord{Uln = "3400293847", Surname = "Boyd", Firstname = "Craig", DateOfBirth = "15/04/2002", StandardId = "207", LearningStartDate = "01/10/2015", Provider = "ACCRINGTON AND ROSSENDALE COLLEGE"},
                new IlrRecord{Uln = "6488273664", Surname = "Robinson", Firstname = "Marie", DateOfBirth = "15/04/2002", StandardId = "208", LearningStartDate = "08/08/2015", Provider = "ADECCO UK LIMITED"},
            };
        }


        public async Task<SearchResponse> Search(IlrSearchRequest request)
        {
            var response = new SearchResponse(_records.Where(r =>
                r.Surname.ToLower() == request.Surname.ToLower()
                && r.Uln == request.Uln
                && request.StandardIds.Contains(r.StandardId)));

            return response;
        }
    }
}