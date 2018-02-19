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
    using SFA.DAS.AssessorService.Domain.Exceptions;
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
            var organisationEntity = _assessorDbContext.Organisations.FirstOrDefault(q => q.Id == organisationUpdateDomainModel.Id);

            organisationEntity.PrimaryContactId = organisationUpdateDomainModel.PrimaryContactId;
            organisationEntity.EndPointAssessorName = organisationUpdateDomainModel.EndPointAssessorName;
            organisationEntity.Status = organisationUpdateDomainModel.Status;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<OrganisationQueryViewModel>(organisationEntity);
            return organisationQueryViewModel;
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
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn && q.IsDeleted == false);
            if (organisation == null)
                return null;

            var organisationViewModel = Mapper.Map<OrganisationQueryViewModel>(organisation);
            return organisationViewModel;
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.IsDeleted == false);
            return organisation == null ? false : true;
        }

        public async Task<bool> CheckIfAlreadyExists(Guid id)
        {
            var organisation = await _assessorDbContext.Organisations
                        .FirstOrDefaultAsync(q => q.Id == id && q.IsDeleted == true);
            return organisation == null ? false : true;
        }

        public async Task Delete(int ukprn)
        {
            var organisationEntity = _assessorDbContext.Organisations
                      .FirstOrDefault(q => q.EndPointAssessorUKPRN == ukprn);

            if (organisationEntity == null)
                throw (new NotFound());

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.IsDeleted = true;

            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}