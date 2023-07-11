@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod' 
])
param environment string = 'lab'

resource egtopic 'Microsoft.EventGrid/topics@2023-06-01-preview' = {
  name: 'eg-custom-topic-${environment}'
  location: location
}
