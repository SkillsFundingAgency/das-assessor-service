using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;
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

        public async Task<OrganisationResponse> CreateNewOrganisation(
            CreateOrganisationDomainModel createOrganisationDomainModel)
        {
            var organisation = Mapper.Map<Organisation>(createOrganisationDomainModel);

            _assessorDbContext.Organisations.Add(organisation);
            await _assessorDbContext.SaveChangesAsync();

            var organisationResponse = Mapper.Map<OrganisationResponse>(organisation);
            return organisationResponse;
        }

        public async Task<OrganisationResponse> UpdateOrganisation(
            UpdateOrganisationDomainModel updateOrganisationDomainModel)
        {
            var organisationEntity = _assessorDbContext.Organisations.First(q =>
                q.EndPointAssessorOrganisationId == updateOrganisationDomainModel.EndPointAssessorOrganisationId);
            if (string.IsNullOrEmpty(updateOrganisationDomainModel.PrimaryContact))
            {
                organisationEntity.PrimaryContact = null;
            }
            else
            {
                var contact =
                    _assessorDbContext.Contacts.First(q => q.Username == updateOrganisationDomainModel.PrimaryContact);
                organisationEntity.PrimaryContact = contact.Username;
            }

            organisationEntity.EndPointAssessorName = updateOrganisationDomainModel.EndPointAssessorName;
            organisationEntity.Status = updateOrganisationDomainModel.Status;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<OrganisationResponse>(organisationEntity);
            return organisationQueryViewModel;
        }

        public async Task Delete(string endPointAssessorOrganisationId)
        {
            var organisationEntity = _assessorDbContext.Organisations
                .FirstOrDefault(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    q.Status != OrganisationStatus.Deleted);

            if (organisationEntity == null)
                throw new NotFound();

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.Status = OrganisationStatus.Deleted;

            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}