using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Data
{
    public class CertificatesRepository
    {
        public IEnumerable<Certificate> GetData()
        {
            //var mergedList = list1.Union(list2).ToList();
            // Merge in data from example ....
            var certificates = new List<Certificate>();
            for (var sequenceNumber = 1; sequenceNumber <= 8; sequenceNumber++)
            {
                var number = sequenceNumber;
                certificates.Add(
                    Builder<Certificate>.CreateNew().With(q => q.CertificateReference = number.ToString().PadLeft(8, '0')).Build()
                );
            }

            var dummyCertificates = Builder<Certificate>.CreateListOfSize(2)
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

            return certificates.Union(dummyCertificates);
        }
    }
}
