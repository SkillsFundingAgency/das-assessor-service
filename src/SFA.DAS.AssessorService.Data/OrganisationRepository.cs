using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public Task CreateNewOrganisation(Organisation newOrganisation)
        {
            Debug.WriteLine("Saving Org");
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Organisation>> GetAllOrganisations()
        {
            return new List<Organisation>()
            {
                new Organisation() { EndPointAssessorOrganisationId = "EPA0001", EndPointAssessorName = "BCS, The Chartered Institute for IT" }
            }.AsEnumerable();
        }

        public async Task<OrganisationQueryViewModel> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations                      
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn);
            if (organisation == null)
                return null;                
            
            var organisationViewModel = Mapper.Map<OrganisationQueryViewModel>(organisation);
            return organisationViewModel;
        }
    }
}