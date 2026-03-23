az login

az group create --name AzureCli --location "West Europe"

az appservice plan create --name AzureCliPlan --resource-group AzureCli --location "West Europe" --sku F1 --is-linux

az webapp create --name AzureCliWebApp9862345463652 --resource-group AzureCli --plan AzureCliPlan --runtime "DOTNETCORE|8.0"