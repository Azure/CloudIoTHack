# 1 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino"
# 1 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino"
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. 
// To get started please visit https://microsoft.github.io/azure-iot-developer-kit/docs/projects/connect-iot-hub?utm_source=ArduinoExtension&utm_medium=ReleaseNote&utm_campaign=VSCode
# 5 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2
# 6 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2
# 7 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2
# 8 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2
# 9 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2
# 10 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 2

static bool hasWifi = false;
int messageCount = 1;

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

void setup()
{
    hasWifi = false;
    initWifi();
    if (!hasWifi)
    {
        do{{ LOGGER_LOG l = xlogging_get_log_function(); if (l != 
# 37 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino" 3 4
       __null
# 37 "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino"
       ) l(AZ_LOG_INFO, "C:\\Users\\codes\\Documents\\Arduino\\generated_examples\\FlySim\\FlySim.ino", __func__, 37, 0x01, "Please make sure the wifi connected!"); }; }while((void)0,0);
        return;
    }

    // Microsoft collects data to operate effectively and provide you the best experiences with our products. 
    // We collect data about the features you use, how often you use them, and how you use them.
    send_telemetry_data_async("", "HappyPathSetup", "");

    Serial.begin(115200);
    sensorInit();
    iothubInit();
}

void loop()
{
    char messagePayload[256];
    bool temperatureAlert = readMessage(messageCount, messagePayload);
    iothubSendMessage((const unsigned char *)messagePayload, temperatureAlert);
    iothubLoop();
    delay(50);
}
