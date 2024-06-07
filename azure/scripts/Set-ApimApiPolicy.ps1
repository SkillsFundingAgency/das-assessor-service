<#
.SYNOPSIS
Add a policy to an API in an APIM instance

.PARAMETER ResourceGroupName
The name of the resource group that contains the APIM instance

.PARAMETER ServiceName
The name of the APIM instnace

.PARAMETER ApiId
The ApiId of the API to update. This will be the API Name if it was created using ARM templates but with hyphens between each word e.g. my-api-id

.PARAMETER ApimApiPolicyFilePath
The full path to the XML file containing the policy to apply to the API

.PARAMETER ApplicationIdentifierUri
The Application Identifier URI of the API app registration

#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [String]$ResourceGroupName,
    [Parameter(Mandatory=$true)]
    [String]$ServiceName,
    [Parameter(Mandatory=$true)]
    [String]$ApiId,
    [Parameter(Mandatory=$true)]
    [String]$ApimApiPolicyFilePath,
    [Parameter(Mandatory=$true)]
    [String]$ApplicationIdentifierUri
)

try {

    # --- Build context and retrieve apiid
    Write-Host "Building APIM context for $ResourceGroupName\$ServiceName"
    $ApimContext = New-AzApiManagementContext -ResourceGroupName $ResourceGroupName -ServiceName $ServiceName

    #Verify ApplicationIdentifierUri
    Write-Host "ApplicationIdentifierUri = $ApplicationIdentifierUri"

    # Ensure policy file exists
    Write-Host "Test that policy file exists $ApimApiPolicyFilePath"

    # Define the relative path to the file from the root directory
    $relativePath = "SFA.DAS.AssessorService/azure/api-management/policies/apis"
    $fileName = "das-assessor-service-api-external.xml"

    # Get the value of System.DefaultWorkingDirectory from the environment variable
    $defaultWorkingDirectory = $env:SYSTEM_DEFAULTWORKINGDIRECTORY

    # Combine the base path with the relative path
    $searchPath = Join-Path -Path $defaultWorkingDirectory -ChildPath $relativePath

    Write-Host "Searching in directory: $searchPath"

    # Search for the specific file
    $files = Get-ChildItem -Path $searchPath -Recurse -File -Filter $fileName

    if ($files) {
        Write-Host "Policy file(s) found:"
        foreach ($file in $files) {
            Write-Host $file.FullName
        }
    } else {
        Write-Host "File '$fileName' not found in directory '$searchPath'."

        # Search for any XML files
        $xmlFiles = Get-ChildItem -Path $searchPath -Recurse -File -Filter "*.xml"
        if ($xmlFiles) {
            Write-Host "XML File(s) found:"
            foreach ($file in $xmlFiles) {
                Write-Host $file.FullName
            }
        } else {
            Write-Host "No XML files found in directory '$searchPath'."
        }
}
 

    #if (Test-Path -Path $ApimApiPolicyFilePath) {
    #    Write-Host "Set API policy"
        
	#$policy = Get-Content -Path $ApimApiPolicyFilePath -Raw
	#$policy = $policy -replace "{ApplicationIdentifierUri}", $ApplicationIdentifierUri
    #
	#Set-AzApiManagementPolicy -Context $ApimContext -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyContent $policy -ErrorAction Stop -Verbose:$VerbosePreference
    #} else {
    #    Write-Host "Please specify a valid policy file path"
    #}
    
} catch {
   throw $_
}