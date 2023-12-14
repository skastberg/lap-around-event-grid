# Instructions

This sample is a console application that sends events to Event Grid. It can be used to test the Event Grid integration in the Azure Portal.
It can be used to send events to Event Grid, Event Grid Domains and Cloud Events.
The sample expects a local.appsettings.json file to be present in the same folder as the executable. The file should contain the settings as described below.
Infrastructure is expected to be present in Azure. The sample will not create any resources, but you can find IaC and GitHub action in the Repository.


1. Login to Azure Portal
2. Create an EventGrid Topic for CloudEvents schema

> Description [here](https://learn.microsoft.com/en-us/azure/event-grid/create-custom-topic) on how to do that.

3. Create an EventGrid Topic for Event Grid schema
4. Give your account `EventGrid Data Sender` permissions on the new topics. 
5. Configure a `local.appsettings.json` file as described below

**Settings**

| Key                   | Description                                                                     | Value                                                                      |
|-----------------------|---------------------------------------------------------------------------------|----------------------------------------------------------------------------|
| CloudEventsEndpoint   | Endpoint value from Azure portal for Cloud Events.                              | https://egrid1.westeurope-1.eventgrid.azure.net/api/events                 |
| EventGridEndpoint     | Endpoint value from Azure portal for Event Grid.                                | https://egrid2.westeurope-1.eventgrid.azure.net/api/events                 |
| DomainEndpoint | Endpoint value from Azure portal for Event Grid Domain.                         | https://egrid3.westeurope-1.eventgrid.azure.net/api/events                 |
| VisualStudioTenantId  | ID of the tenant to use when running from Visual Studio. Only needed to ensure the right tenant is chosen when having multiple.        | DA81714D-081F-49C9-B277-48A635044FF0                                        |

> Note: The table and sample file contains fake values.

**Sample local config file**

```json
{
  "CloudEventsEndpoint": "https://egrid1.westeurope-1.eventgrid.azure.net/api/events",
  "EventGridEndpoint": "https://egrid2.westeurope-1.eventgrid.azure.net/api/events",
  "DomainEndpoint": "https://egrid3.westeurope-1.eventgrid.azure.net/api/events",
  "VisualStudioTenantId": "DA81714D-081F-49C9-B277-48A635044FF0",
}
```

6. To ensure you have a current token you can use the following command:
```ps
az login --scope https://eventgrid.azure.net/.default
```
7. To use the updated token, reopen Visual Studio if VS was open before login.
8. Create subscriptions for the events.
9. Execute the application
