
SonarScanner.MSBuild.exe begin /k:"SkillsFundingAgency_das-assessor-service" /o:"educationandskillsfundingagency" /d:sonar.exclusions="**/*.sql"
Nuget Restore c:\projects\das-assessor-service\src\SFA.DAS.AssessorService.sln 
MSBuild.exe c:\projects\das-assessor-service\src\SFA.DAS.AssessorService.sln /t:Rebuild
SonarScanner.MSBuild.exe end 