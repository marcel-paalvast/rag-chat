metadata description = 'Returns the endpoint of the Azure Functions app.' 
import { createUniqueName } from './main.bicep'

targetScope = 'resourceGroup'

@maxLength(9)
@description('The name of the Azure Functions app.')
param functionAppPrefix string

var functionAppName = createUniqueName(functionAppPrefix)

resource functionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

output endpoint string = functionApp.properties.defaultHostName
output name string = functionApp.name
