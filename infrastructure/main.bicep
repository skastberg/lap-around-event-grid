
@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod' 
])
param environment string

module pushpull 'modules/push-pull.bicep' = {
  name: 'push_pull'
  params: {
    location: location
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
