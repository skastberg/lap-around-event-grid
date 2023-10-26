@allowed(['westeurope' 
])
param location string = 'westeurope'
@allowed(['lab','dev','test','prod' 
])
param environment string = 'lab'
param prefix string 
@allowed(['EventGridSchema','CloudEventSchemaV1_0'])
param inputSchema string = 'EventGridSchema'

resource egtopic 'Microsoft.EventGrid/topics@2023-06-01-preview' = {
  name: '${prefix}-${environment}'
  location: location
  properties: {
    inputSchema: inputSchema
  }
}
