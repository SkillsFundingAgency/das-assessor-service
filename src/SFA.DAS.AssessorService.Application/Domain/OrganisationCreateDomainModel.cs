﻿using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.ViewModel.Models
{
    using MediatR;

    public class OrganisationCreateDomainModel
    {        
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUKPRN { get; set; }
        public string EndPointAssessorName { get; set; }
        public int? PrimaryContactId { get; set; }
        public OrganisationStatus Status { get; set; }
    }
}
