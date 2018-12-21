using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{

    public class BatchDataHandler : SqlMapper.TypeHandler<BatchData>
    {
        public override BatchData Parse(object value)
        {
            return JsonConvert.DeserializeObject<BatchData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, BatchData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
