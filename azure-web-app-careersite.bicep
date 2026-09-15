targetScope = 'resourceGroup'

param webAppName string = 'ahrm-careersite'
@allowed([
  'F1'
  'B1'
])
param appServicePlanSkuName string = 'F1'
@allowed([
  'Free'
  'Basic'
])
param appServicePlanSkuTier string = 'Free'
param appServicePlanName string = '${webAppName}-plan'
param webAppLinuxFxVersion string = 'DOTNET|10.0'

param hrmServerHost string = 'https://your-ahrm-environment.azurewebsites.net/'
@allowed([
  'ApiKey'
  'OAuth'
  'None'
])
param hrmServerAuthenticationMode string = 'None'
param hrmServerAuthenticationApiKeyHeaderName string = 'X-Api-Key'
@secure()
param hrmServerAuthenticationApiKey string = ''
@secure()
param hrmServerAuthenticationOAuthTenantId string = ''
@secure()
param hrmServerAuthenticationOAuthClientId string = ''
@secure()
param hrmServerAuthenticationOAuthClientSecret string = ''

var deploymentLocation = resourceGroup().location

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: appServicePlanName
  location: deploymentLocation
  sku: {
    name: appServicePlanSkuName
    tier: appServicePlanSkuTier
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2024-11-01' = {
  name: webAppName
  location: deploymentLocation
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: webAppLinuxFxVersion
      ftpsState: 'Disabled'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'HrmServer__Host'
          value: hrmServerHost
        }
        {
          name: 'HrmServer__Authentication__Mode'
          value: hrmServerAuthenticationMode
        }
        {
          name: 'HrmServer__Authentication__ApiKeyHeaderName'
          value: hrmServerAuthenticationApiKeyHeaderName
        }
        {
          name: 'HrmServer__Authentication__ApiKey'
          value: hrmServerAuthenticationApiKey
        }
        {
          name: 'HrmServer__Authentication__OAuth__TenantId'
          value: hrmServerAuthenticationOAuthTenantId
        }
        {
          name: 'HrmServer__Authentication__OAuth__ClientId'
          value: hrmServerAuthenticationOAuthClientId
        }
        {
          name: 'HrmServer__Authentication__OAuth__ClientSecret'
          value: hrmServerAuthenticationOAuthClientSecret
        }
      ]
    }
  }
}

output webAppNameOutput string = webApp.name
output appServicePlanNameOutput string = appServicePlan.name
