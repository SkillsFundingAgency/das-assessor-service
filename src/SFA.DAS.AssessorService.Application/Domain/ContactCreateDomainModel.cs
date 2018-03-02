﻿namespace SFA.DAS.AssessorService.Application.Domain
{
    using System;
    using AssessorService.Domain.Enums;

    public class ContactCreateDomainModel
    {      
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    
        public ContactStatus ContactStatus { get; set; }
    }
}
