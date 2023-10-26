@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod' 
])
param environment string = 'lab'
param prefix string 

resource egtopic 'Microsoft.EventGrid/topics@2023-06-01-preview' = {
  name: '${prefix}-${environment}'
  location: location
}
