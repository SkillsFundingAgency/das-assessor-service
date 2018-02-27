namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Application.Domain;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Api.Types;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;

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
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn && q.OrganisationStatus != OrganisationStatus.Deleted);
            if (organisation == null)
                return null;

            var organisationViewModel = Mapper.Map<Organisation>(organisation);
            return organisationViewModel;
        }

        public async Task<OrganisationUpdateDomainModel> Get(Guid organisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                      .FirstAsync(q => q.Id == organisationId && q.OrganisationStatus != OrganisationStatus.Deleted);

            var organisationUpdateDomainModel = Mapper.Map<OrganisationUpdateDomainModel>(organisation);
            return organisationUpdateDomainModel;
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.OrganisationStatus != OrganisationStatus.Deleted);
            return organisation == null ? false : true;
        }

        public async Task<bool> CheckIfAlreadyExists(Guid id)
        {
            var organisation = await _assessorDbContext.Organisations
                        .FirstOrDefaultAsync(q => q.Id == id && q.OrganisationStatus != OrganisationStatus.Deleted);
            return organisation == null ? false : true;
        }

        public async Task<bool> CheckIfOrganisationHasContacts(Guid organisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                        .Include(q => q.Contacts)
                       .FirstOrDefaultAsync(q => q.Id == organisationId && q.OrganisationStatus != OrganisationStatus.Deleted);
            return organisation.Contacts.Count() == 0 ? false : true;
        }
    }
}