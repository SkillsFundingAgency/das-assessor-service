﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffLearnerRepository : IStaffLearnerRepository
    {
        private readonly IAssessorUnitOfWork _assessorUnitOfWork;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IDbConnection _connection;

        public StaffLearnerRepository(IAssessorUnitOfWork assessorUnitOfWork, ILearnerRepository learnerRepository, IDbConnection connection)
        {
            _assessorUnitOfWork = assessorUnitOfWork;
            _learnerRepository = learnerRepository;
            _connection = connection;
        }

        public async Task<IEnumerable<Learner>> SearchForLearnerByCertificateReference(string certRef)
        {
            var results = new List<Learner>();
            
            var cert = await _assessorUnitOfWork.AssessorDbContext.StandardCertificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c => c.CertificateReference == certRef);

            if(cert != null)
            {
                var learner = await _learnerRepository.Get(cert.Uln, cert.StandardCode);

                if (learner is null)
                {
                    learner = new Learner
                    {
                        Uln = cert.Uln,
                        GivenNames = cert.CertificateData.LearnerGivenNames,
                        FamilyName = cert.CertificateData.LearnerFamilyName,
                        StdCode = cert.StandardCode,
                        LearnRefNumber = cert.LearnRefNumber,
                        LearnStartDate = cert.CertificateData.LearningStartDate ?? DateTime.MinValue,
                        EpaOrgId = cert.Organisation?.EndPointAssessorOrganisationId,
                        UkPrn = cert.Organisation?.EndPointAssessorUkprn ?? int.MinValue
                    };
                }

                results.Add(learner);
            }

            return results;
        }

        public async Task<IEnumerable<Learner>> SearchForLearnerByUln(long uln, int page, int pageSize)
        {
            /* get rows from Certificates and/or Learner - there must be a better way! - and it does not need paging as there will never be as many as 10 rows*/
            return (await _connection.QueryAsync<Learner>(
                            @"SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                     [Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[PlannedEndDate]
                            FROM (
                            SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                   [Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[PlannedEndDate],
                             row_number() OVER (PARTITION BY uln,Stdcode ORDER BY choice) rownumber2
                            FROM (
                            SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                  [Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[PlannedEndDate], 2 choice
                            FROM (
                            SELECT Row_number() OVER (ORDER BY certificatereferenceid DESC) rownumber, 
                            ce.id,Uln,
                            json_value(certificatedata, '$.LearnerFamilyName') FamilyName,
                            json_value(certificatedata, '$.LearnerGivenNames') GivenNames,
                            ProviderUkPrn Ukprn,
                            Standardcode Stdcode, 
                            json_value(certificatedata, '$.LearningStartDate') LearnStartDate,
                            og.EndPointAssessorOrganisationId EPAOrgId,
                            NULL FundingModel,
                            NULL ApprenticeshipId,
                            NULL [Source],
                            ce.CreatedAt,
                            ce.UpdatedAt,
                            [LearnRefNumber],
                            1 [CompletionStatus],
                            NULL [PlannedEndDate]
                            FROM [StandardCertificates] ce 
                            JOIN [Organisations] og ON ce.OrganisationId = og.Id
                            WHERE ce.[Status] <> 'Deleted'
                            AND [Uln] = @uln 
                            ) ab1 WHERE rownumber = 1
                            UNION
                            SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                  [Source] ,NULL [CreatedAt] ,NULL [UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[PlannedEndDate], 4 choice
                            FROM [Learner] 
                            WHERE [Uln] = @uln 
                            ) ab2 
                            ) ab3 WHERE rownumber2 = 1 
                            ORDER BY [UpdatedAt] DESC, [CreatedAt] DESC  
                            OFFSET @skip ROWS 
                            FETCH NEXT @take ROWS ONLY",
                new { uln, skip = (page - 1) * pageSize, take = pageSize} )).ToList();
        }

        public async Task<int> SearchForLearnerByUlnCount(long uln)
        {
            var result = await _connection.QuerySingleAsync<int>(
                            @"SELECT COUNT(ab3.Id)
                              FROM (
                                    SELECT [Id] ,[Uln], [StdCode], row_number() OVER (PARTITION BY Uln, StdCode ORDER BY choice) rownumber2
                                    FROM (
                                    SELECT [Id] ,[Uln], [StdCode], 2 choice
                                    FROM (
                                    SELECT Row_number() OVER (ORDER BY CertificateReferenceId DESC) rownumber, ce.id, Uln, StandardCode StdCode
                                    FROM [StandardCertificates] ce 
                                    JOIN [Organisations] og ON ce.OrganisationId = og.Id
                                    WHERE ce.[Status] <> 'Deleted'
                                    AND [Uln] = @uln 
                                    ) ab1 WHERE rownumber = 1
                                    UNION
                                    SELECT [Id] ,[Uln], [StdCode], 4 choice
                                    FROM [Learner] 
                                    WHERE [Uln] = @uln 
                                    ) ab2 
                                ) ab3 WHERE rownumber2 = 1",
                            new { uln });
            return result;
        }

        public async Task<IEnumerable<Learner>> SearchForLearnerByName(string learnerName, int page, int pageSize)
        {
            var result = await _connection.QueryAsync<Learner>("StaffSearchCertificates",
                new { Search = learnerName, Skip = (page - 1) * pageSize, Take = pageSize },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<int> SearchForLearnerByNameCount(string learnerName)
        {
            var result = await _connection.QuerySingleAsync<int>("StaffSearchCertificates_Count",
                                                                 new { Search = learnerName },
                                                                 commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<StaffReposSearchResult> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest)
        {
            var searchResult = new StaffReposSearchResult
            {
                PageOfResults = (await _connection.QueryAsync<Learner>(
                        @"SELECT org.EndPointAssessorOrganisationId, cert.Uln, JSON_VALUE(CertificateData, '$.LearnerGivenNames') AS GivenNames, JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS FamilyName, cert.StandardCode AS StdCode, cert.UpdatedAt as LastUpdatedAt 
                            FROM StandardCertificates cert
                            INNER JOIN Organisations org ON org.Id = cert.OrganisationId
                            WHERE org.EndPointAssessorOrganisationId = @epaOrgId
                            ORDER BY cert.UpdatedAt DESC                     
                            OFFSET @skip ROWS 
                            FETCH NEXT @take ROWS ONLY",
                        new { epaOrgId = searchRequest.SearchQuery.ToLower(), skip = (searchRequest.Page - 1) * 10, take = 10 }))
                    .ToList(),
                TotalCount = await _connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1)
                    FROM StandardCertificates cert
                        INNER JOIN Organisations org ON org.Id = cert.OrganisationId
                    WHERE org.EndPointAssessorOrganisationId = @epaOrgId", new { epaOrgId = searchRequest.SearchQuery.ToLower() })
            };

            return searchResult;
        }
    }
}