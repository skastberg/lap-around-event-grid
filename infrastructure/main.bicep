
@allowed(['westeurope' 
])
param location string = 'westeurope'


module pushpull 'modules/push-pull.bicep' = {
  name: 'push pull'
  params: {
    location: location
  }
} 
