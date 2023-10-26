
@allowed(['westeurope' 
])
param location string = 'westeurope'

@allowed(['lab','dev','test','prod'])
param environment string = 'lab'
@description('Part of the name that will be suffixed/prefixed')
param namepart string


resource eventgridnamespace 'Microsoft.EventGrid/namespaces@2023-06-01-preview' = {
  name: 'egns-${namepart}-${environment}'
  location: location
  properties: {
    // Needs to be true for the template to work in West Europe (Preview)
    isZoneRedundant: true 
  }
}

resource egtopic 'Microsoft.EventGrid/namespaces/topics@2023-06-01-preview' = {
  name: 'workitems-${environment}'
  parent: eventgridnamespace
  properties: {
    eventRetentionInDays: 1
    inputSchema: 'CloudEventSchemaV1_0'
    publisherType: 'Custom'
  }
}


resource paintSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2023-06-01-preview' = {
  name: 'paintEvents'
  parent: egtopic
  properties: {
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filtersConfiguration: {
      includedEventTypes: [
        'workitems.paint'
      ]
    }
    deliveryConfiguration: {
      deliveryMode: 'Queue'
    }
  }
}

resource allSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2023-06-01-preview' = {
  name: 'allEvents'
  parent: egtopic
  properties: {
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filtersConfiguration: { }
    deliveryConfiguration: {
      deliveryMode: 'Queue'
    }
  }
}


output EventGrid_Namespace string = eventgridnamespace.name
output Topic_name string = egtopic.name
