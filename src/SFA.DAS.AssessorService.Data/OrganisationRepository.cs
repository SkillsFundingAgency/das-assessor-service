﻿namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Domain;
    using AutoMapper;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using Organisation = Api.Types.Models.Organisation;

    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<Organisation> CreateNewOrganisation(OrganisationCreateDomainModel newOrganisation)
        {
            var organisation = Mapper.Map<Domain.Entities.Organisation>(newOrganisation);

            _assessorDbContext.Organisations.Add(organisation);
            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<Organisation>(organisation);
            return organisationQueryViewModel;
        }

        public async Task<Organisation> UpdateOrganisation(OrganisationUpdateDomainModel organisationUpdateDomainModel)
        {
            var organisationEntity = _assessorDbContext.Organisations.First(q => q.EndPointAssessorOrganisationId == organisationUpdateDomainModel.EndPointAssessorOrganisationId);
            if (string.IsNullOrEmpty(organisationUpdateDomainModel.PrimaryContact))
                organisationEntity.PrimaryContactId = null;
            else
            {
                var contact =
                    _assessorDbContext.Contacts.First(q => q.Username == organisationUpdateDomainModel.PrimaryContact);
                organisationEntity.PrimaryContactId = contact.Id;
            }

            organisationEntity.EndPointAssessorName = organisationUpdateDomainModel.EndPointAssessorName;
            organisationEntity.OrganisationStatus = organisationUpdateDomainModel.OrganisationStatus;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<Organisation>(organisationEntity);
            return organisationQueryViewModel;
        }

        public async Task Delete(string endPointAssessorOrganisationId)
        {
            var organisationEntity = _assessorDbContext.Organisations
                      .FirstOrDefault(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && q.OrganisationStatus != OrganisationStatus.Deleted);

            if (organisationEntity == null)
                throw (new NotFound());

            organisationEntity.DeletedAt = DateTime.Now;
            organisationEntity.OrganisationStatus = OrganisationStatus.Deleted;

            _assessorDbContext.MarkAsModified(organisationEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}