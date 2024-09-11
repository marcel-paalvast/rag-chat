metadata description = 'Create Azure Resources.'

targetScope = 'resourceGroup'

@export()
@description('Creates a unique name for Azure Resources.')
func createUniqueName(prefix string) string => '${prefix}${uniqueString(resourceGroup().id)}'

param location string = resourceGroup().location

@maxLength(32)
@description('The name used for the Function App.')
param functionAppPlanName string

@maxLength(9)
@description('The prefix for naming the Azure Function App.')
param functionAppPrefix string

@maxLength(9)
@description('The prefix for naming the Storage Account.')
param storageAccountPrefix string

@maxLength(32)
@description('The name used for the Application Insights.')
param appInsightsName string

@maxLength(32)
@description('The name used for the Workspace.')
param workspaceName string

@maxLength(32)
@description('The prefix for naming the Storage Account.')
param cosmosAccountPrefix string

@maxLength(16)
@description('The name used for the Key Vault.')
param keyVaultName string

@maxLength(32)
@description('The prefix for naming the Identity used by the Function App.')
param identityName string

var functionAppName = createUniqueName(functionAppPrefix)
var storageAccountName = createUniqueName(storageAccountPrefix)
var cosmosAccountName = createUniqueName(cosmosAccountPrefix)
var cosmosDatabaseName = 'ragChat' // used by api

@description('Contains tags that are deployed on each resource.')
var tags = {
  App: 'ragChat'
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
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    clientAffinityEnabled: false
    httpsOnly: true
    serverFarmId: functionsAppPlan.id
    keyVaultReferenceIdentity: identity.id
    siteConfig: {
      ftpsState: 'Disabled'
      http20Enabled: true
      keyVaultReferenceIdentity: identity.id
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
        {
          name: 'Cosmos:Uri'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'Cosmos:Key'
          value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${secretCosmosKey.name})'
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

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosAccountName
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    capabilities: [
      {
        name: 'EnableNoSQLVectorSearch'
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableBurstCapacity: true
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    minimalTlsVersion: 'Tls12'
    publicNetworkAccess: 'Enabled'
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  name: cosmosDatabaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: cosmosDatabaseName
    }
    options: {
      throughput: 400
    }
  }
}

// cosmos container is created through script due to limitations in creating vector policies through bicep

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
  tags: tags
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    enablePurgeProtection: null //null to disable
    softDeleteRetentionInDays: 90
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        objectId: identity.properties.principalId
        tenantId: identity.properties.tenantId
        permissions: {
          secrets: [ 
            'get' 
          ]
        }
      }
    ]
  }
}

resource keyVaultDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'service'
  scope: keyVault
  properties: {
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
      }
    ]
    workspaceId: workspace.id
  }
}

resource secretCosmosKey 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'CosmosKey'
  properties: {
    value: cosmosAccount.listKeys().primaryMasterKey
  }
}
