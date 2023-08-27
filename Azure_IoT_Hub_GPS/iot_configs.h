// Copyright (c) Microsoft Corporation. All rights reserved.
// SPDX-License-Identifier: MIT

#define IOT_CONFIG_WIFI_SSID "APQLZM"
#define IOT_CONFIG_WIFI_PASSWORD "silly1371"

// Azure IoT
#define IOT_CONFIG_IOTHUB_FQDN "AzIoTHubDj1.azure-devices.net"
#define IOT_CONFIG_DEVICE_ID "AzTelemHubDevice1"
#define IOT_CONFIG_DEVICE_KEY "oIciiPuh0w1pfnwcTPc6cjGh+bttT/buzNp6OJCIbxQ="


// Publish 1 message every 2 seconds
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_START 1000 //Start at 1s
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_MAX 60000  //Will exp up to 60s
#define LOOP_DELAY 2000
#define PAYLOAD_PARAM_SEP '-'

#define SerialConnected  true

void SetTelemetryGap(int gap);
void GPSsetup();

enum Messages {msgtelemetryStart,msgtelemetryStop,msgtelemetryReset,msgtelemetrySet,msgtelemetryNone};

// Receive CD Messages:
void receivedCallback(char* topic, byte* payload, unsigned int length);

void onActivateRelayCommand(String cmdName, JsonVariant jsonValue);

