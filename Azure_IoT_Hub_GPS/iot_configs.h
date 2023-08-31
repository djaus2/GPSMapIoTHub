// Copyright (c) Microsoft Corporation. All rights reserved.
// SPDX-License-Identifier: MIT

#define IOT_CONFIG_WIFI_SSID "<SSID>"
#define IOT_CONFIG_WIFI_PASSWORD "<SSID_PWD"

#define IOT_CONFIG_WIFI_SSID_MOBILE "<MOBILE_SSID>"
#define IOT_CONFIG_WIFI_PASSWORD_MOBILE "M<OBILE_SSID_PWD>"

// Azure IoT
#define IOT_CONFIG_IOTHUB_FQDN "<HUB PATH>"
#define IOT_CONFIG_DEVICE_ID "<DEVICE ID>"
#define IOT_CONFIG_DEVICE_KEY "<DEVICE KEY>"


// Publish 1 message every 2 seconds
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_START 1000 //Start at 1s
#define TELEMETRY_FREQUENCY_MILLISECS_GAP_MAX 10000  //Will exp up to 10s
#define LOOP_DELAY 1000
#define PAYLOAD_PARAM_SEP '-'

//#define SerialConnected  false

#define WIFI_SRC_PIN 13
#define SERIAL_MODE_PIN 12

void SetTelemetryGap(int gap);
void GPSsetup();

enum Messages {msgtelemetryStart,msgtelemetryStop,msgtelemetryReset,msgtelemetrySet,msgtelemetryNone};

// Receive CD Messages:
void receivedCallback(char* topic, byte* payload, unsigned int length);


void onActivateRelayCommand(String cmdName, JsonVariant jsonValue);

