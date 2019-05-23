using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Data;

namespace SFA.DAS.AssessorService.EpaoImporter.Data.DapperTypeHandlers
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
