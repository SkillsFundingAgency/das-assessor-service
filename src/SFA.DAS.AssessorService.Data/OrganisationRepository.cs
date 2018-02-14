namespace SFA.DAS.AssessorService.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Entities;

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

        public async Task<Organisation> GetByUkPrn(int ukprn)
        {
            var organisation = await _assessorDbContext.Organisations                      
                         .FirstOrDefaultAsync(q => q.EndPointAssessorUKPRN == ukprn);

            return null;
        }
    }
}