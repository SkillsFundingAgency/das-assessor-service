using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Data.Configuration
{
    public class ConfigurationWrapper : IConfigurationWrapper
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ConfigurationWrapper(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public string AssessmentOrgsUrl => "https://sfa-gov-uk.visualstudio.com/DefaultCollection/c39e0c0b-7aff-4606-b160-3566f3bbce23/_api/_versioncontrol/itemContent?repositoryId=9b4f676e-ce9a-4f10-a043-0ec9e5bf053c&path=%2FassessmentOrgs%2Flocal%2FassessmentOrgs.xlsx&amp;version=GBmaster&amp;contentOnly=false&amp;__v=5";

        public string GitUserName => Environment.GetEnvironmentVariable("DAS_GitUsername");
        public string GitPassword => Environment.GetEnvironmentVariable("DAS_GitPassword");

        public string DbConnectionString => _assessorDbContext.Database.GetDbConnection().ConnectionString;
    }
}
