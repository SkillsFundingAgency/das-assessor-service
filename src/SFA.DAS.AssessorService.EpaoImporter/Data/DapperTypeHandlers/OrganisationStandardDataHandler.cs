using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;
using System.Data;

namespace SFA.DAS.AssessorService.EpaoImporter.Data.DapperTypeHandlers
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
