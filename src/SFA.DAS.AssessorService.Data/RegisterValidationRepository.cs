using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterValidationRepository: Repository, IRegisterValidationRepository
    {
        public RegisterValidationRepository(IUnitOfWork unitOfWork)
            : base (unitOfWork)
        {
        }

        public async Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE EndPointAssessorOrganisationId = @organisationId";

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationId });
        }

        public async Task<bool> EpaOrganisationExistsWithCompanyNumber(string organisationIdToExclude, string companyNumber)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber " +
                "AND EndPointAssessorOrganisationId != @organisationIdToExclude";

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, companyNumber });
        }

        public async Task<bool> EpaOrganisationExistsWithCompanyNumber(string companyNumber)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { companyNumber });
        }

        public async Task<bool> EpaOrganisationExistsWithCharityNumber(string organisationIdToExclude, string charityNumber)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber " +
                "AND EndPointAssessorOrganisationId != @organisationIdToExclude";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, charityNumber });
        }

        public async Task<bool> EpaOrganisationExistsWithCharityNumber(string charityNumber)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { charityNumber });
        }

        public async Task<bool> EpaOrganisationExistsWithUkprn(long ukprn)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE EndPointAssessorUkprn = @ukprn";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { ukprn });
        }

        public async Task<bool> OrganisationTypeExists(int organisationTypeId)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationType] " +
                "WHERE Id = @organisationTypeId";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationTypeId });
        }

        public async Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationIdToExclude)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE EndPointAssessorOrganisationId != @organisationIdToExclude and EndPointAssessorUkprn = @ukprn";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, ukprn });
        }

        public async Task<bool> EpaOrganisationStandardExists(string organisationId, int standardCode)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationStandard] " +
                "WHERE EndPointAssessorOrganisationId = @organisationId and standardCode = @standardCode";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationId, standardCode });
        }

        public async Task<bool> EpaOrganisationStandardVersionExists(string organisationId, int standardCode, List<string> standardVersions)
        {
            var selectOrganisationStandardId =
                "select id FROM [OrganisationStandard] " +
                "WHERE EndPointAssessorOrganisationId = @organisationId and standardCode = @standardCode " +
                "AND (EffectiveTo !=null OR EffectiveTo > GetDate())";
            var organisationStandardId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(selectOrganisationStandardId, new { organisationId, standardCode });
            if (default(int) == organisationStandardId) return false;

            if (null == standardVersions || !standardVersions.Any()) return false;

            var standardExists = false;
            foreach (var sv in standardVersions)
            {
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationStandardVersion] " +
                    "WHERE OrganisationStandardId = @organisationStandardId and Version = @version";
                standardExists = await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationStandardId, version = sv });
                if (standardExists) continue;
            }

            return standardExists;
        }

        public async Task<bool> EpaOrganisationNameExists(string organisationName, string excludingOrganisationId = null)
        {
            var sqlToCheckExists =
                "SELECT CASE COUNT(0) WHEN 0 THEN 0 ELSE 1 END Result FROM [Organisations] " +
                "WHERE EndPointAssessorName = @organisationName";

            if (!string.IsNullOrEmpty(excludingOrganisationId))
            {
                sqlToCheckExists += " AND EndPointAssessorOrganisationId != @excludingOrganisationId";
            }

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationName, excludingOrganisationId });
        }

        public async Task<bool> ContactIdIsValid(Guid contactId)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                "WHERE id  = @contactId";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId });
        }

        public async Task<bool> ContactIdIsValidForOrganisationId(Guid contactId, string organisationId)
        {
            var sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                "WHERE id = @ContactId and EndPointAssessorOrganisationId = @organisationId";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId, organisationId });
        }

        public async Task<bool> EmailExistsExcludeContact(string email, Guid excludingContactId)
        {
            const string sqlToCheckExists =
                "SELECT CASE COUNT(0) WHEN 0 THEN 0 ELSE 1 END Result FROM [Contacts] " +
                "WHERE Email = @email AND Id != @excludingContactId";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { email, excludingContactId });
        }

        public async Task<bool> ContactExists(Guid contactId)
        {
            const string sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                "WHERE id  = @contactId";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId });
        }

        public async Task<bool> ContactDetailsAlreadyExist(string firstName, string lastName, string email, string phone, Guid? contactId)
        {
            var sqlToCheckExists =
                 "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                 "WHERE GivenNames = @firstName and FamilyName = @lastName and email = @email";

            sqlToCheckExists = !string.IsNullOrEmpty(phone)
                ? sqlToCheckExists + " and phonenumber = @phone"
                : sqlToCheckExists + " and phonenumber is null";

            if (contactId != null)
                sqlToCheckExists = sqlToCheckExists + " and id != @contactId ";

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId, firstName, lastName, email, phone });
        }

        public async Task<bool> EmailExists(string email)
        {
            const string sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                "WHERE email  = @email";
            
            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { email });
        }

        public async Task<bool> EpaOrganisationExistsWithRecognitionNumber(string recognitionNumber, string organisationId = null)
        {
            var sqlToCheckExists = 
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                "WHERE LOWER(RecognitionNumber) = @recognitionNumber";

            if (!string.IsNullOrEmpty(organisationId)) { sqlToCheckExists += " AND LOWER(EndPointAssessorOrganisationId) <> @organisationId"; }

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { recognitionNumber, organisationId });
        }

        public async Task<bool> CheckRecognitionNumberExists(string recognitionNumber)
        {
            const string sqlToCheckExists =
                "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OfqualOrganisation] " +
                "WHERE LOWER(RecognitionNumber) = @recognitionNumber";

            return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { recognitionNumber });
        }
    }
}