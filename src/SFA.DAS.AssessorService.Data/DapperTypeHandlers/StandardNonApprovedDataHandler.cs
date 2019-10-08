using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class StandardNonApprovedDataHandler : SqlMapper.TypeHandler<StandardNonApprovedData>
    {
        public override StandardNonApprovedData Parse(object value)
        {
            return JsonConvert.DeserializeObject<StandardNonApprovedData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, StandardNonApprovedData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
