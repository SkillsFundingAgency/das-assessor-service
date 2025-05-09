﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IRegisterValidationRepository
    {
        Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId);
        Task<bool> EpaOrganisationExistsWithCompanyNumber(string organisationIdToExclude, string companyId);
        Task<bool> EpaOrganisationExistsWithCompanyNumber(string companyId);
        Task<bool> EpaOrganisationExistsWithCharityNumber(string organisationIdToExclude, string charityId);
        Task<bool> EpaOrganisationExistsWithCharityNumber(string charityId);
        Task<bool> EpaOrganisationExistsWithUkprn(long ukprn);
        Task<bool> OrganisationTypeExists(int organisationTypeId);
        Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationId);
        Task<bool> EpaOrganisationNameExists(string organisationName, string excludingOrganisationId = null);
        Task<bool> ContactIdIsValid(Guid contactId);
        Task<bool> EmailExists(string email);

        Task<bool> EmailExistsExcludeContact(string email, Guid excludingContactId);
        Task<bool> ContactIdIsValidForOrganisationId(Guid contactId, string organisationId);
        Task<bool> EpaOrganisationStandardExists(string organisationId, int standardCode);
        Task<bool> EpaOrganisationStandardVersionExists(string organisationId, int standardCode, List<string> standardVersions);
        
        Task<bool> ContactExists(Guid contactId);

        Task<bool> ContactDetailsAlreadyExist(string firstName, string lastName, string email, string phone,
            Guid? contactId);
        Task<bool> EpaOrganisationExistsWithRecognitionNumber(string recognitionNumber, string organisationId);
        Task<bool> CheckRecognitionNumberExists(string recognitionNumber);
    }
}