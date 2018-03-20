using System.Collections.Generic;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Data
{
    public class CertificatesRepository
    {
        public static IEnumerable<Certificate> GetData()
        {
            var certificates = Builder<Certificate>.CreateListOfSize(100)
                .All()
                .With(
                    q => q.CertificateData =
                        JsonConvert.SerializeObject(
                            Builder<SFA.DAS.AssessorService.Domain.JsonData.CertificateData>
                                .CreateNew()
                                .With(x => x.ContactName = "Kim Fucious")
                                .With(x => x.ContactPostCode = "B61 4FE")
                                .Build()
                        ))
                .Build();

            return certificates;
        }
    }
}
