using FizzWare.NBuilder;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    public class WhenRetrieveContactBase
    {
        private readonly ContactData _contactData;
        private readonly OrganisationData _organisationData;

        public WhenRetrieveContactBase(ContactData contactData,
            OrganisationData organisationData)
        {
            _contactData = contactData;
            _organisationData = organisationData;
        }

        protected void Setup(string endPointAssessorOrganisationId, string userName, string emailAddress)
        {
            CreateOrganisation(endPointAssessorOrganisationId);
            CreateContact(endPointAssessorOrganisationId, userName, emailAddress);
        }

        private Organisation CreateOrganisation(string endPointAssessorOrganisationId)
        {
            var organisation = Builder<Organisation>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = endPointAssessorOrganisationId)
                .Build();

            _organisationData.Insert(organisation);
            return organisation;
        }

        private void CreateContact(string endPointAssessorOrganisationId, string userName, string emailAddress)
        {
            var contact = Builder<Contact>.CreateNew()
                .With(q => q.Username = userName)
                .With(q => q.Email = emailAddress)                
                .With(q => q.EndPointAssessorOrganisationId = endPointAssessorOrganisationId)
                .Build();

            var result = _contactData.Insert(contact);
        }
    }
}
