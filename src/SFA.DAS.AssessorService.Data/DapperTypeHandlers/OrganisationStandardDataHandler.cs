using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Data;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class OrganisationStandardDataHandler : SqlMapper.TypeHandler<OrganisationStandardData>
    {
        public override OrganisationStandardData Parse(object value)
        {
            return JsonConvert.DeserializeObject<OrganisationStandardData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, OrganisationStandardData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
