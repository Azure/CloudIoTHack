// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. 

#ifndef UTILITY_H
#define UTILITY_H

void parseTwinMessage(DEVICE_TWIN_UPDATE_STATE, const char *);
bool readMessage(int, char *, size_t);

void sensorInit(void);

void blinkLED(void);
void blinkLEDLonger(void);
void blinkSendConfirmation(void);
int getInterval(void);

#endif /* UTILITY_H */