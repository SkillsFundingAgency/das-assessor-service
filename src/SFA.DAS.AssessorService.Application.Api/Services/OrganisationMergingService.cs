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
        public const string Deleted = "Deleted";
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

        public async Task<MergeOrganisation> MergeOrganisations(Organisation primaryOrganisation, Organisation secondaryOrganisation, Guid mergedByUserId, DateTime secondaryStandardsEffectiveTo)
        {
            if (null == primaryOrganisation) throw new ArgumentNullException(nameof(primaryOrganisation), "primaryOrganisation must be specified.");
            if (null == secondaryOrganisation) throw new ArgumentNullException(nameof(secondaryOrganisation), "secondaryOrganisation must be specified.");

            // Create the MergeOrganisation

            var mergeOrganisation = CreateMergeOrganisations(
                primaryOrganisation.EndPointAssessorOrganisationId, 
                secondaryOrganisation.EndPointAssessorOrganisationId, 
                mergedByUserId);

            // Create the "Before" snapshot

            CreateStandardsSnapshot(mergeOrganisation, primaryOrganisation.EndPointAssessorOrganisationId, "Before");
            CreateStandardsSnapshot(mergeOrganisation, secondaryOrganisation.EndPointAssessorOrganisationId, "Before");

            // Perform the merge.

            MergeOrganisationStandardsAndVersions(
                primaryOrganisation,
                secondaryOrganisation,
                mergedByUserId,
                secondaryStandardsEffectiveTo);

            // Create the "After" snapshot.

            CreateStandardsSnapshot(mergeOrganisation, primaryOrganisation.EndPointAssessorOrganisationId, "After");
            CreateStandardsSnapshot(mergeOrganisation, secondaryOrganisation.EndPointAssessorOrganisationId, "After");

            // check
            /*
            List<Object> modifiedOrAddedEntities = _dbContext.ChangeTracker.Entries()
                .Where(x => x.State == Microsoft.EntityFrameworkCore.EntityState.Added
                    || x.State == Microsoft.EntityFrameworkCore.EntityState.Modified
                    )
                .Select(x => x.Entity).ToList();
            */

            // Now save all the changes.
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                // check rollback has worked
                throw ex;
            }

            return mergeOrganisation;
        }

        private MergeOrganisation CreateMergeOrganisations(string primaryEndpointAssessorOrganisationId, string secondaryEndpointAssessorOrganisationId, Guid createdByUserId)
        {
            var mo = new MergeOrganisation()
            {
                PrimaryEndPointAssessorOrganisationId = primaryEndpointAssessorOrganisationId,
                SecondaryEndPointAssessorOrganisationId = secondaryEndpointAssessorOrganisationId,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                ApprovedBy = createdByUserId,
                ApprovedAt = DateTime.UtcNow,
                Status = MergeOrganisationStatus.Approved,  // auto-approved for now
            };
            _dbContext.MergeOrganisations.Add(mo);

            return mo;
        }

        private void CreateStandardsSnapshot(MergeOrganisation mo, string endpointAssessorOrganisationId, string replicates)
        {
            var organisationStandards = _dbContext.OrganisationStandard.Where(e => e.EndPointAssessorOrganisationId == endpointAssessorOrganisationId);
            foreach (var os in organisationStandards)
            {
                mo.MergeOrganisationStandards.Add(CreateMergeOrganisationStandard(replicates, os));

                var organisationStandardVersions = _dbContext.OrganisationStandardVersion.Where(e => e.OrganisationStandardId == os.Id);
                foreach (var osv in organisationStandardVersions)
                {
                    mo.MergeOrganisationStandardVersions.Add(CreateMergeOrganisationStandardVersion(replicates, osv));
                }

                var organisationStandardDeliveryAreas = _dbContext.OrganisationStandardDeliveryAreas.Where(e => e.OrganisationStandardId == os.Id);
                foreach (var osda in organisationStandardDeliveryAreas)
                {
                    mo.MergeOrganisationStandardDeliveryAreas.Add(CreateMergeOrganisationStandardDeliveryArea(replicates, osda));
                }
            }
        }

        private MergeOrganisationStandard CreateMergeOrganisationStandard(string replicates, OrganisationStandard sourceOrganisationStandard)
        {
            var mos = new MergeOrganisationStandard()
            {
                EndPointAssessorOrganisationId = sourceOrganisationStandard.EndPointAssessorOrganisationId,
                StandardCode = sourceOrganisationStandard.StandardCode,
                StandardReference = sourceOrganisationStandard.StandardReference,
                EffectiveFrom = sourceOrganisationStandard.EffectiveFrom,
                EffectiveTo = sourceOrganisationStandard.EffectiveTo,
                DateStandardApprovedOnRegister = sourceOrganisationStandard.DateStandardApprovedOnRegister,
                Comments = sourceOrganisationStandard.Comments,
                Status = sourceOrganisationStandard.Status,
                ContactId = sourceOrganisationStandard.ContactId,
                OrganisationStandardData = sourceOrganisationStandard.OrganisationStandardData,
                Replicates = replicates,                
            };

            return mos;
        }
        private MergeOrganisationStandardVersion CreateMergeOrganisationStandardVersion(string replicates, OrganisationStandardVersion sourceOrganisationStandardVersion)
        {
            var mosv = new MergeOrganisationStandardVersion()
            {
                StandardUid = sourceOrganisationStandardVersion.StandardUId,
                Version = sourceOrganisationStandardVersion.Version,
                Replicates = replicates,
                OrganisationStandardId = sourceOrganisationStandardVersion.OrganisationStandardId,
                Status = sourceOrganisationStandardVersion.Status,
                Comments = sourceOrganisationStandardVersion.Comments,
                DateVersionApproved = sourceOrganisationStandardVersion.DateVersionApproved,
                EffectiveFrom = sourceOrganisationStandardVersion.EffectiveFrom,
                EffectiveTo = sourceOrganisationStandardVersion.EffectiveTo,
            };

            return mosv;
        }
        private MergeOrganisationStandardDeliveryArea CreateMergeOrganisationStandardDeliveryArea(string replicates, OrganisationStandardDeliveryArea sourceOrganisationStandardDeliveryArea)
        {
            var mosda = new MergeOrganisationStandardDeliveryArea()
            {
                Replicates = replicates,
                DeliveryAreaId = sourceOrganisationStandardDeliveryArea.DeliveryAreaId,
                OrganisationStandardId = sourceOrganisationStandardDeliveryArea.OrganisationStandardId,
                Status = sourceOrganisationStandardDeliveryArea.Status,
                Comments = sourceOrganisationStandardDeliveryArea.Comments,
            };

            return mosda;
        }


        private void MergeOrganisationStandardsAndVersions(Organisation primaryOrganisation, Organisation secondaryOrganisation, Guid createdByUserId, DateTime secondaryStandardsEffectiveTo)
        {
            // @ToDo: a bit monolithic - can this be refactored in to something more readable?

            // Grab the primary contact for the primary organisation because we're going to need it.
            var primaryContact = _dbContext.Contacts.FirstOrDefault(c => c.EndPointAssessorOrganisationId == primaryOrganisation.EndPointAssessorOrganisationId && c.Email == primaryOrganisation.PrimaryContact);

            // Read all the standards from the secondary organisation.

            foreach(var secondaryOrganisationStandard in secondaryOrganisation.OrganisationStandards)
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
                        Comments = $"{secondaryOrganisationStandard.Comments} ** this standard has been merged from Organisation {secondaryOrganisation.EndPointAssessorOrganisationId}",
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
                    primaryOrganisation.OrganisationStandards.Add(primaryOrganisationStandard);
                }

                // Now read all the versions for this standard for the secondary organisation.
                var secondaryOrganisationStandardVersions = _dbContext.OrganisationStandardVersion.Where(sosv => sosv.OrganisationStandardId == secondaryOrganisationStandard.Id);
                foreach(var secondaryOrganisationStandardVersion in secondaryOrganisationStandardVersions)
                {
                    // Does the standard version exist for this standard in the primary organisation?

                    var primaryOrganisationStandardVersion = primaryOrganisationStandard.OrganisationStandardVersions.FirstOrDefault(posv => posv.StandardUId == secondaryOrganisationStandardVersion.StandardUId && posv.Version == secondaryOrganisationStandardVersion.Version);
                    if(null == primaryOrganisationStandardVersion)
                    {
                        // No - so add the standard version to the standard for the primary organisation.

                        primaryOrganisationStandardVersion = new OrganisationStandardVersion()
                        {
                            //OrganisationStandardId = existingPrimaryOrganisationStandard.Id,
                            DateVersionApproved = secondaryOrganisationStandardVersion.DateVersionApproved,
                            Comments = $"{secondaryOrganisationStandardVersion.Comments} ** This standard version has been merged from Organisation {secondaryOrganisation.EndPointAssessorOrganisationId}",
                            EffectiveFrom = secondaryOrganisationStandardVersion.EffectiveFrom,
                            EffectiveTo = secondaryOrganisationStandardVersion.EffectiveTo,
                            StandardUId = secondaryOrganisationStandardVersion.StandardUId,
                            Status = secondaryOrganisationStandardVersion.Status,
                            Version = secondaryOrganisationStandardVersion.Version,
                        };
                        primaryOrganisationStandard.OrganisationStandardVersions.Add(primaryOrganisationStandardVersion);
                    }

                    // Mark the secondary standard version as ending
                    secondaryOrganisationStandardVersion.EffectiveTo = secondaryStandardsEffectiveTo;
                    secondaryOrganisationStandardVersion.Comments = $"** This standard version has been merged in to Organisation {primaryOrganisation.EndPointAssessorOrganisationId}";
                }

                // Read all the delivery areas for this standard from the secondary organisation

                var secondaryOrganisationStandardDeliveryAreas = _dbContext.OrganisationStandardDeliveryAreas.Where(sosda => sosda.OrganisationStandardId == secondaryOrganisationStandard.Id);

                // Merge in any missing delivery areas that the primary organisation doesn't have

                foreach(var secondaryOrganisationStandardDeliveryArea in secondaryOrganisationStandardDeliveryAreas)
                {
                    // Does the primary organisation standard have this delivery area?
                    var primaryOrganisationStandardDeliveryArea = primaryOrganisationStandard.OrganisationStandardDeliveryAreas.FirstOrDefault(posda => posda.DeliveryAreaId == secondaryOrganisationStandardDeliveryArea.DeliveryAreaId);

                    if(null == primaryOrganisationStandardDeliveryArea)
                    {
                        // No - so add the area

                        primaryOrganisationStandardDeliveryArea = new OrganisationStandardDeliveryArea()
                        {
                            DeliveryAreaId = secondaryOrganisationStandardDeliveryArea.DeliveryAreaId,
                            Comments = $"{secondaryOrganisationStandardDeliveryArea.Comments} ** This delivery area has been merged from Organsation {secondaryOrganisation.EndPointAssessorOrganisationId}",
                            Status = secondaryOrganisationStandardDeliveryArea.Status,
                            OrganisationStandard = primaryOrganisationStandard,
                            DeliveryArea = secondaryOrganisationStandardDeliveryArea.DeliveryArea,
                        };
                        primaryOrganisationStandard.OrganisationStandardDeliveryAreas.Add(primaryOrganisationStandardDeliveryArea);
                    }

                    secondaryOrganisationStandardDeliveryArea.Comments = $"** This delivery area has been merged in to Organisation {primaryOrganisation.EndPointAssessorOrganisationId}";
                }


                // Now set the effectiveTo and comments @ToDo: need content

                secondaryOrganisationStandard.EffectiveTo = secondaryStandardsEffectiveTo;
                secondaryOrganisationStandard.Comments = $"{secondaryOrganisationStandard.Comments} ** this standard has been merged in to Organisation {primaryOrganisation.EndPointAssessorOrganisationId}";
            }
        }
    }
}
