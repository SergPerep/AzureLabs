az login

az group create --name AzureCli --location "West Europe"

az vm create --resource-group AzureCli --name AzureCliVM --image Ubuntu2404 --admin-username azureuser --admin-password "Azure12345678" --location "West Europe"