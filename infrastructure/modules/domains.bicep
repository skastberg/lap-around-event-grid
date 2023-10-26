
@allowed(['westeurope' 
])
param location string = 'westeurope'

@allowed(['lab','dev','test','prod'])
param environment string = 'lab'
@description('Part of the name that will be suffixed/prefixed')
param namepart string


resource eventGridDomain 'Microsoft.EventGrid/domains@2023-06-01-preview' = {
  name: 'egd-${namepart}-${environment}'
  location: location
  properties: {
    inputSchema: 'EventGridSchema'
    publicNetworkAccess: 'Enabled'
    
  }
}

resource hrTopic 'Microsoft.EventGrid/domains/topics@2023-06-01-preview' = {
  name: 'humanresources'
  parent: eventGridDomain
}

resource hrsub 'Microsoft.EventGrid/domains/topics/eventSubscriptions@2023-06-01-preview' = {
  name: 'hrsub'
  parent: hrTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: 'https://${namepart}-viewer.azurewebsites.net/api/updates'
      }
    }
    filter: {
      includedEventTypes: null
    }
  }
}


resource financeTopic 'Microsoft.EventGrid/domains/topics@2023-06-01-preview' = {
  name: 'finance'
  parent: eventGridDomain
 
}

resource buildingsTopic 'Microsoft.EventGrid/domains/topics@2023-06-01-preview' = {
  name: 'buildings'
  parent: eventGridDomain
 
}
