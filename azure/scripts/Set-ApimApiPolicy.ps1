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

    # Function to list XML files in a directory
    function List-XmlFiles {
    param (
        [string]$directoryPath
    )

    if (Test-Path -Path $directoryPath) {
        Write-Host "Searching in directory: $directoryPath"
        $xmlFiles = Get-ChildItem -Path $directoryPath -Recurse -File -Filter "*.xml"

        if ($xmlFiles) {
            Write-Host "XML file(s) found in $directoryPath:"
            foreach ($file in $xmlFiles) {
                Write-Host $file.FullName
            }
        } else {
            Write-Host "No XML files found in directory $directoryPath."
        }
    } else {
        Write-Host "Directory $directoryPath does not exist."
    }
}

# List of common artifact paths
$paths = @(
    $env:SYSTEM_DEFAULTWORKINGDIRECTORY,
    $env:BUILD_ARTIFACTSTAGINGDIRECTORY,
    $env:BUILD_BINARIESDIRECTORY,
    $env:AGENT_TEMPDIRECTORY
)

# Check each path
foreach ($path in $paths) {
    List-XmlFiles -directoryPath $path
}

    
} catch {
   throw $_
}