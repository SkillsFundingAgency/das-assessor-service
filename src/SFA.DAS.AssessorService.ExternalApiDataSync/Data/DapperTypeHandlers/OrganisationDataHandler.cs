using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Data;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.DapperTypeHandlers
{
    public class OrganisationDataHandler : SqlMapper.TypeHandler<OrganisationData>
    {
        public override OrganisationData Parse(object value)
        {
            return JsonConvert.DeserializeObject<OrganisationData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, OrganisationData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
