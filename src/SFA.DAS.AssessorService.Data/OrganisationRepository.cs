namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<OrganisationQueryViewModel> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation)
        {
            var organisation = Mapper.Map<Organisation>(newOrganisation);

            _assessorDbContext.Organisations.Add(organisation);
            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<OrganisationQueryViewModel>(organisation);
            return organisationQueryViewModel;
        }

        public async Task<OrganisationQueryViewModel> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel)
        {
            var organisationEntity = _assessorDbContext.Organisations.First(q => q.Id == organisationUpdateDomainModel.Id);

            organisationEntity.PrimaryContactId = organisationUpdateDomainModel.PrimaryContactId;
            organisationEntity.EndPointAssessorName = organisationUpdateDomainModel.EndPointAssessorName;
            organisationEntity.Status = organisationUpdateDomainModel.Status;

            _assessorDbContext.Entry(organisationEntity).State = EntityState.Modified;
            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<OrganisationQueryViewModel>(organisationEntity);
            return organisationQueryViewModel;
        }

        public async Task DeleteOrganisationByEpaoId(string epaoId)
        {
            var organisation = _assessorDbContext.Organisations.Single(o => o.EndPointAssessorOrganisationId == epaoId);
            organisation.Status = OrganisationStatus.Deleted;
            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Organisation>> GetAllOrganisations()
        {
            return new List<Organisation>()
            {
                new Organisation() { EndPointAssessorOrganisationId = "EPA0001", EndPointAssessorName = "BCS, The Chartered Institute for IT" }
            }.AsEnumerable();
        }

        public async Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn);
            if (organisation == null)
                return null;

            var organisationViewModel = Mapper.Map<OrganisationQueryViewModel>(organisation);
            return organisationViewModel;
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);
            return organisation == null ? false : true;
        }

        public async Task<bool> CheckIfAlreadyExists(Guid id)
        {
            var organisation = await _assessorDbContext.Organisations
                        .FirstOrDefaultAsync(q => q.Id == id);
            return organisation == null ? false : true;
        }
    }
}