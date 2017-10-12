// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

#include "HTS221Sensor.h"
#include "AzureIotHub.h"
#include "Arduino.h"
#include <ArduinoJson.h>
#include "config.h"
#include "RGB_LED.h"
#include "AZ3166WiFi.h"
#include "Sensor.h"
#include "SystemVersion.h"
#include "http_client.h"
#include "telemetry.h"

#define RGB_LED_BRIGHTNESS 32

DevI2C *i2c;
HTS221Sensor *sensor;
LSM6DSLSensor *acc_gyro;
static RGB_LED rgbLed;
static int interval = INTERVAL;
int axes[3];
float xMovement;
float yMovement;
float zMovement;

int getInterval()
{
    return interval;
}

void blinkLEDLonger()
{
    rgbLed.turnOff();
    rgbLed.setColor(RGB_LED_BRIGHTNESS, 0, 0);
    delay(5000);
    rgbLed.turnOff();
}

void blinkLED()
{
    rgbLed.turnOff();
    rgbLed.setColor(RGB_LED_BRIGHTNESS, 0, 0);
    delay(500);
    rgbLed.turnOff();
}

void blinkSendConfirmation()
{
    rgbLed.turnOff();
    rgbLed.setColor(0, 0, RGB_LED_BRIGHTNESS);
    delay(500);
    rgbLed.turnOff();
}

void parseTwinMessage(const char *message)
{
    StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    JsonObject &root = jsonBuffer.parseObject(message);
    if (!root.success())
    {
        LogError("parse %s failed", message);
        return;
    }

    if (root["desired"]["interval"].success())
    {
        interval = root["desired"]["interval"];
    }
    else if (root.containsKey("interval"))
    {
        interval = root["interval"];
    }
}

void sensorInit()
{
    i2c = new DevI2C(D14, D15);

    acc_gyro = new LSM6DSLSensor(*i2c, D4, D5);
    acc_gyro->init(NULL);
    acc_gyro->enableAccelerator();
    acc_gyro->enableGyroscope();

    sensor = new HTS221Sensor(*i2c);
    sensor->init(NULL);    
}

void setMotionGyroSensor()
{
    acc_gyro->getXAxes(axes);
    char buff[128];

    xMovement = axes[0];
    yMovement = axes[1];
    zMovement = axes[2];
     
    snprintf(buff, 128, "IN FLIGHT: \r\n x:%d \r\n y:%d \r\n z:%d  ", axes[0], axes[1], axes[2]);
    Screen.print(buff);
    
}

float readTemperature()
{
    sensor->reset();

    float temperature = 0;
    sensor->getTemperature(&temperature);

    return temperature;
}

float readHumidity()
{
    sensor->reset();

    float humidity = 0;
    sensor->getHumidity(&humidity);

    return humidity;
}

float readXMovement()
{

    return xMovement;
}

float readYMovement()
{

    return yMovement;
}
 
float readZMovement()
{

    return zMovement;
}

bool readMessage(int messageId, char *payload)
{
    float temperature = readTemperature();
    float humidity = readHumidity();

    setMotionGyroSensor();

    float xMovement = readXMovement();
    float yMovement = readYMovement();
    float zMovement = readZMovement();

    StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    JsonObject &root = jsonBuffer.createObject();

    root["deviceId"] = DEVICE_ID;
    root["messageId"] = messageId;

    bool temperatureAlert = false;

    time_t rawtime;
    struct tm * timeinfo;
    char buffer [80];
    time (&rawtime);
    timeinfo = localtime (&rawtime);  
    strftime (buffer, 80,"%m/%d/%y %H:%M:%S",timeinfo);

    root["timestamp"] = buffer;

    if(temperature != temperature)
    {
        root["temperature"] = NULL;
    }
    else
    {
        root["temperature"] = temperature;
        if(temperature > TEMPERATURE_ALERT)
        {
            temperatureAlert = true;
        }
    }

    if(humidity != humidity)
    {
        root["humidity"] = NULL;
    }
    else
    {
        root["humidity"] = humidity;
    }

    if (xMovement != xMovement)
    {
        root["x"] = NULL;
    }
    else
    {
        root["x"] = xMovement;
    }

    if (yMovement != yMovement)
    {
        root["y"] = NULL;
    }
    else
    {
        root["y"] = yMovement;
    }

    if (zMovement != zMovement)
    {
        root["z"] = NULL;
    }
    else
    {
        root["z"] = zMovement;
    }

    root.printTo(payload, MESSAGE_MAX_LEN);

    return true;
}