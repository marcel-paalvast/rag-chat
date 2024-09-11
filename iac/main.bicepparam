using 'main.bicep'

var defaultPrefix = 'ragChat'

param functionAppPrefix = defaultPrefix

param storageAccountPrefix =  toLower(defaultPrefix)

param appInsightsName = '${defaultPrefix}AppInsights'

param workspaceName = '${defaultPrefix}Workspace'

param functionAppPlanName = '${defaultPrefix}Plan'

param cosmosAccountPrefix = toLower('${defaultPrefix}CosmosDb')

param keyVaultName = '${defaultPrefix}KeyVault'

param identityName = '${defaultPrefix}Identity'
