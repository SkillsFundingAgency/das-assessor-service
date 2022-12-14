# EPAO Onboarding and Certification
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-assessor-service?repoName=SkillsFundingAgency%2Fdas-assessor-service&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2539&repoName=SkillsFundingAgency%2Fdas-assessor-service&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-assessor-service&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-assessor-service)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)]()
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)]()
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This repository represents the Assessors API code base. This service ...

## Developer Setup

### Requirements

In order to run this solution locally you will need the following:

* Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
* Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    * ASP.NET and web development
    * Azure development
* Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
* Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
* Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on atleast v5.3)
* Install [Azure Storage Explorer](http://storageexplorer.com/) 
* Administrator Access

* Optionally Install [Specflow](http://specflow.org/documentation/Installation/) (used for integration tests)

### Environment Setup

* Clone this repository and open Visual Studio as an administrator.
* Either use Visual Studio's `Publish Database` tool to publish the database project `SFA.DAS.AssessorService.Database` to the database name `{{database name}}` on `{{local instance name}}`, or create a database manually named `{{database name}}` on `{{local instance name}}` and run each of the `.sql` scripts in the `SFA.DAS.AssessorService.Database` project.
* **json file** - Get the `das-assessor-service` configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-assessor-service/SFA.DAS.AssessorService.json); which is a non-public repository.
* **Azure Table Storage Config** - Add the following data to your Azure Table Storage Config:

RowKey: SFA.DAS.AssessorService_1.0


PartitionKey: LOCAL

Data: 
```
{{The contents of the local config json file}}.
```
Make sure you update the `SqlConnectionString` value to your local instance name, `Initial Catalog` value to your database name, and `Trusted_Connection` value to True.
* Follow the [EPAO Data Setup Guide](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1731395918/EPAO+-+Data+Setup+Guide#Assessor-Service---Initial-Setup) to populate local database test data.

### Run the solution

The default JSON configuration was created to work with dotnet run:
* Navigate to src/SFA.DAS.AssessorService.Web/
* run `dotnet restore`
* run `dotnet run`
* Open https://localhost:5015

* Navigate to src/SFA.DAS.AssessorService.Application.Api/
* run `dotnet restore`
* run `dotnet run`

**Note:** Running the solution from VS2019 is not supported currently as the Login Service (OpenId Identity Server 4) is configured by default for the client end point to originate at https://localhost:5015 which is not a valid port for IIS Express; altering the Login Service configuration is out of scope for this Readme.

* To run a local copy you will also need the following solutions locally:
    * [Login Service](https://github.com/SkillsFundingAgency/das-login-service)
    * [QnA API](https://github.com/SkillsFundingAgency/das-qna-api)
    * [Admin Service](https://github.com/SkillsFundingAgency/das-admin-service)
*  And you may also require:  
    * [Assessor Functions](https://github.com/SkillsFundingAgency/das-assessor-functions)   

* Please follow the [Walkthrough](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1533345867/EPAO+-+Walkthrough) which is a non-public wiki to create an account and setup local test data. 

### Tests

This codebase includes unit tests and integration tests. These are all in seperate projects aptly named after the project they cover.

#### Unit Tests

There are several unit test projects in the solution built using C#, NUnit, Moq, FluentAssertions, .NET and AutoFixture.
* `SFA.DAS.AssessorService.Application.Api.External.UnitTests`
* `SFA.DAS.AssessorService.Application.Api.UnitTests`
* `SFA.DAS.AssessorService.Application.UnitTests`
* `SFA.DAS.AssessorService.Data.UnitTests`
* `SFA.DAS.AssessorService.Domain.UnitTests`
* `SFA.DAS.AssessorService.Web.UnitTests`

#### Integration Tests
There are two integration test projects: 
* `SFA.DAS.AssessorService.Application.Api.IntegrationTests`
* `SFA.DAS.AssessorService.Data.IntegrationTests`

Specflow is currently used for integration testing the internal API. It is configured to run using the NUnit Test runner. 
Hence, it requires:
* To be run using the NUnit Test Runner.
* The `SFA.DAS.AssessorService.Application.Api` project to already be running.
* The BaseAddress value in the app.config to be set to the base address of the running `SFA.DAS.AssessorService.Application.Api` project.

### SonarCloud Analysis (Optional)

SonarCloud analysis can be performed using a docker container which can be built from the included dockerfile.

    Docker must be running Windows containers in this instance

An example of the `docker run` command to analyse the code base can be found below. 

For this docker container to be successfully created you will need:
* docker running Windows containers
* a user on `SonarCloud.io` with permission to run analysis
* a `SonarQube.Analysis.xml` file in the root of the git repository.

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

Execute the `analyse.ps1` PowerShell script	    

### Setup issues

If you get issues with localhost certificate validation when accessing the local login service (e.g. "AuthenticationException: The remote certificate is invalid according to the validation procedure") then run the following command from the login service directory to install the local dev certificates:

```dotnet dev-certs https --trust``` 

Confirm the certificate install in the dialog that appears. 

If you get issues with the target framework when building (e.g. it is targeting .net core 3 instead of 2.2.) then add a `global.json` file in each of the projects being run with the required target framework specified as follows:

``` 
{
  "sdk": {
    "version": "2.2.207"
  }
}
```

To see the installed SDK versions run the following command:

```dotnet --info```
