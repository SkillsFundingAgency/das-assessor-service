using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
    public class QualificationDetailsResolver : IValueResolver<FrameworkLearner, GetFrameworkCertificateResult, List<QualificationDetails>>
    {
        public List<QualificationDetails> Resolve(FrameworkLearner source, GetFrameworkCertificateResult destination, List<QualificationDetails> destMember, ResolutionContext context)
        {
            var qualifications = new List<QualificationDetails>();

            if (!string.IsNullOrEmpty(source.CombinedQualification) && !string.IsNullOrEmpty(source.CombinedAwardingBody))
            {
                qualifications.Add(new QualificationDetails
                {
                    Name = source.CombinedQualification,
                    AwardingBody = source.CombinedAwardingBody
                });
            }

            if (!string.IsNullOrEmpty(source.CompetenceQualification) && !string.IsNullOrEmpty(source.CompetanceAwardingBody))
            {
                qualifications.Add(new QualificationDetails
                {
                    Name = source.CompetenceQualification,
                    AwardingBody = source.CompetanceAwardingBody
                });
            }

            if (!string.IsNullOrEmpty(source.KnowledgeQualification) && !string.IsNullOrEmpty(source.KnowledgeAwardingBody))
            {
                qualifications.Add(new QualificationDetails
                {
                    Name = source.KnowledgeQualification,
                    AwardingBody = source.KnowledgeAwardingBody
                });
            }

            return qualifications;
        }
    }
}
