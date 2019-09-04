using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Data;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class ApplyDataHandler : SqlMapper.TypeHandler<ApplyData>
    {
        public override void SetValue(IDbDataParameter parameter, ApplyData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override ApplyData Parse(object value)
        {
            return JsonConvert.DeserializeObject<ApplyData>(value.ToString());
        }

    }
}
