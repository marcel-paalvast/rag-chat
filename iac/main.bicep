metadata description = 'Create Azure Resources.' 

targetScope = 'resourceGroup'

@export()
@description('Creates a unique name for Azure Resources.')
func createUniqueName(prefix string) string => '${prefix}${uniqueString(resourceGroup().id)}'

param location string = resourceGroup().location

@maxLength(32)
@description('The name used for Function App.')
param functionAppPlanName string

@maxLength(9)
@description('The prefix for naming the Azure Function App.')
param functionAppPrefix string

@maxLength(9)
@description('The prefix for naming the Storage Account.')
param storageAccountPrefix string

@maxLength(32)
@description('The name used for Application Insights.')
param appInsightsName string

@maxLength(32)
@description('The name used for Workspace.')
param workspaceName string

var functionAppName = createUniqueName(functionAppPrefix)
var storageAccountName = createUniqueName(storageAccountPrefix)

@description('Contains tags that are deployed on each resource.')
var tags = {
  App: 'ragchat'
}

resource functionsAppPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: functionAppPlanName
  location: resourceGroup().location
  tags: tags
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: resourceGroup().location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource functionsApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: resourceGroup().location
  tags: tags
  kind: 'functionapp'
  properties: {
    clientAffinityEnabled: false
    httpsOnly: true
    serverFarmId: functionsAppPlan.id
    siteConfig: {
      ftpsState: 'Disabled'
      http20Enabled: true
      minTlsVersion: '1.2'
      netFrameworkVersion: 'v8.0'
      scmMinTlsVersion: '1.2'
      use32BitWorkerProcess: false
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'   
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
      ]
    }
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  properties: {
    workspaceCapping: {
      dailyQuotaGb: json('0.1')
    }
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: workspace.id
    DisableIpMasking: false
    RetentionInDays: 90
  }
}
