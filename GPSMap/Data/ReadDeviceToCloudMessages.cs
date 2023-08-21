// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/samples/README.md
// For documentation see: https://docs.microsoft.com/azure/event-hubs/

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventHubs.Consumer;
using Newtonsoft.Json;
//using CommandLine;

namespace ReadD2cMessages
{
    public class Geolocation
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public double alt { get; set; }
  
    }

    public class Geopoint
    {
        public Geolocation location {get; set;}
    }
    /// <summary>
    /// A sample to illustrate reading Device-to-Cloud messages from a service app.
    /// </summary>
    public class GPSCls
    {
        static string EventHubConnectionString = "<Endpoint>";
        static string HubName = "<Hub Name>";
        static bool showProperties = true;
        static Func<double[], int>? MyMethodName = null;
        static double lat = 0;
        static double lon = 45;
        static DateTime Start = DateTime.Now;
        public static async Task Main(Func< double[], int> myMethodName)
        {
            MyMethodName = myMethodName;
            System.Diagnostics.Debug.WriteLine("IoT Hub  - Read device to cloud messages. Ctrl-C to exit.\n");
            System.Diagnostics.Debug.WriteLine(".NET 6.0 C# 9.0.\n");
            Start = DateTime.Now.ToUniversalTime();
            System.Diagnostics.Debug.WriteLine("Do you want to SHOW System and App  Properties sent by IoT Hub? [Y]es Default No");
            System.Diagnostics.Debug.WriteLine("");

            // Set up a way for the user to gracefully shutdown
            using var cts = new CancellationTokenSource();
            //System.Diagnostics.Debug.CancelKeyPress += (sender, eventArgs) =>
            //{
            //    eventArgs.Cancel = true;
            //    cts.Cancel();
            //    System.Diagnostics.Debug.WriteLine("Exiting...");
            //};

            // Run the sample
            await ReceiveMessagesFromDeviceAsync(cts.Token);

            System.Diagnostics.Debug.WriteLine("Cloud message reader finished.");
        }

        // Asynchronously create a PartitionReceiver for a partition and then start
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(/*Parameters parameters,*/ CancellationToken ct)
        {
            //string connectionString = parameters.GetEventHubConnectionString();

            // Create the consumer using the default consumer group using a direct connection to the service.
            // Information on using the client with a proxy can be found in the README for this quick start, here:
            // https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/main/iot-hub/Quickstarts/ReadD2cMessages/README.md#websocket-and-proxy-support
            await using var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                EventHubConnectionString,
                HubName);

            System.Diagnostics.Debug.WriteLine("Listening for messages on all partitions.");

            try
            {
                // Begin reading events for all partitions, starting with the first event in each partition and waiting indefinitely for
                // events to become available. Reading can be canceled by breaking out of the loop when an event is processed or by
                // signaling the cancellation token.
                //
                // The "ReadEventsAsync" method on the consumer is a good starting point for consuming events for prototypes
                // and samples. For real-world production scenarios, it is strongly recommended that you consider using the
                // "EventProcessorClient" from the "Azure.Messaging.EventHubs.Processor" package.
                //
                // More information on the "EventProcessorClient" and its benefits can be found here:
                //   https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs.Processor/README.md
                await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(ct))
                {

                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    if (data[data.Length - 1] != '}')
                        data += '}';
           
                    DateTime xx = (DateTime)partitionEvent.Data.SystemProperties["iothub-enqueuedtime"];
                    //System.Diagnostics.Debug.WriteLine("{0} {1}",Start,xx);
                    if (xx.Ticks < Start.Ticks)
                        continue;
                    System.Diagnostics.Debug.WriteLine($"\nMessage received on partition {partitionEvent.Partition.PartitionId}:");
                    System.Diagnostics.Debug.WriteLine($"\tMessage body: {data}");
                    
                    if ((MyMethodName != null) &&(!string.IsNullOrEmpty(data)))
                    {
                        Geolocation? _location = JsonConvert.DeserializeObject<Geolocation>(data);

                        if (_location != null)
                        {
                            Geolocation location = (Geolocation)_location;

                            double lat = location.lat;
                            double lon = location.lon;
                            double alt = location.alt;
  
                            double[] dbl = new double[] { lat, lon };
                            int res = ((Func<double[], int>)MyMethodName)(dbl);
                        }
                    }

                    if (showProperties)
                    {
                        System.Diagnostics.Debug.WriteLine("\tApplication properties (set by device):");
                        foreach (KeyValuePair<string, object> prop in partitionEvent.Data.Properties)
                        {
                            PrintProperties(prop);
                        }

                        System.Diagnostics.Debug.WriteLine("\tSystem properties (set by IoT hub):");
                        foreach (KeyValuePair<string, object> prop in partitionEvent.Data.SystemProperties)
                        {
                            PrintProperties(prop);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // This is expected when the token is signaled; it should not be considered an
                // error in this scenario.
            }
        }

        private static void PrintProperties(KeyValuePair<string, object> prop)
        {
            string propValue = prop.Value is DateTime time
                ? time.ToString("O") // using a built-in date format here that includes milliseconds
                : prop.Value.ToString();

            System.Diagnostics.Debug.WriteLine($"\t\t{prop.Key}: {propValue}");
        }
    }
}
