using 'main.bicep'
import { defaultPrefix } from './main.bicep'

param functionAppPrefix = defaultPrefix

param storageAccountPrefix =  toLower(defaultPrefix)

param appInsightsName = '${defaultPrefix}AppInsights'

param workspaceName = '${defaultPrefix}Workspace'

param functionAppPlanName = '${defaultPrefix}Plan'

param cosmosAccountPrefix = toLower('${defaultPrefix}CosmosDb')

param keyVaultName = '${defaultPrefix}KeyVault'

param identityName = '${defaultPrefix}Identity'

//the following params are declared during deployment
param openAiAssistant = ommited

param openAiEmbedding = ommited

param openAiKey = ommited

param openAiUri = ommited

var ommited = '<should be set during deployment>'
