using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly IAssessorUnitOfWork _assessorUnitOfWork;

        public OrganisationRepository(IAssessorUnitOfWork assessorUnitOfWork)
        {
            _assessorUnitOfWork = assessorUnitOfWork;
        }

        public async Task<Organisation> CreateNewOrganisation(Organisation organisation)
        {
            _assessorUnitOfWork.AssessorDbContext.Organisations.Add(organisation);
            await _assessorUnitOfWork.SaveChangesAsync();

            return organisation;
        }

        public async Task<Organisation> UpdateOrganisation(
            Organisation organisation)
        {
            var organisationEntity = _assessorUnitOfWork.AssessorDbContext.Organisations.First(q =>
                q.EndPointAssessorOrganisationId == organisation.EndPointAssessorOrganisationId);
            if (string.IsNullOrEmpty(organisation.PrimaryContact))
            {
                organisationEntity.PrimaryContact = null;
            }
            else
            {
                var contact =
                    _assessorUnitOfWork.AssessorDbContext.Contacts.First(q => q.Username == organisation.PrimaryContact);
                organisationEntity.PrimaryContact = contact.Username;
            }

            // Only update if a new status was specified!
            if(!string.IsNullOrEmpty(organisation.Status))
            {
                organisationEntity.Status = organisation.Status;
            }

            // Only update if OrganisationData was specified!
            if (organisation.OrganisationData != null)
            {
                organisationEntity.OrganisationData = organisation.OrganisationData;
            }

            organisationEntity.EndPointAssessorName = organisation.EndPointAssessorName;
            organisationEntity.EndPointAssessorUkprn = organisation.EndPointAssessorUkprn;
            organisationEntity.ApiEnabled = organisation.ApiEnabled ;
            organisationEntity.ApiUser = organisation.ApiUser;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(organisationEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
            
            return organisationEntity;
        }

        public async Task Delete(string endPointAssessorOrganisationId)
        {
            var organisationEntity = _assessorUnitOfWork.AssessorDbContext.Organisations
                .FirstOrDefault(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            if (organisationEntity == null)
                throw new NotFoundException();

            // If already deleted ignore
            if (organisationEntity.Status == OrganisationStatus.Deleted)
                return;

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.Status = OrganisationStatus.Deleted;

            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(organisationEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }
    }
}