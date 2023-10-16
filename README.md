# GPSMap

A Blazor Server app to monitor an Azure IoT Hub and  map the GPS coordinatess sent to it  as telemetry.  
Uses Telerik Map Component.

<hr/>

## Updates

- **Scrolling** map < > Up and Down works, Recentres if location is more than 200m off centre.
- **6X Replay** option _( Default telemetry period is 1 / 10 sec, so 6x is 1 min of telemetry / 10s )_
- **WorkInProgress** branch has been merged back into Master
- Much improved **NMEA GPGGA** sentence filter.
- Can record and playback history
- Can jumper GP12/13 for selecting between two wifis and whether or not to use Serial.  Can also select Blutooth, GP11
  - GP13 _High = _Deskyop WiFi , alt Mobile Wifi_
      - Have dev WiFi and my phone wifi for when mobile
  - GP12 _High = _Serial enabled_
  - GP11 _High = _BT enabled but Serial takes precedence_
    - If Serial/BT enabled, it blocks if no Serial/BT connected.

<hr/>


![Device Image](https://github.com/djaus2/GPSMapIoTHub/blob/master/GPSMap/wwwroot/images/uart-gps-module-with-real-time-clock.jpg)  
**_A typical GPS Device, suitable for Arduino use_**

## About

This project is a coming together of the 7th Arduino Sketch (Azure_IoT_Hub_GPS) on GitHub at [djaus2/RPI Pico W GPS and Bluetooth](https://github.com/djaus2/RpiPicoWGPSandBT) and the [Telerik Map Demo](https://demos.telerik.com/blazor-ui/map/overview). It started out as an endeavour to map that Sketch's IoT Hub telemetry in Azure IOT Central but this was deemed a simpler approach. This app maps the current location of the GPS device.

![App Image](https://github.com/djaus2/GPSMapIoTHub/blob/master/GPSMap/wwwroot/images/gpsmapapp.png)  
**_The App showing the location of the GPS device_**

## Azure  Sketch
As per the seventh Sketch in [djaus2/RPI Pico W GPS and Bluetooth](https://github.com/djaus2/RpiPicoWGPSandBT)  but modified specifically for this app. 
- Json is single level.
  - eg. ```{"lat":-37.7468138,"lon":144.8956847,"alt":64.6}```
- Includes Cloud to Device Messages that are interpreted as  Telemetry config _(case sensitive)_ commands:
  - Reset
    - Restarts the telemetry timing
    - At first is 1s but doubles each send until it reaches 60s.
  - Start
    - Restarts telemetry without any change to the period.
  - Stop
    - Stops telemetry without any chnage to the period.
  - Set-Period
    - Set the telemetry peroiod
    - The command is Set separated using a highen to:
      - The period, a numerical parameter in milliseconds
    - eg. ```Set-30000```  sets the period between sends to 30 seconds _(approx)_.

> Have added abilty to send commands from GPSMap app to the device.

## Getting Started

Setup the above Sketch in an Arduino Pico W. See [djaus2/RPI Pico W GPS and Bluetooth](https://github.com/djaus2/RpiPicoWGPSandBT) .

You will need a Telerik license, whether a full license or a 30 day trial. Follow the steps as below3 at [First Steps with Server-Side UI for Blazor](https://docs.telerik.com/blazor-ui/getting-started/server-blazor?_ga=2.73417493.680605814.1692843673-472055910.1692083918&_gl=1*13uct7u*_ga*NDcyMDU1OTEwLjE2OTIwODM5MTg.*_ga_9JSNBCSF54*MTY5Mjg0MzY3NC44LjEuMTY5Mjg0Mzc4NC4xMS4wLjA.)
- Step 0: Download Telerik UI for Blazor
- Step 2: Add the Telerik NuGet Feed to Visual Studio
- Step 3: Install the Telerik UI for Blazor Components
  The other steps have been implemented in the project.

Azure IoT Hub and Device
- As used with Sketch 7.
- Having created them need Hub name. 

Now open this project and add your connection details in appsettings.json:
- "HubName": The IoT Hub Name
- "HubConnectionString": The IoT Hub connection string
  - On the left-side pane of your IoT hub, select Shared access policies
  - From the list of policies, select the service policy.
  - Copy the Primary connection string.
- "DeviceId": The target device's Id
- "EventHubConnectionString": The Endpoint
  - Go to the **IoT Hub** in **Azure Portal**
  - Select **Built-in endpoints**
  - Scroll down to **Event Hub compatible endpoint**
  - Select the **Endpoint**


OR

If you have [Azure Cli installed](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli).
- Create an azcli prompt and login
- Run ```az iot hub connection-string show -n <IoT Hub name> --policy-name service --key-type primary```
- Run ```az iot hub connection-string show -n <IoT Hub name> --default-eventhub```

<hr/>
  
## Operational Modes:

- Live: Scan through historical telemetry then map wrt live telemetry
- Historical: Scan though al previous telemetry (typically a day) then replay in timely manner.
  - Can set start and end date/time

<hr />

## Footnote

If running the Blazor app in development mode, the appp can be opened in a phone browser locally using **ngrok** as 
per the blog post: [Blazor Server Development: Viewing in a phone](https://davidjones.sportronics.com.au/web/Blazor-Viewing_in_a_phone-web.html).
  

