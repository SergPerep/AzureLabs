# Creates App Service

# Connect-AzAccount -UseDeviceAuthentication

$Location="West Europe"
$ResourceGroupName="PowerShellStart"
$AppServicePlanName="appplan715234"
$AppName="pswebapi715234soma"

# Create Resource Group
New-AzResourceGroup -Name $ResourceGroupName -Location $Location

# Create App Service Plan
New-AzAppServicePlan -Name $AppServicePlanName -ResourceGroupName $ResourceGroupName -Location $Location -Tier "Free" -Linux

# Create Web App
New-AzWebApp -Name $AppName `
    -ResourceGroupName $ResourceGroupName `
    -Location $Location `
    -AppServicePlan $AppServicePlanName
