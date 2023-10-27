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
            Console.WriteLine("Send EG schema");
            SendEventGridSchemaEvent(credential); 
            Console.WriteLine("Enter to continue ...");
            Console.ReadLine();
            // CloudEvents            
            Console.WriteLine("Send CE schema");
            SendCloudSchemaEvent(credential);
            Console.WriteLine("Send CE schema Native");
            SendCloudSchemaNativeEvent(credential);
            Console.WriteLine("Enter to continue ...");
            Console.ReadLine();
            // Domains
            Console.WriteLine("Send HR events to domain topic");
            SendEventGridSchemaHREvent(credential);


        }
        static void SendCloudSchemaEvent(DefaultAzureCredential credential)
        {
            // Set up the Event Grid client
            // Event Grid endpoint from the portal
            var endpoint = new Uri(configuration["CloudEventsEndpoint"]);
            var client = new EventGridPublisherClient(endpoint, credential);

            var data = new
            {
                doorId = 456,
                buildingName = "DobsTower",
                customerName = "Cloud Event",
                email = "johndoe@cloud.com"
            };
            string etype = data.doorId % 2 == 0 ? "contoso.building.door.open" : "contoso.building.door.close";
            var newEvent = new CloudEvent(
                    new Uri("building:DobsTower:door:456").ToString(),
                    etype,
                    new BinaryData(Newtonsoft.Json.JsonConvert.SerializeObject(data)),
                    "application/json");

            newEvent.Id = Guid.NewGuid().ToString();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newEvent);
            client.SendEvent(newEvent);
        }

        static void SendCloudSchemaNativeEvent(DefaultAzureCredential credential)
        {
            // Set up the Event Grid client
            // Event Grid endpoint from the portal
            var endpoint = new Uri(configuration["CloudEventsEndpoint"]);

            var client = new EventGridPublisherClient(endpoint, credential);

            var data = new
            {
                doorId = 789,
                buildingName = "DobsTower",
                customerName = "Cloud Event Native",
                email = "johndoe@cloud.com"
            };
            string etype = data.doorId % 2 == 0 ? "contoso.building.door.open" : "contoso.building.door.close";
            var newEvent = new CloudNative.CloudEvents.CloudEvent() { Type = etype, Source = new Uri("building:DobsTower:door:456"), DataContentType = "application/json", Data = data };
            newEvent.Id = Guid.NewGuid().ToString();
            CloudEventFormatter formatter = new JsonEventFormatter();
            var bytes = formatter.EncodeStructuredModeMessage(newEvent, out var contentType);
            string json = Encoding.UTF8.GetString(bytes.Span);
            client.SendCloudNativeCloudEvent(newEvent);



        }


        static void SendEventGridSchemaEvent(DefaultAzureCredential credential)
        {
            // Set up the Event Grid client
            var endpoint = new Uri(configuration["EventGridEndpoint"]); // Event Grid endpoint from the portal
            var client = new EventGridPublisherClient(endpoint, credential);
            List<EventGridEvent> list = new List<EventGridEvent>();
            for (int i = 0; i < 50; i++)
            {
                #region Create data for the event
                var data = new
                {
                    doorId = 123 + i,
                    buildingName = "DobsTower",
                    customerName = "John Doe",
                    email = "johndoe@example.com"
                };
                #endregion
                string etype = data.doorId % 2 == 0 ? "contoso.building.door.open" : "contoso.building.door.close";
                // Create the event
                EventGridEvent e = new EventGridEvent(
                    eventType: etype,
                    subject: new Uri($"building:DobsTower:door:{data.doorId}").ToString(),
                    dataVersion: "1.0",
                    data: new BinaryData(data)
                );
                //e.Topic = "line";
                // Add the current event to the list
                list.Add(e);
            }
            // Send the events batch to the event grid
            client.SendEvents(list);
        }

        static void SendEventGridSchemaHREvent(DefaultAzureCredential credential)
        {
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
                        freeDays = (123+i) % 7

                        
                    };
                    // Create the event
                    EventGridEvent e = new EventGridEvent(
                        eventType: etype,
                        subject: new Uri($"contoso.hr.employee.vacation.booked:{empId}").ToString(),
                        dataVersion: "1.0",
                        data: new BinaryData(data)
                    );
                    e.Topic = "humanresources";
                    // Add the current event to the list
                    list.Add(e);
                }

            }
            // Send the events batch to the event grid
            client.SendEvents(list);
        }


    }
}
