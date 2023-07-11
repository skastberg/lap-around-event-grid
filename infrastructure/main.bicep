
@allowed(['westeurope' 
])
param location string = 'westeurope'


module pushpull 'modules/push-pull.bicep' = {
  name: 'pushpull'
  params: {
    location: location
  }
} 
