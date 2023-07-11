
@allowed(['westeurope' 
])
param location string = 'westeurope'


resource eventgridnamespace 'Microsoft.EventGrid/namespaces@2023-06-01-preview' = {
  name: 'egns'
  location: location
  properties: {
    isZoneRedundant: false
    minimumTlsVersionAllowed: '1.2'
  }
}

resource egtopic 'Microsoft.EventGrid/namespaces/topics@2023-06-01-preview' = {
  name: 'strokeEvents'
  parent: eventgridnamespace
  properties: {
    eventRetentionInDays: 3
    inputSchema: 'CloudEventSchemaV1_0'
    publisherType: 'Custom'
  }
}


resource strokeSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2023-06-01-preview' = {
  name: 'fromTeeEvents'
  parent: egtopic
  properties: {
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filtersConfiguration: {
      includedEventTypes: [
        'Stroke.FromTee'
      ]
    }
  }

}