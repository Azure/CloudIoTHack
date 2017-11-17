// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. 
// To get started please visit https://microsoft.github.io/azure-iot-developer-kit/docs/projects/connect-iot-hub?utm_source=ArduinoExtension&utm_medium=ReleaseNote&utm_campaign=VSCode
#include "AZ3166WiFi.h"
#include "AzureIotHub.h"
#include "DevKitMQTTClient.h"

#include "config.h"
#include "utility.h"
#include "SystemTickCounter.h"

static bool hasWifi = false;
int messageCount = 1;
static bool messageSending = true;
static uint64_t send_interval_ms;

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Utilities
void initWifi()
{
     Screen.print("IoT DevKit\r\n \r\nConnecting...\r\n");

    if (WiFi.begin() == WL_CONNECTED)
    {
        IPAddress ip = WiFi.localIP();
        Screen.print(1, ip.get_address());
        hasWifi = true;
        Screen.print(2, "Running... \r\n");
    }
    else
    {
        Screen.print(1, "No Wi-Fi\r\n ");
    }
}

static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result)
{
    if (IOTHUB_CLIENT_CONFIRMATION_OK == result)
    {
        LogInfo("Flight data received by your Azure IoT Hub successfully.");
        blinkSendConfirmation();
    }
    else
    {
        LogInfo("Failed to send flight data to your Azure IoT Hub.");
    }
}

static void sendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result)
{
    if (result == IOTHUB_CLIENT_CONFIRMATION_OK)
    {
        blinkSendConfirmation();
    }
}

static void messageCallback(const char* payLoad, int size)
{
    char buff[128];
    
    //Get the flight status coming from Azure
    snprintf(buff, 128, "FLIGHT WARNING\r\n \r\n %s \r\n \r\n", payLoad);
    Screen.print(buff);
        
    LogInfo("Message received from the Azure IoT Portal: %s", payLoad);

    //Show message with LED for a while
    for (int i = 0; i < 5; i++)
    {
        blinkLED();
        delay(500);
    }
}

int deviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size)
{
    LogInfo("Try to invoke method %s", methodName);
    const char *responseMessage = "\"Successfully invoke device method\"";
    int result = 200;

    if (strcmp(methodName, "start") == 0)
    {
        LogInfo("Starting transmission of flight data...");
        messageSending = true;
    }
    else if (strcmp(methodName, "stop") == 0)
    {
        LogInfo("Stopping transmision of flight data...");
        messageSending = false;
    }
    else
    {
        LogInfo("No method %s found", methodName);
        responseMessage = "\"No method found\"";
        result = 404;
    }

    *response_size = strlen(responseMessage);
    *response = (unsigned char *)malloc(*response_size);
    strncpy((char *)(*response), responseMessage, *response_size);

    return result;
}

static void deviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
  char *temp = (char *)malloc(size + 1);
  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';
  parseTwinMessage(updateState, temp);
  free(temp);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch
void setup()
{
    Screen.init();
    Screen.print(0, "IoT DevKit");
    Screen.print(2, "Initializing...");
    
    Screen.print(3, " > Serial");
    Serial.begin(115200);

    hasWifi = false;
    Screen.print(3, " > WiFi");
    initWifi();
    if (!hasWifi)
    {
        LogInfo("Please make sure the wifi connected!");
        return;
    }
    
    LogTrace("FlySim", NULL);

    Screen.print(3, " > Sensors");
    sensorInit();
  
    Screen.print(3, " > IoT Hub");
    DevKitMQTTClient_Init(true);

    DevKitMQTTClient_SetSendConfirmationCallback(sendConfirmationCallback);
    DevKitMQTTClient_SetMessageCallback(messageCallback);
    DevKitMQTTClient_SetDeviceTwinCallback(deviceTwinCallback);
    DevKitMQTTClient_SetDeviceMethodCallback(deviceMethodCallback);
}

void loop()
{
    if (hasWifi)
    {
        if (messageSending && 
            (int)(SystemTickCounterRead() - send_interval_ms) >= getInterval())
        {
            char messagePayload[MESSAGE_MAX_LEN];

            bool temperatureAlert = readMessage(messageCount++, messagePayload, MESSAGE_MAX_LEN);
            EVENT_INSTANCE* message = DevKitMQTTClient_Event_Generate(messagePayload, MESSAGE);
            DevKitMQTTClient_Event_AddProp(message, "temperatureAlert", temperatureAlert ? "true" : "false");
            if (DevKitMQTTClient_SendEventInstance(message))
            {
                LogInfo("Flight data sent to your Azure IoT Hub successfully.");
            }
            else
            {
                LogInfo("Failed to send flight data to your Azure IoT Hub.");
            }

            send_interval_ms = SystemTickCounterRead();
        }
        else
        {
            DevKitMQTTClient_Check();
        }
    }
    delay(10);
}
