using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private Dictionary<string, Organisation> _organisations;

        public OrganisationRepository()
        {
            _organisations = new Dictionary<string, Organisation>()
            {
                { "c208ec2f-ecbf-4f1e-a6e2-8b96c6a6461b", new Organisation() {EpaoOrgId = "EPA0002", Name = "An EPAO", UkPrn = "c208ec2f-ecbf-4f1e-a6e2-8b96c6a6461b"}},
                { "c93b96be-5625-4ccf-8d81-bac1df0272d4", new Organisation() {EpaoOrgId = "EPA0045", Name = "An Completely different EPAO", UkPrn = "c93b96be-5625-4ccf-8d81-bac1df0272d4"}}
            };
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
                new Organisation() {EpaoOrgId = "EPA0001", Name = "BCS, The Chartered Institute for IT"}
            }.AsEnumerable();
        }

        public Task<Organisation> GetByUkPrn(string ukprn)
        {
            return Task.FromResult(_organisations[ukprn]);
        }
    }
}