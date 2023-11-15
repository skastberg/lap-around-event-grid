using Azure.Messaging.EventGrid;
using Azure.Identity;
using System;
using Azure.Messaging;
using Microsoft.Azure.Messaging.EventGrid.CloudNativeCloudEvents;
using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents;
using CloudEvent = Azure.Messaging.CloudEvent;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Text;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Event_Sender
{
    internal class Program
    {
        private static IConfiguration configuration;
        static void Main(string[] args)
        {
            // Read from configuration file
            #region Load configuration

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("local.appsettings.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Register your services here if needed
                });

            var host = builder.Build();
            configuration = host.Services.GetRequiredService<IConfiguration>();
            #endregion

            DefaultAzureCredentialOptions defaultAzureCredentialOptions = new DefaultAzureCredentialOptions();
            defaultAzureCredentialOptions.VisualStudioTenantId = configuration["VisualStudioTenantId"];

            // Set up Azure credentials
            var credential = new DefaultAzureCredential(options: defaultAzureCredentialOptions);

            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Send EG schema");
                Console.WriteLine("2. Send CE schema");
                Console.WriteLine("3. Send CE schema Native");
                Console.WriteLine("4. Send HR events to domain topic");
                Console.WriteLine("5. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SendEventGridSchemaEvent(credential);
                        break;
                    case "2":
                        SendCloudSchemaEvent(credential);
                        break;
                    case "3":
                        SendCloudSchemaNativeEvent(credential);
                        break;
                    case "4":
                        SendEventGridSchemaHREvent(credential);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                Console.WriteLine("Enter to continue ...");
                Console.ReadLine();
            }


        }
        static void SendCloudSchemaEvent(DefaultAzureCredential credential)
        {

            // Using Azure's CloudEvent class to create and send a single event compliant with the CloudEvent schema.
            // Sets eventType, source, and payload before sending it through Azure's Event Grid SDK.
            // Check out the SDK here: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventgrid/Azure.Messaging.EventGrid

            // Set up the Event Grid client
            // Event Grid endpoint from the portal
            var endpoint = new Uri(configuration["CloudEventsEndpoint"]);
            var client = new EventGridPublisherClient(endpoint, credential);

            // Create the event data with random values
            var data = new DoorOpenEventData();
            // Get the event type based on the door id
            string etype = GetEventType(data.DoorId);
            // Create the event using CloudEvent schema 
            var newEvent = new CloudEvent(
                    new Uri($"building:{data.BuildingName.ToLower()}:door:{data.DoorId}").ToString(),
                    etype,
                    new BinaryData(Newtonsoft.Json.JsonConvert.SerializeObject(data)),
                    "application/json");

            newEvent.Id = Guid.NewGuid().ToString();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newEvent);

            Console.WriteLine($"Sending event for building {data.BuildingName} door {data.DoorId} to Event Grid");
            client.SendEvent(newEvent);
        }



        static void SendCloudSchemaNativeEvent(DefaultAzureCredential credential)
        {
            /******************************************************************************
            Using the `CloudNative.CloudEvents.CloudEvent` here to create and send a cloudnative event to Azure's Event Grid. 
            This makes it compliant with the CNCF CloudEvents spec. 
            Here's how to find this component on GitHub for more details: CloudNative.CloudEvents GitHub Repo https://github.com/cloudevents/sdk-csharp.
            /******************************************************************************/

            // Set up the Event Grid client
            // Event Grid endpoint from the portal
            var endpoint = new Uri(configuration["CloudEventsEndpoint"]);
        var client = new EventGridPublisherClient(endpoint, credential);

        // Create the event data with random values
            var data = new DoorOpenEventData();
            // Get the event type based on the door id
            string etype = GetEventType(data.DoorId);
             
            var newEvent = new CloudNative.CloudEvents.CloudEvent() { Type = etype, Source = new Uri($"building:{data.BuildingName.ToLower()}:door:{data.DoorId}"), DataContentType = "application/json", Data = data };
            newEvent.Id = Guid.NewGuid().ToString();
            CloudEventFormatter formatter = new JsonEventFormatter();
            var bytes = formatter.EncodeStructuredModeMessage(newEvent, out var contentType);
            string json = Encoding.UTF8.GetString(bytes.Span);
            Console.WriteLine($"Sending event for building {data.BuildingName} door {data.DoorId} to Event Grid");
            client.SendCloudNativeCloudEvent(newEvent);



        }


        static void SendEventGridSchemaEvent(DefaultAzureCredential credential)
        {
            /******************************************************************************
            Using Azure's EventGridEvent from the Azure Event Grid SDK to create and batch-send events.
            Sending the events in a batch for efficiency. Check out the SDK on GitHub: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventgrid/Azure.Messaging.EventGrid
            ******************************************************************************/

            // Set up the Event Grid client
            var endpoint = new Uri(configuration["EventGridEndpoint"]); // Event Grid endpoint from the portal
            var client = new EventGridPublisherClient(endpoint, credential);
            List<EventGridEvent> list = new List<EventGridEvent>();
            for (int i = 0; i < 50; i++)
            {
                #region Create data for the event
                var data = new DoorOpenEventData();
                #endregion
                string etype = GetEventType(data.DoorId);
                // Create the event
                EventGridEvent e = new EventGridEvent(
                    eventType: etype,
                    subject: new Uri($"building:{data.BuildingName.ToLower()}:door:{data.DoorId}").ToString(),
                    dataVersion: "1.0",
                    data: new BinaryData(data)
                );
                Console.WriteLine($"Adding event for building {data.BuildingName} door {data.DoorId} to batch");
                // Add the current event to the list
                list.Add(e);
            }
            // Send the events batch to the event grid
            Console.WriteLine("Sending batch of events to Event Grid");
            client.SendEvents(list);
        }

        static void SendEventGridSchemaHREvent(DefaultAzureCredential credential)
        {
            /******************************************************************************
             * Sending events to a domain topic
             * Using Azure's EventGridEvent from the Azure Event Grid SDK to create and batch-send events.
             * The SDK is available on GitHub:  https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventgrid/Azure.Messaging.EventGrid
             * 
             */
            // Set up the Event Grid client
            var endpoint = new Uri(configuration["DomainEndpoint"]); // Event Grid endpoint from the portal
            var client = new EventGridPublisherClient(endpoint, credential);
            List<EventGridEvent> list = new List<EventGridEvent>();
            for (int i = 0; i < 50; i++)
            {
                var empId = 123 + i;
                string etype = empId % 2 == 0 ? "contoso.hr.payroll.payment" : "contoso.hr.employee.vacation.booked";

                if (etype == "contoso.hr.payroll.payment")
                {
                    #region Create data for the event
                    var data = new
                    {
                        employeeId = empId,
                        employeeName = "John doe {123+i}",
                        email = $"johndoe.{empId}@example.com",
                        amount = Random.Shared.Next(10548, 95203)
                    };
                    #endregion
                    // Create the event
                    EventGridEvent e = new EventGridEvent(
                        eventType: etype,
                        subject: new Uri($"hr:payroll:payment:{empId}").ToString(),
                        dataVersion: "1.0",
                        data: new BinaryData(data)
                    );
                    Console.WriteLine($"Adding payroll payment event for employee {empId} to batch");
                    e.Topic = "humanresources";
                    // Add the current event to the list
                    list.Add(e);

                }
                else
                {
                    // create data for vacation booked event
                    var data = new
                    {
                        employeeId = empId,
                        startDate = DateTime.UtcNow.AddDays(i),
                        freeDays = (123 + i) % 7


                    };
                    // Create the event
                    EventGridEvent e = new EventGridEvent(
                        eventType: etype,
                        subject: new Uri($"contoso.hr.employee.vacation.booked:{empId}").ToString(),
                        dataVersion: "1.0",
                        data: new BinaryData(data)
                    );
                    e.Topic = "humanresources";
                    Console.WriteLine($"Adding vacation booked event for employee {empId} to batch");
                    // Add the current event to the list
                    list.Add(e);
                }

            }
            Console.WriteLine("Sending batch of events to Event Grid");
            // Send the events batch to the event grid
            client.SendEvents(list);
        }

        /// <summary>
        /// Returns the event type based on the door id, just to fake some data.
        /// </summary>
        /// <param name="doorid"></param>
        /// <returns>string</returns>
        private static string GetEventType(int doorid)
        {
            return doorid % 2 == 0 ? "contoso.building.door.open" : "contoso.building.door.close";
        }


    }
}
