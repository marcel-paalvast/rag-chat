using 'main.bicep'

var defaultPrefix = 'ragchat'

param functionAppPrefix = defaultPrefix

param storageAccountPrefix =  defaultPrefix

param appInsightsName = '${defaultPrefix}AppInsights'

param workspaceName = '${defaultPrefix}Workspace'

param functionAppPlanName = '${defaultPrefix}Plan'
