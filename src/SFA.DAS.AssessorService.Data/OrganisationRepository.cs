using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<Organisation> CreateNewOrganisation(Organisation organisation)
        {
            _assessorDbContext.Organisations.Add(organisation);
            await _assessorDbContext.SaveChangesAsync();

            return organisation;
        }

        public async Task<Organisation> UpdateOrganisation(
            Organisation organisation)
        {
            var organisationEntity = _assessorDbContext.Organisations.First(q =>
                q.EndPointAssessorOrganisationId == organisation.EndPointAssessorOrganisationId);
            if (string.IsNullOrEmpty(organisation.PrimaryContact))
            {
                organisationEntity.PrimaryContact = null;
            }
            else
            {
                var contact =
                    _assessorDbContext.Contacts.First(q => q.Username == organisation.PrimaryContact);
                organisationEntity.PrimaryContact = contact.Username;
            }

            organisationEntity.EndPointAssessorName = organisation.EndPointAssessorName;
            organisationEntity.Status = organisation.Status;
            organisationEntity.EndPointAssessorUkprn = organisation.EndPointAssessorUkprn;
            organisationEntity.ApiEnabled = organisation.ApiEnabled ;
            organisationEntity.ApiUser = organisation.ApiUser;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
            
            return organisationEntity;
        }

        public async Task Delete(string endPointAssessorOrganisationId)
        {
            var organisationEntity = _assessorDbContext.Organisations
                .FirstOrDefault(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            if (organisationEntity == null)
                throw new NotFound();

            // If already deleted ignore
            if (organisationEntity.Status == OrganisationStatus.Deleted)
                return;

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.Status = OrganisationStatus.Deleted;

            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}