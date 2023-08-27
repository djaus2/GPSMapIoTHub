// Copyright (c) Microsoft Corporation. All rights reserved.
// SPDX-License-Identifier: MIT

#define IOT_CONFIG_WIFI_SSID "<SSID>"
#define IOT_CONFIG_WIFI_PASSWORD "<SSIDPWD"

// Azure IoT
#define IOT_CONFIG_IOTHUB_FQDN "<HUB PATH>"
#define IOT_CONFIG_DEVICE_ID "<DEVICE ID>"
#define IOT_CONFIG_DEVICE_KEY "<DEVICE KEY>"


// Publish 1 message every 2 seconds
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_START 1000 //Start at 1s
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_MAX 60000  //Will exp up to 60s
// delay at end of loop()
#define LOOP_DELAY 500

// When CD Message sent, trate as a command
// Split on this to get command-parameter
#define PAYLOAD_PARAM_SEP '-'

// If not connected can run without serial port
#define SerialConnected  true

/////////////////////////////////////////////////////////////////////

void SetTelemetryGap(int gap);
void GPSsetup();


// Receive CD Messages:
void receivedCallback(char* topic, byte* payload, unsigned int length);
enum Messages { msgtelemetryStart, msgtelemetryStop, msgtelemetryReset, msgtelemetrySet, msgtelemetryNone };


void onActivateRelayCommand(String cmdName, JsonVariant jsonValue);

