@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod' 
])
param environment string = 'lab'

resource egtopic 'Microsoft.EventGrid/topics@2022-06-15' = {
  name: 'eg-custom-topic-${environment}'
  location: location
}
