using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport
{
    public static class ConfigurationWrapper
    {
        public static string AssessmentOrgsUrl => "https://sfa-gov-uk.visualstudio.com/DefaultCollection/c39e0c0b-7aff-4606-b160-3566f3bbce23/_api/_versioncontrol/itemContent?repositoryId=9b4f676e-ce9a-4f10-a043-0ec9e5bf053c&path=%2FassessmentOrgs%2Flocal%2FassessmentOrgs.xlsx&amp;version=GBmaster&amp;contentOnly=false&amp;__v=5";

        public static string GitUserName => Environment.GetEnvironmentVariable("DAS_GitUsername");
        public static string GitPassword => Environment.GetEnvironmentVariable("DAS_GitPassword");

        public static string AccessorDbConnectionString =>
            @"Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.AssessorService.Database;Integrated Security=True; MultipleActiveResultSets=True;";

    }

}
