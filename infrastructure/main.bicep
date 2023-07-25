
@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod'])
param environment string = 'lab'
@description('Part of the name that will be suffixed/prefixed')
param namepart string = 'carsample'


module pushpull 'modules/push-pull.bicep' = {
  name: 'push_pull'
  params: {
    location: location
    namepart: namepart
    environment: environment
  }
} 

module pushpush 'modules/push-push.bicep' = {
  name: 'custom_topic'
  params: {
    location: location
    environment: environment
  }
}
