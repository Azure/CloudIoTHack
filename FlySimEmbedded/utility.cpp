// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

#include "Arduino.h"
#include "AzureIotHub.h"

#include "HTS221Sensor.h"
#include "LSM6DSLSensor.h"
#include "RGB_LED.h"

#include "parson.h"
#include "config.h"

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

void parseTwinMessage(DEVICE_TWIN_UPDATE_STATE updateState, const char *message)
{
    JSON_Value *root_value;
    root_value = json_parse_string(message);
    if (json_value_get_type(root_value) != JSONObject)
    {
        if (root_value != NULL)
        {
            json_value_free(root_value);
        }
        LogError("parse %s failed", message);
        return;
    }
    JSON_Object *root_object = json_value_get_object(root_value);

    double val = 0;
    if (updateState == DEVICE_TWIN_UPDATE_COMPLETE)
    {
        JSON_Object *desired_object = json_object_get_object(root_object, "desired");
        if (desired_object != NULL)
        {
            val = json_object_get_number(desired_object, "interval");
        }
    }
    else
    {
        val = json_object_get_number(root_object, "interval");
    }
    if (val > 500)
    {
        interval = (int)val;
        LogInfo(">>>Device twin updated: set interval to %d", interval);
    }
    json_value_free(root_value);
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

bool readMessage(int messageId, char *payload, size_t len)
{
    float temperature = readTemperature();
    float humidity = readHumidity();

    setMotionGyroSensor();

    float xMovement = readXMovement();
    float yMovement = readYMovement();
    float zMovement = readZMovement();

    JSON_Value *root_value = json_value_init_object();
    JSON_Object *root_object = json_value_get_object(root_value);

    json_object_set_string(root_object, "deviceId", DISPLAY_NAME);
    json_object_set_number(root_object, "messageId", messageId);
    
    bool temperatureAlert = false;

    time_t rawtime;
    struct tm * timeinfo;
    char buffer [80];
    time (&rawtime);
    timeinfo = localtime (&rawtime);  
    strftime (buffer, 80,"%m/%d/%y %H:%M:%S",timeinfo);

    json_object_set_string(root_object, "timestamp", buffer);
    
    json_object_set_number(root_object, "temperature", ceil(temperature));
    if(temperature > TEMPERATURE_ALERT)
    {
        temperatureAlert = true;
    }

    json_object_set_number(root_object, "humidity", ceil(humidity));

    json_object_set_number(root_object, "x", xMovement);
    json_object_set_number(root_object, "y", yMovement);
    json_object_set_number(root_object, "z", zMovement);
    
    char *serialized_string = json_serialize_to_string(root_value);
    
    strncpy(payload, serialized_string, len);
    payload[len - 1] = 0;
    
    json_free_serialized_string(serialized_string);
    json_value_free(root_value);

    return temperatureAlert;
}