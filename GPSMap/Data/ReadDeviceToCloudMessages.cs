﻿// Copyright (c) Microsoft. All rights reserved.
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
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Amqp.Framing;
using Azure.Core;
//using CommandLine;

namespace ReadD2cMessages
{
    public enum AppMode { none, live, from, fromto, running }
    public enum DataState {  none, loaded }

    public enum AppState { none, loading, running, playing, tunningpaused, playingpaused}

    public class AppInfo
    {
        public AppInfo()
        {
            StartTime = DateTime.Now;
        }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public AppMode appMode { get; set; } = AppMode.none;
        public DataState dataState { get; set; } = DataState.none;
        public AppState appState { get; set; } = AppState.none;

        public void Set(AppMode appmode) {appMode =appmode; }
        public void Set(DataState datastate) { 
            dataState = datastate;
            if (dataState == DataState.loaded)
            {
                if (appState == AppState.loading)
                    if (appMode == AppMode.live)
                        appState = AppState.running;
            }
        }
        public void Set(AppState appstate) { appState = appstate; }

        public void Set(AppMode appmode, DataState datastate) 
        { 
            appMode = appmode;
            dataState = datastate;
            if (dataState == DataState.loaded)
            {
                if (appState == AppState.loading)
                    if (appMode == AppMode.live)
                        appState = AppState.running;
            }
        }

        public void Set(AppMode appmode,  AppState appstate)
        {
            appMode = appmode;
            appState = appstate;
            if (dataState == DataState.loaded)
            {
                if (appState == AppState.loading)
                    if (appMode == AppMode.live)
                        appState = AppState.running;
            }
        }

        public void Set( DataState datastate, AppState appstate)
        {
            dataState = datastate;
            appState = appstate;
            if (dataState== DataState.loaded)
            {
                if (appState == AppState.loading)
                    if (appMode == AppMode.live)
                        appState = AppState.running;
             }
          
        }
        public void Set(AppMode appmode, DataState datastate, AppState appstate)
        {
            appMode = appmode;
            dataState = datastate;
            appState = appstate;
            if (dataState == DataState.loaded)
            {
                if (appState == AppState.loading)
                    if (appMode == AppMode.live)
                        appState = AppState.running;
            }
        }

    }

    public class Telemetry: Geolocation
    {
        public DateTime TimeStamp { get; set; }

        public Telemetry() { }
        public Telemetry(DateTime stamp) { TimeStamp = stamp; }
        public Telemetry(Geolocation location, DateTime stamp) 
        { 
            TimeStamp = stamp;
            lat = location.lat;
            lon = location.lon;
            alt = location.alt;
        }
    }
    public class Geolocation
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public double alt { get; set; }

    }

    /// <summary>
    /// A sample to illustrate reading Device-to-Cloud messages from a service app.
    /// </summary>
    public class GPSCls
    {
        public static AppInfo appInfo { get; set; } = new AppInfo();
        
        public static DateTime startTime = DateTime.Now;
        public static DateTime endTime = DateTime.Now;
        public static bool HideLoading { get; set; } = true;
        public static List<Telemetry> telemetrys { get; set;} = new List<Telemetry>();

        private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        static bool showProperties = true;
        static Func<Telemetry, int>? MyMethodName = null;
        static double lat = 0;
        static double lon = 45;
        static DateTime StartUniversal = DateTime.Now.ToUniversalTime();
        static DateTime EndUniversal = DateTime.Now.ToUniversalTime();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static CancellationTokenSource cts;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static CancellationToken token;

        public static async Task Delay(int del)
        {
            await Task.Delay(del);
        }

        public static async Task Playy(bool playMode)
        {
            if (appInfo.dataState == DataState.loaded)
            {
                if (playMode)
                {
                    if ((MyMethodName != null) && (telemetrys.Count() != 0))
                    {
                        appInfo.appState = AppState.playing;
                        int count = telemetrys.Count();
                        for (int i = 0; i < count; i++)
                        {
                            if (appInfo.appState != AppState.playing)
                                break;
                            int res = ((Func<Telemetry, int>)MyMethodName)(telemetrys[i]);
                            if (i < (count - 1))
                            {
                                TimeSpan span = telemetrys[i + 1].TimeStamp.Subtract(
                                    telemetrys[i].TimeStamp);

                                await Delay(span.Milliseconds);
                            }
                        }
                        appInfo.Set(AppState.none);
                        HideLoading = true;
                    }
                    else
                    {

                    }
                }
                else
                {
                    //Use this to break the loop
                    appInfo.Set(AppState.none);
                }
            }
            else
            {
                appInfo.Set(AppState.none);
            }
        }

        public static void StopMonitor()
        {
            cts.Cancel();
        }
        public static async Task StartMonitor(Func< Telemetry, int> myMethodName)
        {
            appInfo.Set(DataState.none, AppState.loading);
            telemetrys = new List<Telemetry>();
            MyMethodName = myMethodName;
            System.Diagnostics.Debug.WriteLine("IoT Hub  - Read device to cloud messages. Ctrl-C to exit.\n");
            System.Diagnostics.Debug.WriteLine(".NET 6.0 C# 9.0.\n");
            StartUniversal = ((DateTime)startTime).ToUniversalTime();
            EndUniversal = ((DateTime)endTime).ToUniversalTime();
            System.Diagnostics.Debug.WriteLine("Do you want to SHOW System and App  Properties sent by IoT Hub? [Y]es Default No");
            System.Diagnostics.Debug.WriteLine("");

            // Set up a way for the user to gracefully shutdown
            if (appInfo.appMode == AppMode.live)
                cts = new CancellationTokenSource();
            else
            {
                int timeout = config.GetValue<int>("TIMEOUT");
                cts = new CancellationTokenSource(new TimeSpan(0, 0, timeout));
            }
            token = cts.Token;

            // Run the sample
            await ReceiveMessagesFromDeviceAsync(token);

            System.Diagnostics.Debug.WriteLine("Cloud message reader finished.");
        }

        // Asynchronously create a PartitionReceiver for a partition and then start
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(/*Parameters parameters,*/ CancellationToken ct)
        {
            string EventHubConnectionString = config.GetValue<string>("EventHubConnectionString");
            string HubName = config.GetValue<string>("HubName");

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
                    //if (ct.IsCancellationRequested)
                    //{
                    //    if (appInfo.appMode == AppMode.live)
                    //    {
                    //        // Just update is loading indication
                    //        HideLoading = false;
                    //        Telemetry telem = new Telemetry { lat = 0, lon = 0, alt = 0 };
                    //        int res = ((Func<Telemetry, int>)MyMethodName)(telem);
                    //    }
                    //    else
                    //        ct.ThrowIfCancellationRequested();
                    //}
                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    if (data[data.Length - 1] != '}')
                        data += '}';
           
                    DateTime xx = (DateTime)partitionEvent.Data.SystemProperties["iothub-enqueuedtime"];
                    System.Diagnostics.Debug.WriteLine("{0} {1} {2}",StartUniversal,xx, EndUniversal);
                    if (xx.Ticks < StartUniversal.Ticks)
                        continue;
                    appInfo.Set(DataState.loaded);
                    if (appInfo.appMode == AppMode.fromto)
                    {
                        if ((xx.Ticks >= EndUniversal.Ticks))
                        {
                            StopMonitor();
                        }
                    }
                    else if (appInfo.appMode == AppMode.from)
                    {
                        if ((xx.Ticks >= EndUniversal.Ticks))
                        {
                            StopMonitor();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"\nMessage received on partition {partitionEvent.Partition.PartitionId}:");
                    System.Diagnostics.Debug.WriteLine($"\tMessage body: {data}");
                    
                    if ((MyMethodName != null) &&(!string.IsNullOrEmpty(data)))
                    {
                        Geolocation? _location;
                        if (data.Contains("Error"))
                            continue;
                        try
                        {
                            _location = JsonConvert.DeserializeObject<Geolocation>(data);
                        } catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Skip");
                            continue;
                        }

                        if (_location != null)
                        {
                            Geolocation location = (Geolocation)_location;

                            double lat = location.lat;
                            double lon = location.lon;
                            double alt = location.alt;

                            Telemetry telem = new Telemetry(location, xx);
                            telemetrys.Add(telem);
  
                            double[] dbl = new double[] { lat, lon };
                            int res = ((Func<Telemetry, int>)MyMethodName)(telem);
                        }
                        ct.ThrowIfCancellationRequested();
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
            catch (TaskCanceledException ex)
            {
                // This is expected when the token is signaled; it should not be considered an
                // error in this scenario.
                System.Diagnostics.Debug.WriteLine("Cancelled");
                appInfo.Set(AppState.none);
                if(appInfo.appMode != AppMode.live)
                    appInfo.Set(DataState.loaded);
                HideLoading = false;
                Telemetry telem = new Telemetry { lat=0,lon=0, alt=0 };
                int res = ((Func<Telemetry, int>)MyMethodName)(telem);
            }
        }

        private static void PrintProperties(KeyValuePair<string, object> prop)
        {
            string? propValue = prop.Value is DateTime time
                ? time.ToString("O") // using a built-in date format here that includes milliseconds
                : prop.Value.ToString();

            System.Diagnostics.Debug.WriteLine($"\t\t{prop.Key}: {propValue}");
        }
    }
}
