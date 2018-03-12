# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  EPAO Onboarding and Certification

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/831/badge)

### Developer Setup

#### Requirements

- Install [Visual Studio 2017 Enterprise](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/)
- Install [Specflow](http://specflow.org/documentation/Installation/)
- Administrator Access

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.AssessorService_1.0, Data: {The contents of the local config json file}.

##### Open the solution

- Open Visual studio as an administrator
- Open the solution
- Set SFA.DAS.AssessorService.Web and SFA.DAS.AssessorService.Api as startup projects
- Running the solution will launch the site and API in your browser

-or-

- Navigate to src/SFA.DAS.AssessorService.Web/
- run `dotnet restore`
- run `dotnet run`
- Open https://localhost:5015

Running Specflow

Specflow is currently used for integrations testing the Internal API.
In order to run it you should take a backup copy of a clean database.
and configure the app.config file in the SFA.DAS.AssessorService.Application.Api.Specflow.Tests project to point 
to the the backup copy location.

A clean database can be created by 

1). Deleting the AssessorDB database currently pointed to by the  SFA.DAS.AssessorService.Application.Api project.

2). Running update-database from the package manager console window.
(Make sure the current startup project is set to SFA.DAS.AssessorService.Data an also 
Defaut project in the Package Manager Console is set to SFA.DAS.AssessorService.Data).

3). Running the SFA.DAS.AssessorService.Application.Api project to populate the database.

The folowing variables are configurable in the SFA.DAS.AssessorService.Application.Api.Specflow.Tests
project app.config

<add key="RestoreDatabase" value="c:\backup\SFA.DAS.AssessorService.Application.Api.bak" />
<add key="DatabaseName" value="AssessorDB" />
<add key="BaseAddress" value="http://localhost:59021/" />

where:

RestoreDatabase = database backup location.
DatabaseName = The Database currently in use by the SFA.DAS.AssessorService.Application.Api project.
BaseAddress = Base url address for the SFA.DAS.AssessorService.Application.Api project.


    
    


