
@allowed(['westeurope' 
])
param location string = 'westeurope'

@allowed(['lab','dev','test','prod'])
param environment string = 'lab'
@description('Part of the name that will be suffixed/prefixed')
param namepart string

resource asev3 'Microsoft.Web/hostingEnvironments@2022-09-01' = {
  name: 'asev3-${namepart}-${environment}'
  location: location
  properties: {
    zoneRedundant: false
    
    virtualNetwork: {
      id: 
    }
  }
}
