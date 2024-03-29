﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public static class MergeOrganisationStatus
    {
        public const string InProgress = "In Progress";
        public const string Approved = "Approved";
        public const string Completed = "Completed";
        public const string Reverted = "Reverted";
    }

    public static class ReplicationType
    {
        public const string Before = "Before";
        public const string After = "After";
    }

    public class OrganisationMergingService : IOrganisationMergingService
    {
        private readonly AssessorDbContext _dbContext;

        public OrganisationMergingService(AssessorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MergeOrganisation> MergeOrganisations(Organisation primaryOrganisation, Organisation secondaryOrganisation, DateTime secondaryStandardsEffectiveTo, string actionedByUser)
        {
            if (null == primaryOrganisation) throw new ArgumentNullException(nameof(primaryOrganisation), "primaryOrganisation must be specified.");
            if (null == secondaryOrganisation) throw new ArgumentNullException(nameof(secondaryOrganisation), "secondaryOrganisation must be specified.");

            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync( async () =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Validation: you cannot use an organisation as a secondary organisation more than once.

                        if (HasAlreadyMergedAsSecondaryOrganisation(secondaryOrganisation.EndPointAssessorOrganisationId))
                        {
                            throw new Exception($"Cannot merge {secondaryOrganisation.EndPointAssessorOrganisationId}. It has already been merged into another organisation.");
                        }

                        // Create the MergeOrganisation

                        var mergeOrganisation = CreateMergeOrganisations(
                            primaryOrganisation,
                            secondaryOrganisation,
                            secondaryStandardsEffectiveTo,
                            actionedByUser);

                        // Create the "Before" snapshot

                        CreateStandardsSnapshot(mergeOrganisation, primaryOrganisation, "Before");
                        CreateStandardsSnapshot(mergeOrganisation, secondaryOrganisation, "Before");
                        CreateApplySnapshot(mergeOrganisation, secondaryOrganisation.EndPointAssessorOrganisationId, "Before");

                        // Perform the merge.                

                        MergeOrganisationStandardsAndVersions(
                            primaryOrganisation,
                            secondaryOrganisation,
                            actionedByUser,
                            secondaryStandardsEffectiveTo);

                        // Delete any in-progress applications for the secondary organisation

                        DeleteInProgressApplications(secondaryOrganisation, actionedByUser);

                        // Approve and complete the merge

                        ApproveMerge(mergeOrganisation, actionedByUser);
                        CompleteMerge(mergeOrganisation, actionedByUser);

                        // Now save all the changes.
                        // We do this here to make sure everything has worked and so that all the Id's are generated/wrired up
                        // ready for the after snapshot

                        await _dbContext.SaveChangesAsync();

                        // Now we know the merge has worked, create the "After" snapshot.

                        CreateStandardsSnapshot(mergeOrganisation, primaryOrganisation, "After");
                        CreateStandardsSnapshot(mergeOrganisation, secondaryOrganisation, "After");
                        CreateApplySnapshot(mergeOrganisation, secondaryOrganisation.EndPointAssessorOrganisationId, "After");
                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();

                        return mergeOrganisation;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });
        }

        private bool HasAlreadyMergedAsSecondaryOrganisation(string secondaryEndpointAssessorOrganisationId)
        {
            return _dbContext.MergeOrganisations.Any(mo => 
            mo.SecondaryEndPointAssessorOrganisationId == secondaryEndpointAssessorOrganisationId
            && (mo.Status != MergeOrganisationStatus.Reverted));
        }

        private void ApproveMerge(MergeOrganisation mo, string approvedByUser)
        {
            mo.ApprovedBy = approvedByUser;
            mo.ApprovedAt = DateTime.UtcNow;
            mo.Status = MergeOrganisationStatus.Approved;            
        }

        private void CompleteMerge(MergeOrganisation mo, string completedByUser)
        {
            mo.CompletedBy = completedByUser;
            mo.CompletedAt = DateTime.UtcNow;
            mo.Status = MergeOrganisationStatus.Completed;
        }

        private MergeOrganisation CreateMergeOrganisations(Organisation primaryOrganisation, Organisation secondaryOrganisation, DateTime secondaryStandardsEffectiveTo, string createdByUser)
        {
            var primaryContactName = _dbContext.Contacts.AsNoTracking().FirstOrDefault(c => c.Email == primaryOrganisation.PrimaryContact)?.DisplayName;
            var secondaryContactName = _dbContext.Contacts.AsNoTracking().FirstOrDefault(c => c.Email == secondaryOrganisation.PrimaryContact)?.DisplayName;

            var mo = new MergeOrganisation()
            {
                PrimaryEndPointAssessorOrganisationId = primaryOrganisation.EndPointAssessorOrganisationId,
                PrimaryEndPointAssessorOrganisationName = primaryOrganisation.EndPointAssessorName,
                PrimaryOrganisationEmail = primaryOrganisation.PrimaryContact,
                PrimaryContactName = primaryContactName,
                SecondaryEndPointAssessorOrganisationId = secondaryOrganisation.EndPointAssessorOrganisationId,
                SecondaryEndPointAssessorOrganisationName = secondaryOrganisation.EndPointAssessorName,
                SecondaryOrganisationEmail = secondaryOrganisation.PrimaryContact,
                SecondaryContactName = secondaryContactName,
                SecondaryEPAOEffectiveTo = secondaryStandardsEffectiveTo,
                CreatedBy = createdByUser,
                CreatedAt = DateTime.UtcNow,
                Status = MergeOrganisationStatus.InProgress,
            };
            _dbContext.MergeOrganisations.Add(mo);

            return mo;
        }

        private void CreateStandardsSnapshot(MergeOrganisation mo, Organisation organisation, string replicates)
        {
            // Make sure the standards are loaded.
            _dbContext.Entry(organisation)
                .Collection(os => os.OrganisationStandards)
                .Load();

            foreach (var os in organisation.OrganisationStandards)
            {
                mo.MergeOrganisationStandards.Add(new MergeOrganisationStandard(os, replicates));

                // Make sure the versions are loaded.
                _dbContext.Entry(os)
                    .Collection(osv => osv.OrganisationStandardVersions)
                    .Load();

                foreach (var osv in os.OrganisationStandardVersions)
                {
                    mo.MergeOrganisationStandardVersions.Add(new MergeOrganisationStandardVersion(osv, replicates));
                }

                // Make sure the areas are loaded.
                _dbContext.Entry(os)
                    .Collection(osv => osv.OrganisationStandardDeliveryAreas)
                    .Load();
                foreach (var osda in os.OrganisationStandardDeliveryAreas)
                {
                    mo.MergeOrganisationStandardDeliveryAreas.Add(new MergeOrganisationStandardDeliveryArea(osda, replicates));
                }
            }
        }

        private void MergeOrganisationStandardsAndVersions(Organisation primaryOrganisation, Organisation secondaryOrganisation, string createdByUser, DateTime secondaryStandardsEffectiveTo)
        {
            // Grab the primary contact for the primary organisation because we're going to need it.
            var primaryContact = _dbContext.Contacts.FirstOrDefault(c => c.EndPointAssessorOrganisationId == primaryOrganisation.EndPointAssessorOrganisationId && c.Email == primaryOrganisation.PrimaryContact);

            // Read all the standards from the secondary organisation.

            foreach (var secondaryOrganisationStandard in secondaryOrganisation.OrganisationStandards)
            {
                // Does the primary organisation have this standard already?
                var primaryOrganisationStandard = primaryOrganisation.OrganisationStandards.FirstOrDefault(pos => pos.StandardCode == secondaryOrganisationStandard.StandardCode);

                if (null == primaryOrganisationStandard)
                {
                    // No - so add the standard to the primary organisation.

                    primaryOrganisationStandard = new OrganisationStandard()
                    {
                        DateStandardApprovedOnRegister = secondaryOrganisationStandard.DateStandardApprovedOnRegister,
                        OrganisationStandardData = secondaryOrganisationStandard.OrganisationStandardData,
                        Comments = $"This standard/version has been acquired from a merge with {secondaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. {secondaryOrganisationStandard.Comments}",
                        ContactId = primaryContact?.Id,
                        EffectiveFrom = secondaryOrganisationStandard.EffectiveFrom,
                        EffectiveTo = secondaryOrganisationStandard.EffectiveTo,
                        EndPointAssessorOrganisationId = primaryOrganisation.EndPointAssessorOrganisationId,
                        StandardCode = secondaryOrganisationStandard.StandardCode,
                        StandardReference = secondaryOrganisationStandard.StandardReference,
                        Status = secondaryOrganisationStandard.Status,

                        OrganisationStandardVersions = new List<OrganisationStandardVersion>(),
                        OrganisationStandardDeliveryAreas = new List<OrganisationStandardDeliveryArea>(),
                    };
                    primaryOrganisationStandard.Comments = primaryOrganisationStandard.Comments.Substring(0, Math.Min(primaryOrganisationStandard.Comments.Length, 500));
                    primaryOrganisation.OrganisationStandards.Add(primaryOrganisationStandard);
                }

                // Now read all the versions for this standard for the secondary organisation.

                foreach(var secondaryOrganisationStandardVersion in secondaryOrganisationStandard.OrganisationStandardVersions)
                {
                    // Does the standard version exist for this standard in the primary organisation?

                    var primaryOrganisationStandardVersion = primaryOrganisationStandard.OrganisationStandardVersions.FirstOrDefault(posv => posv.StandardUId == secondaryOrganisationStandardVersion.StandardUId && posv.Version == secondaryOrganisationStandardVersion.Version);
                    if(null == primaryOrganisationStandardVersion)
                    {
                        // No - so add the standard version to the standard for the primary organisation.

                        primaryOrganisationStandardVersion = new OrganisationStandardVersion()
                        {
                            DateVersionApproved = secondaryOrganisationStandardVersion.DateVersionApproved,
                            Comments = $"This standard/version has been acquired from a merge with {secondaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. {secondaryOrganisationStandardVersion.Comments}",
                            EffectiveFrom = secondaryOrganisationStandardVersion.EffectiveFrom,
                            EffectiveTo = secondaryOrganisationStandardVersion.EffectiveTo,
                            StandardUId = secondaryOrganisationStandardVersion.StandardUId,
                            Status = secondaryOrganisationStandardVersion.Status,
                            Version = secondaryOrganisationStandardVersion.Version,
                        };
                        primaryOrganisationStandardVersion.Comments = primaryOrganisationStandardVersion.Comments.Substring(0, Math.Min(primaryOrganisationStandardVersion.Comments.Length, 500));
                        primaryOrganisationStandard.OrganisationStandardVersions.Add(primaryOrganisationStandardVersion);
                    }

                    // Mark the secondary standard version as ending
                    secondaryOrganisationStandardVersion.EffectiveTo = secondaryStandardsEffectiveTo;
                    secondaryOrganisationStandardVersion.Comments = $"This standard version has been merged in to {primaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. ${secondaryOrganisationStandardVersion.Comments}";
                    secondaryOrganisationStandardVersion.Comments = secondaryOrganisationStandardVersion.Comments.Substring(0, Math.Min(secondaryOrganisationStandardVersion.Comments.Length, 500));
                }

                foreach (var secondaryOrganisationStandardDeliveryArea in secondaryOrganisationStandard.OrganisationStandardDeliveryAreas)
                {
                    // Does the primary organisation standard have this delivery area?
                    var primaryOrganisationStandardDeliveryArea = primaryOrganisationStandard.OrganisationStandardDeliveryAreas.FirstOrDefault(posda => posda.DeliveryAreaId == secondaryOrganisationStandardDeliveryArea.DeliveryAreaId);

                    if(null == primaryOrganisationStandardDeliveryArea)
                    {
                        // No - so add the area

                        primaryOrganisationStandardDeliveryArea = new OrganisationStandardDeliveryArea()
                        {
                            DeliveryAreaId = secondaryOrganisationStandardDeliveryArea.DeliveryAreaId,
                            Comments = $"This delivery area has been acquired from a merge with {secondaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. {secondaryOrganisationStandardDeliveryArea.Comments}",
                            Status = secondaryOrganisationStandardDeliveryArea.Status,
                            OrganisationStandard = primaryOrganisationStandard,
                            DeliveryArea = secondaryOrganisationStandardDeliveryArea.DeliveryArea,
                        };
                        primaryOrganisationStandardDeliveryArea.Comments = primaryOrganisationStandardDeliveryArea.Comments.Substring(0, Math.Min(primaryOrganisationStandardDeliveryArea.Comments.Length, 500));
                        primaryOrganisationStandard.OrganisationStandardDeliveryAreas.Add(primaryOrganisationStandardDeliveryArea);
                    }

                    secondaryOrganisationStandardDeliveryArea.Comments = $"This delivery area has been merged in to {primaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. {secondaryOrganisationStandardDeliveryArea.Comments}";
                    secondaryOrganisationStandardDeliveryArea.Comments = secondaryOrganisationStandardDeliveryArea.Comments.Substring(0, Math.Min(secondaryOrganisationStandardDeliveryArea.Comments.Length, 500));
                }

                // Now set the effectiveTo and comments @ToDo: need content

                secondaryOrganisationStandard.EffectiveTo = secondaryStandardsEffectiveTo;
                secondaryOrganisationStandard.Comments = $"This standard has been merged in to Organisation {primaryOrganisation.EndPointAssessorOrganisationId} on {DateTime.UtcNow.ToString("dd MMM yyyy")}. {secondaryOrganisationStandard.Comments}";
                secondaryOrganisationStandard.Comments = secondaryOrganisationStandard.Comments.Substring(0, Math.Min(secondaryOrganisationStandard.Comments.Length, 500));
            }
        }

        private void CreateApplySnapshot(MergeOrganisation mergeOrganisation, string endpointAssessorOrganisationId, string replicates)
        {
            var applications = _dbContext.Applications.Include("Organisation").Where(e => e.Organisation.EndPointAssessorOrganisationId == endpointAssessorOrganisationId);
            foreach(var application in applications)
            {
                var mergeApplication = MergeApply.CreateFrom(application);
                mergeApplication.Replicates = replicates;
                mergeOrganisation.MergeSecondaryApplications.Add(mergeApplication);
            }            
        }

        private void DeleteInProgressApplications(Organisation organisation, string deletedByUser)
        {
            var applications = _dbContext.Applications.Where(e => e.Organisation.EndPointAssessorOrganisationId == organisation.EndPointAssessorOrganisationId);
            foreach (var application in applications)
            {
                if(application.ApplicationStatus == ApplyTypes.ApplicationStatus.InProgress)
                {
                    application.DeletedAt = DateTime.UtcNow;
                    application.ApplicationStatus = ApplyTypes.ApplicationStatus.Deleted;
                    application.DeletedBy = deletedByUser;
                }
            }
        }
    }
}
