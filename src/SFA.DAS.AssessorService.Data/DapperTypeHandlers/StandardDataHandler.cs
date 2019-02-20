using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class StandardDataHandler : SqlMapper.TypeHandler<StandardData>
    {
        public override StandardData Parse(object value)
        {
            return JsonConvert.DeserializeObject<StandardData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, StandardData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
