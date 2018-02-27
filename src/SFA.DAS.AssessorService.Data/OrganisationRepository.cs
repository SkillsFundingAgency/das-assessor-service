namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<AssessorService.Api.Types.Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation)
        {
            var organisation = Mapper.Map<Domain.Entities.Organisation>(newOrganisation);

            _assessorDbContext.Organisations.Add(organisation);
            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<AssessorService.Api.Types.Organisation>(organisation);
            return organisationQueryViewModel;
        }

        public async Task<AssessorService.Api.Types.Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel)
        {
            var organisationEntity = _assessorDbContext.Organisations.FirstOrDefault(q => q.Id == organisationUpdateDomainModel.Id);

            organisationEntity.PrimaryContactId = organisationUpdateDomainModel.PrimaryContactId;
            organisationEntity.EndPointAssessorName = organisationUpdateDomainModel.EndPointAssessorName;
            organisationEntity.OrganisationStatus = organisationUpdateDomainModel.OrganisationStatus;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<AssessorService.Api.Types.Organisation>(organisationEntity);
            return organisationQueryViewModel;
        }

        public async Task Delete(Guid id)
        {
            var organisationEntity = _assessorDbContext.Organisations
                      .FirstOrDefault(q => q.Id == id && q.OrganisationStatus != OrganisationStatus.Deleted);

            if (organisationEntity == null)
                throw (new NotFound());

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.OrganisationStatus = OrganisationStatus.Deleted;

            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}