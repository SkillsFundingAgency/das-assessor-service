using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Application.Interfaces;
    using AutoMapper;
    using Domain.Consts;
    using Microsoft.EntityFrameworkCore;

    public class OrganisationQueryRepository : IOrganisationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }
       
        public async Task<IEnumerable<Organisation>> GetAllOrganisations()
        {
            var organisations = await _assessorDbContext.Organisations
                .Select(q => Mapper.Map<Organisation>(q)).AsNoTracking().ToListAsync();

            return organisations;
        }

        public async Task<Organisation> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUkprn == ukprn && q.Status != OrganisationStatus.Deleted);
            if (organisation == null)
                return null;

            var organisationViewModel = Mapper.Map<Organisation>(organisation);
            return organisationViewModel;
        }

        public async Task<OrganisationQueryDomainModel> Get(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                      .FirstAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.Status != OrganisationStatus.Deleted);

            var organisationUpdateDomainModel = Mapper.Map<OrganisationQueryDomainModel>(organisation);
            return organisationUpdateDomainModel;
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.Status != OrganisationStatus.Deleted);
            return organisation != null;
        }

        public async Task<bool> CheckIfAlreadyExists(Guid id)
        {
            var organisation = await _assessorDbContext.Organisations
                        .FirstOrDefaultAsync(q => q.Id == id && q.Status != OrganisationStatus.Deleted);
            return organisation != null;
        }

        public async Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                        .Include(q => q.Contacts)
                       .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.Status != OrganisationStatus.Deleted);
            return organisation.Contacts.Count() != 0;
        }
    }
}