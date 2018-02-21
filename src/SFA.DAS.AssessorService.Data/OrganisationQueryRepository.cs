namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationQueryRepository : IOrganisationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }
       
        public async Task<IEnumerable<OrganisationQueryViewModel>> GetAllOrganisations()
        {
            var organisations = await _assessorDbContext.Organisations
                .Select(q => Mapper.Map<OrganisationQueryViewModel>(q)).AsNoTracking().ToListAsync();

            return organisations;
        }

        public async Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn && q.OrganisationStatus != OrganisationStatus.Deleted);
            if (organisation == null)
                return null;

            var organisationViewModel = Mapper.Map<OrganisationQueryViewModel>(organisation);
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