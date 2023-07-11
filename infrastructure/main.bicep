
@allowed(['westeurope' 
])
param location string = 'westeurope'


module pushpull 'modules/push-pull.bicep' = {
  name: 'push_pull'
  params: {
    location: location
  }
} 
