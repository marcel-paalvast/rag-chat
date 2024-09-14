metadata description = 'Returns the endpoint of the Azure Functions app.' 
import { createUniqueName } from './main.bicep'

targetScope = 'resourceGroup'

@maxLength(9)
@description('The name of the Azure Functions app.')
param functionAppPrefix string

@maxLength(9)
@description('The prefix for naming the Storage Account.')
param storageAccountPrefix string

var functionAppName = createUniqueName(functionAppPrefix)
var storageAccountName = createUniqueName(storageAccountPrefix)

resource functionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

output endpoint string = functionApp.properties.defaultHostName
output name string = functionApp.name
output storageAccountName string = storageAccount.name
