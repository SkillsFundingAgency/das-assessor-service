using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
//    public class CertificateHistoryMappingConverter<T> : ITypeConverter<string, T>
//        where T: CertificateHistoryResponse
//    {
//        public T Convert(string source, T destination, ResolutionContext context)
//        {
//            var certificateData = JsonConvert.DeserializeObject<CertificateData>(source);
//            var certificateHistoryResponse = new CertificateHistoryResponse
//            {
//                AchievementDate =  certificateData.AchievementDate,
//                ContactAddLine1 = certificateData.ContactAddLine1,
//                ContactAddLine2 = certificateData.ContactAddLine2,
//                ContactAddLine3 = certificateData.ContactAddLine3,
//                ContactAddLine4 = certificateData.ContactAddLine4,
//                ContactPostCode = certificateData.ContactPostCode,
//                FullName = certificateData.FullName,
//                CourseOption = certificateData.CourseOption,
//                StandardName = certificateData.StandardName,
//                ContactOrganisation = certificateData.ContactOrganisation,
//                OverallGrade = certificateData.OverallGrade                                
//            };

//            return certificateHistoryResponse as T;
//        }
//    }

    public class CertificateHistoryMappingConverter : ITypeConverter<string, CertificateHistoryResponse>
    {
        public CertificateHistoryResponse Convert(string source, CertificateHistoryResponse destination, ResolutionContext context)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(source);
            var certificateHistoryResponse = new CertificateHistoryResponse
            {
                AchievementDate = certificateData.AchievementDate,
                ContactAddLine1 = certificateData.ContactAddLine1,
                ContactAddLine2 = certificateData.ContactAddLine2,
                ContactAddLine3 = certificateData.ContactAddLine3,
                ContactAddLine4 = certificateData.ContactAddLine4,
                ContactPostCode = certificateData.ContactPostCode,
                FullName = certificateData.FullName,
                CourseOption = certificateData.CourseOption,
                StandardName = certificateData.StandardName,
                ContactOrganisation = certificateData.ContactOrganisation,
                OverallGrade = certificateData.OverallGrade
            };

            return certificateHistoryResponse;
        }
    }
}
