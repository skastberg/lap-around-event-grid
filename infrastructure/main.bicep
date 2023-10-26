
@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod'])
param environment string = 'lab'
@description('Part of the name that will be suffixed/prefixed')
param namepart string = 'swetugg'


module pushpull 'modules/push-pull.bicep' = {
  name: '${namepart}_push_pull'
  params: {
    location: location
    namepart: namepart
    environment: environment
  }
} 

module pushpush 'modules/push-push.bicep' = {
  name: '${namepart}_custom_topic'
  params: {
    location: location
    environment: environment
    prefix: '${namepart}-custom-topic'
  }
}


module pushpusheg 'modules/push-push.bicep' = {
  name: '${namepart}_swetuggevents-eg'
  params: {
    location: location
    environment: environment
    prefix: '${namepart}events-eg'
  }
}

module pushpushce 'modules/push-push.bicep' = {
  name: '${namepart}_swetuggevents-ce'
  params: {
    location: location
    environment: environment
    prefix: '${namepart}events-ce'
    inputSchema: 'CloudEventSchemaV1_0'
  }
}
