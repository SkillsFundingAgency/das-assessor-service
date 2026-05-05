using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class SearchCertificatesRequest : IRequest<List<SearchCertificatesResponse>>
    {
        public DateTime DateOfBirth { get; set; }
        public string Name { get; set; }
        public IEnumerable<long> Exclude { get; set; }
    }
}
