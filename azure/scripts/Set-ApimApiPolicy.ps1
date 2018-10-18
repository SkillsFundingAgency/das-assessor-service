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
    [String]$ApimApiPolicyFilePath
)

try {

    # --- Build context and retrieve apiid
    Write-Host "Building APIM context for $ResourceGroupName\$ServiceName"
    $ApimContext = New-AzureRmApiManagementContext -ResourceGroupName $ResourceGroupName -ServiceName $ServiceName

    # Ensure policy file exists
    Write-Host "Test that policy file exists"
    if (Test-Path -Path $ApimApiPolicyFilePath) {
        Write-Host "Set API policy"
        Set-AzureRmApiManagementPolicy -Context $ApimContext -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $($ApiId) -PolicyFilePath $($ApimApiPolicyFilePath) -ErrorAction Stop -Verbose:$VerbosePreference
    } else {
        Write-Host "Please specify a valid policy file path"
    }
    
} catch {
   throw $_
}