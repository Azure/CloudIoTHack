using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim
{
    public static class ActivityExtensions
    {
        public static void Hydrate(this FlightInformation flightInfo, NewFlightInfo info)
        {
            flightInfo.DisplayName = info.deviceId;
            flightInfo.CurrentAltitude = info.altitude;
            flightInfo.CurrentHeading = info.heading;
            flightInfo.CurrentTemperature = info.temperature;
            flightInfo.CurrentHumidity = info.humidity;
            flightInfo.CurrentRoll = info.roll * -1;
            flightInfo.CurrentSpeed = info.airspeed;
            flightInfo.CurrentPitch = info.pitch * 4.0 * -1;
        }
    }
}
