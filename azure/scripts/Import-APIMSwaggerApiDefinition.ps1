<#
.SYNOPSIS
Update an existing APIM API with a swagger definition

.PARAMETER ResourceGroupName
The name of the resource group that contains the APIM instance

.PARAMETER InstanceName
The name of the APIM instnace

.PARAMETER ApiId
The ApiId of the API to update. This will be the API Name if it was created using ARM templates but with hyphens between each word e.g. my-api-id

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
    [String]$ServiceName,
    [Parameter(Mandatory=$true)]
    [String]$ApiId,    
    [Parameter(Mandatory=$true)]
    [String]$SwaggerSpecificationUrl,
    [Parameter(Mandatory=$false)]
    [String]$ApiUrlSuffix
)

try {
    # --- Build context and retrieve apiid
    Write-Host "Building APIM context for $ResourceGroupName\$ServiceName"
    $Context = New-AzureRmApiManagementContext -ResourceGroupName $ResourceGroupName -ServiceName $ServiceName

    if ($PSBoundParameters.ContainsKey("ApiUrlSuffix")) {
        $ApiSuf = $ApiUrlSuffix.ToLower()
    } else {
        $ApiSuf = $ApiName.Replace(' ','-').ToLower()
    }    

    # --- Import swagger definition
    Write-Host "Updating API $ApiId\$ServiceName from definition $SwaggerSpecficiationUrl"
    Import-AzureRmApiManagementApi -Context $Context -SpecificationFormat "Swagger" -SpecificationUrl $($SwaggerSpecificationUrl) -ApiId $($ApiId) -Path $($ApiSuf) -ErrorAction Stop -Verbose:$VerbosePreference
} catch {
   throw $_
}
