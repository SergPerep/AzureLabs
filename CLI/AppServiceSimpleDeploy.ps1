# Creates App Service

# Connect-AzAccount -UseDeviceAuthentication

$Location="West Europe"
$ResourceGroupName="PowerShellStart"
$AppServicePlanName="appplan715234"
$AppName="pswebapi715234soma"

# Create Resource Group
New-AzResourceGroup -Name $ResourceGroupName -Location $Location

# Create App Service Plan
New-AzAppServicePlan -Name $AppServicePlanName `
    -ResourceGroupName $ResourceGroupName `
    -Location $Location `
    -Tier "Free"

# Create Web App
New-AzWebApp -Name $AppName `
    -ResourceGroupName $ResourceGroupName `
    -Location $Location `
    -AppServicePlan $AppServicePlanName

# Deploy web app from gitgub
$RepoUrl = "https://github.com/SergPerep/foods"
$PropertiesObject = @{
    repoUrl = "$RepoUrl"
    branch = "main"
    isManualIntegration = "true"
}
Set-AzResource -Properties $PropertiesObject `
    -ResourceGroupName $ResourceGroupName `
    -ResourceType Microsoft.Web/sites/sourcecontrols `
    -ResourceName $AppName/web `
    -ApiVersion 2022-03-01 `
    -Force