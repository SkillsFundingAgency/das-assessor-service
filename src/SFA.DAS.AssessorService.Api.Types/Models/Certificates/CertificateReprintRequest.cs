using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateReprintRequest : IRequest
    {
        public string CertificateReference { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public DateTime? AchievementDate { get; set; }
    }
}