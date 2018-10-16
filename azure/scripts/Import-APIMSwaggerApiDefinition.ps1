<#
.SYNOPSIS
Update an APIM API with a swagger definition

.PARAMETER ResourceGroupName
The name of the resource group that contains the APIM instance

.PARAMETER InstanceName
The name of the APIM instnace

.PARAMETER ApiName
The name of the API to update

.PARAMETER SwaggerSpecificationUrl
The full path to the swagger defintion

For example:
https://sit-manage-vacancy.apprenticeships.sfa.bis.gov.uk/swagger/docs/v1

.PARAMETER ApiUrlSuffix
Suffix to apply to APIs, comes after the name of the APIM and before the path of the API

Will default to the ApiName if not provided

#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [String]$ResourceGroupName,
    [Parameter(Mandatory=$true)]
    [String]$InstanceName,
    [Parameter(Mandatory=$true)]
    [String]$ApiName,
    [Parameter(Mandatory=$true)]
    [String]$SwaggerSpecificationUrl,
    [Parameter(Mandatory=$false)]
    [String]$ApiUrlSuffix
)

try {
    # --- Build context and retrieve apiid
    Write-Host "Building APIM context for $ResourceGroupName\$InstanceName"
    $Context = New-AzureRmApiManagementContext -ResourceGroupName $ResourceGroupName -ServiceName $InstanceName
    Write-Host "Retrieving ApiId for API $ApiName"
    $ApiId = (Get-AzureRmApiManagementApi -Context $Context -Name $ApiName).ApiId

    if ($PSBoundParameters.ContainsKey("ApiUrlSuffix")) {
        $ApiSuf = $ApiUrlSuffix.ToLower()
    } else {
        $ApiSuf = $ApiName.Replace(' ','-').ToLower()
    }    

    if (!$ApiId) {
        # create API if it doesn't exist
        Write-Host "Could not retrieve ApiId for API $ApiName - creating API"

        $ApimUri = [System.Uri] $SwaggerSpecificationUrl

        Write-Host "Creating API $ApiName with suffix $ApiSuf"
        $NewApi = New-AzureRmApiManagementApi -Context $Context -Name $ApiName -Description $ApiName -ServiceUrl "https://$($ApimUri.Host)" -Protocols @("https") -Path $ApiSuf -ErrorAction Stop -Verbose:$VerbosePreference
        $ApiId = $NewApi.ApiId
    }

    # --- Import swagger definition
    Write-Host "Updating API $ApiId\$InstanceName from definition $SwaggerSpecficiationUrl"
    Import-AzureRmApiManagementApi -Context $Context -SpecificationFormat "Swagger" -SpecificationUrl $SwaggerSpecificationUrl -ApiId $ApiId -Path $ApiSuf -ErrorAction Stop -Verbose:$VerbosePreference
} catch {
   throw $_
}
