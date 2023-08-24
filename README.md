# GPSMap

Monitor an Azure IoT Hub and  map the GPS coordinatess sent to it  as telemetry.

## About

This project is a coming together of the 7th Arduino Sketch (Azure_IoT_Hub_GPS) on GitHub at [djaus2/RPI Pico W GPS and Bluetooth](https://github.com/djaus2/RpiPicoWGPSandBT) and the [Telerik Map Demo](https://demos.telerik.com/blazor-ui/map/overview). It started out as an endeavour to map that Sketch's IoT Hub telemetry in Azure IOT Central but this was deemed a simpler approach. This app maps the current location of the GPS device.

## Getting Started

You will need a Telerik license, whether a full license or a 30 day trial. Follow the steps as below3 at [First Steps with Server-Side UI for Blazor](https://docs.telerik.com/blazor-ui/getting-started/server-blazor?_ga=2.73417493.680605814.1692843673-472055910.1692083918&_gl=1*13uct7u*_ga*NDcyMDU1OTEwLjE2OTIwODM5MTg.*_ga_9JSNBCSF54*MTY5Mjg0MzY3NC44LjEuMTY5Mjg0Mzc4NC4xMS4wLjA.)
- Step 0: Download Telerik UI for Blazor
- Step 2: Add the Telerik NuGet Feed to Visual Studio
- Step 3: Install the Telerik UI for Blazor Components
  The other steps have been implemented in teh project.

Azure IoT Hub and Device
- As used with Sketch 7.
- Having created them, see elsewhere ..

Now open this project and add your connection details in appsettings.json:
- "HubName": "he IoT Hub Name
- "EventHubConnectionString": The Endpoint
  - Go to the **IoT Hub** in **Azure Portal**
  - Select **Built-in endpoints**
  - Scroll down to **Event Hub compatible endpoint**
  - Select the **Endpoint**
 
OR

If you have AzCli installed:
- Create a prompted and login
- Run ```az iot hub connection-string show -n <IoT Hub name> --default-eventhub```
  

