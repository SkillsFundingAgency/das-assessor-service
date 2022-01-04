using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
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

        public async Task<MergeOrganisation> MergeOrganisations(Organisation primaryOrganisation, Organisation secondaryOrganisation, Guid mergedByUserId)
        {
            if (null == primaryOrganisation) throw new ArgumentNullException(nameof(primaryOrganisation), "primaryOrganisation must be specified.");
            if (null == secondaryOrganisation) throw new ArgumentNullException(nameof(secondaryOrganisation), "secondaryOrganisation must be specified.");

            // Create the MergeOrganisation and "Before" snapshot.

            var mergeOrganisation = CreateMergeOrganisations(
                primaryOrganisation.EndPointAssessorOrganisationId, 
                secondaryOrganisation.EndPointAssessorOrganisationId, 
                mergedByUserId);

            // Perform the merge.
            

            // Create the "After" snapshot.

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                // check rollback
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

            CreateStandardsSnapshot(mo, primaryEndpointAssessorOrganisationId, "Before");
            CreateStandardsSnapshot(mo, secondaryEndpointAssessorOrganisationId, "Before");

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
                ReferenceNumber = "0",  // what is this? standard code ?
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
    }
}
