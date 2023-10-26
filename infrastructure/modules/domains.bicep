
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

resource financeTopic 'Microsoft.EventGrid/domains/topics@2023-06-01-preview' = {
  name: 'finance'
  parent: eventGridDomain
 
}

resource buildingsTopic 'Microsoft.EventGrid/domains/topics@2023-06-01-preview' = {
  name: 'buildings'
  parent: eventGridDomain
 
}
