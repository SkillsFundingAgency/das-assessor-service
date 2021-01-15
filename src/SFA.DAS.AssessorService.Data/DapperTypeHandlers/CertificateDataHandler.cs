using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{

    public class CertificateDataHandler : SqlMapper.TypeHandler<CertificateData>
    {
        public override CertificateData Parse(object value)
        {
            return JsonConvert.DeserializeObject<CertificateData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, CertificateData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
