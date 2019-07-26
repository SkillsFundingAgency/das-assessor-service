using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
    public class BatchNumberResolver : IValueResolver<Certificate, CertificateResponse, string>
    {
        public string Resolve(Certificate source, CertificateResponse destination, string destMember, ResolutionContext context)
        {
            if (source.BatchNumber.HasValue)
            {
                var month = source.ToBePrinted.Value.Month.ToString().PadLeft(2, '0');
                var year = source.ToBePrinted.Value.Year;
                var monthYear = month + year;

                return monthYear + '-' + source.BatchNumber;
            }

            return string.Empty;
        }
    }
}
