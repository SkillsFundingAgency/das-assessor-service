using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationQueryRepository : IOrganisationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<OrganisationResponse>> GetAllOrganisations()
        {
            var organisations = await _assessorDbContext.Organisations
                .Select(q => Mapper.Map<OrganisationResponse>(q)).AsNoTracking().ToListAsync();

            return organisations;
        }

        public async Task<OrganisationResponse> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstOrDefaultAsync(q => q.EndPointAssessorUkprn == ukprn && q.Status != OrganisationStatus.Deleted);
            if (organisation == null)
                return null;

            var organisationResponse = Mapper.Map<OrganisationResponse>(organisation);
            return organisationResponse;
        }

        public async Task<OrganisationDomainModel> Get(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    q.Status != OrganisationStatus.Deleted);

            var organisationUpdateDomainModel = Mapper.Map<OrganisationDomainModel>(organisation);
            return organisationUpdateDomainModel;
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstOrDefaultAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    q.Status != OrganisationStatus.Deleted);
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
                .FirstOrDefaultAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    q.Status != OrganisationStatus.Deleted);
            return organisation.Contacts.Count() != 0;
        }
    }
}