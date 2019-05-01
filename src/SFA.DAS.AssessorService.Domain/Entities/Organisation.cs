using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Organisation : BaseEntity
    {
        private string _extendedOrgData;

        public Guid Id { get; set; }

        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }

        public string PrimaryContact { get; set; }

        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }

        public string Status { get; set; }
        
        [NotMapped]
        public OrganisationData OrganisationDataFromJson
        {
            get => JsonConvert.DeserializeObject<OrganisationData>(string.IsNullOrEmpty(_extendedOrgData) ? "{}" : _extendedOrgData);
            set => _extendedOrgData = value.ToString();
        }
        
        public int OrganisationTypeId { get; set; }

        public virtual OrganisationType OrganisationType { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}