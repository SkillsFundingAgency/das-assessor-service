using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class SearchDataHandler : SqlMapper.TypeHandler<SearchData>
    {
        public override SearchData Parse(object value)
        {
            return JsonConvert.DeserializeObject<SearchData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, SearchData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
