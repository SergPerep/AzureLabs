$azContext = Connect-AzAccount

$Location="North Europe"
$ResourceGroupName="PowerShellStart1"

New-AzResourceGroup -Name $ResourceGroupName -Location $Location

$AppServicePlanName="appplan715234"

New-AzAppServicePlan -Name $AppServicePlanName -ResourceGroupName $ResourceGroupName -Location $Location -Tier "Free" -DefaultProfile $azContext


$AppName="pswebapi715234"

New-AzWebApp -Name $AppName `
    -ResourceGroupName $ResourceGroupName `
    -Location $Location `
    -AppServicePlan $AppServicePlanName
