using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{

    public class BatchDataHandler : SqlMapper.TypeHandler<BatchDetails>
    {
        public override BatchDetails Parse(object value)
        {
            return JsonConvert.DeserializeObject<BatchDetails>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, BatchDetails value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
