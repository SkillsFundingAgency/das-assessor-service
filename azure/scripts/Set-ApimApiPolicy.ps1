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
    Write-Host "$ApimApiPolicyFilePath =  $ApimApiPolicyFilePath"

   $paths = @(
    $env:SYSTEM_DEFAULTWORKINGDIRECTORY,
    $env:BUILD_ARTIFACTSTAGINGDIRECTORY,
    $env:BUILD_BINARIESDIRECTORY,
    $env:AGENT_TEMPDIRECTORY,
    $env:AGENT_BUILDDIRECTORY,
    $env:PIPELINE_WORKSPACE

    )

    ## Check each path
    foreach ($path in $paths) {

        Write-Host "Searching path : $path"    
    
        $xmlFiles = Get-ChildItem -Path $path -Recurse -File -Filter "das-assessor-service-api-external.xml"

        if($xmlFiles){
            foreach ($file in $xmlFiles) {
                Write-Host $file.FullName
            }
        }
    }

} catch {
   throw $_
}