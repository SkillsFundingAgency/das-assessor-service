# Digital Apprenticeships Service

## EPAO Onboarding and Certification

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service/blob/master/LICENSE.txt)

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Assessor Service Web|
| Info | A service which allows an End Point Assessment Organisation (EPAO) to register and assess standards |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-assessor-service?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=831&branchName=master) |
| Web  | https://localhost:5015/  |
|      | https://localhost:5015/find-an-assessment-opportunity  |

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Assessor External API |
| Info | An API which allows an EPAO to access Record a grade functionality programmatically |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-assessor-service?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=831&branchName=master) |
| Api  | https://www.gov.uk/guidance/record-a-grade-api  |

See [Support Site](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1731559639/Login+Service+-+Developer+Overview) for EFSA developer details.

### Developer Setup

#### Requirements

- Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
- Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on atleast v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/) 
- Administrator Access

- Optionally Install [Specflow](http://specflow.org/documentation/Installation/)

#### Setup

- Clone this repository
- Open Visual Studio as an administrator

##### Publish Database

- Build the solution SFA.DAS.AssessorService.sln
- Either use Visual Studio's `Publish Database` tool to publish the database project SFA.DAS.AssessorService.Database to name {{database name}} on {{local instance name}}

	or

- Create a database manually named {{database name}} on {{local instance name}} and run each of the `.sql` scripts in the SFA.DAS.AssessorService.Database project.

##### Config

- Get the das-assessor-service configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-assessor-service/SFA.DAS.AssessorService.json); which is a non-public repository.
- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.AssessorService_1.0, Data: {{The contents of the local config json file}}.
- Update Configuration SFA.DAS.AssessorService_1.0, Data { "SqlConnectionstring":"Server={{local instance name}};Initial Catalog={{database name}};Trusted_Connection=True;" }

##### Complete Data Setup

Follow the [EPAO Data Setup Guide](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1731395918/EPAO+-+Data+Setup+Guide#Assessor-Service---Initial-Setup) to populate local database test data.

#### To run a local copy you will also require 

- [Login Service](https://github.com/SkillsFundingAgency/das-login-service)
- [QnA API](https://github.com/SkillsFundingAgency/das-qna-api)
- [Admin Service](https://github.com/SkillsFundingAgency/das-admin-service)

##### And you may also require 

- [Assessor Functions](https://github.com/SkillsFundingAgency/das-assessor-functions)     

#### Run the solution

The default JSON configuration was created to work with dotnet run:

- Navigate to src/SFA.DAS.AssessorService.Web/
- run `dotnet restore`
- run `dotnet run`
- Open https://localhost:5015

- Navigate to src/SFA.DAS.AssessorService.Application.Api/
- run `dotnet restore`
- run `dotnet run`

**Note:** Running the solution from VS2019 is not supported currently as the Login Service (OpenId Identity Server 4) is configured by default for the client end point to originate at https://localhost:5015 which is not a valid port for IIS Express; altering the Login Service configuration is out of scope for this Readme.

#### Testing using Specflow (Optional)

Specflow is currently used for integrations testing the Internal API.
It is configured to run using the NUnit Test runner. 

As such it requires

1). To be ran using the NUnit Test Runner.

2). The SFA.DAS.AssessorService.Application.Api project to be running in advance.

3). The BaseAddress in the app.config to be set to the base address of the running 
SFA.DAS.AssessorService.Application.Api project.

#### SonarCloud Analysis (Optional)

SonarCloud analysis can be performed using a docker container which can be built from the included dockerfile.

    Docker must be running Windows containers in this instance

An example of the docker run command to analyse the code base can be found below. 

For this docker container to be successfully created you will need:
* docker running Windows containers
* a user on SonarCloud.io with permission to run analysis
* a SonarQube.Analysis.xml file in the root of the git repository.

This file takes the format:

```xml
<SonarQubeAnalysisProperties  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://www.sonarsource.com/msbuild/integration/2015/1">
<Property Name="sonar.host.url">https://sonarcloud.io</Property>
<Property Name="sonar.login">[Your SonarCloud user token]</Property>
</SonarQubeAnalysisProperties>
```     

##### Example:

_docker run [OPTIONS] IMAGE COMMAND_

[Docker run documentation](https://docs.docker.com/engine/reference/commandline/run/)

```docker run --rm -v c:/projects/das-assessor-service:c:/projects/das-assessor-service -w c:/projects/das-assessor-service 3d9151a444b2 powershell -F c:/projects/das-assessor-service/sonarcloud/analyse.ps1```

###### Options:

|Option|Description|
|---|---|
|--rm| Remove any existing containers for this image
|-v| Bind the current directory of the host to the given directory in the container ($PWD may be different on your platform). This should be the folder where the code to be analysed is
|-w| Set the working directory

###### Command:

Execute the analyse.ps1 PowerShell script	    

### Getting Started
Please follow the [Walkthrough](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1533345867/EPAO+-+Walkthrough); which is a non-public Wiki.

### Setup issues

If you get issues with localhost certificate validation when accessing the local login service (e.g. "AuthenticationException: The remote certificate is invalid according to the validation procedure") then run the following command from the login service directory to install the local dev certificates

```dotnet dev-certs https --trust``` 

In the dialog that appears confirm the certificate install. 

If you get issues with the target framework when building (e.g. it is targeting .net core 3 instead of 2.2.) then add a global.json file in each of the projects being run with the required target framework specified as follows

``` 
{
  "sdk": {
    "version": "2.2.207"
  }
}
```

to see the installed SDK versions run the following command

```dotnet --info```
