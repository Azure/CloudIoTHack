using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace FlySimFunctions
{
    public static class FlySimIoTFlightData
    {
        private static HttpClient client = new HttpClient();

        private static DateTime? _last = null;
        private static double _airspeed = 384.0;
        private static double _heading = 0.0;
        private static double _altitude = 32000.0;
        private static double _latitude = 37.242;
        private static double _longitude = -115.8190;
        private static double _pitch = 0.0;
        private static double _roll = 0.0;

        [FunctionName("FlySimIoTFlightData")]
        public async static Task Run([IoTHubTrigger("%eventHubConnectionPath%", Connection = "eventHubConnectionString")]EventData message, [EventHub("flysim", Connection = "cloudcityEventHubConnection")] IAsyncCollector<string> outputMessage, ILogger log)
        {        
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            var converter = new IsoDateTimeConverter { DateTimeFormat = "MM/dd/yy HH:mm:ss" };
            var input = JsonConvert.DeserializeObject<Input>(Encoding.UTF8.GetString(message.Body.Array), converter);
            var outputPayload = "";
            if (_last != null && input.timestamp > _last.Value)
            {
                // Compute milliseconds elapsed since last event was received
                var milliseconds = (input.timestamp - _last.Value).TotalMilliseconds;
                _last = input.timestamp;
	
                // Constrain pitch to +/-15 degrees (positive == nose down)
                _pitch = Math.Max(Math.Min(input.y / 11.0, 15.0), -15.0);
	
                // Constrain roll to +/-30 degrees (positive == rolling right)
                _roll = Math.Max(Math.Min(input.x / 11.0, 30.0), -30.0);
	
                // Compute new heading assuming hard left or right turns 10 degrees per second
                var delta = (milliseconds / 100.0) * (_roll / 30.0);
                _heading += delta;
	
                if (_heading < 0.0)
                    _heading += 360.0;
                else if (_heading >= 360.0)
                    _heading -= 360.0;
	
                // Compute new latitude and longitude
                var radians = _heading * Math.PI / 180.0;
                var distance = (milliseconds / 1000) * (_airspeed * 0.000277778); // 1 MPH == 0.000277778 miles per second
                var dx = distance * Math.Sin(radians);
                var dy = distance * Math.Cos(radians);
                _latitude += (dy / 69.0); // Assume 69 miles per 1 degree of latitude
                _longitude += (dx / 69.0); // Assume 69 miles per 1 degree of longitude
	
                // Compute new altitude and constrain it to 1,000 to 40,000 feet
                _altitude = _altitude - (distance * 5280.0 * Math.Sin(_pitch * Math.PI / 180.0));
                _altitude = Math.Max(Math.Min(_altitude, 40000.0), 1000.0);
	
                 // Send JSON output
                var output = new Output { deviceId = input.deviceId, timestamp = input.timestamp, temperature = input.temperature, humidity = input.humidity, airspeed = _airspeed, altitude = _altitude, heading = _heading, latitude = _latitude, longitude = _longitude, pitch = _pitch, roll = _roll };
                outputPayload = JsonConvert.SerializeObject(output);
            }
            else
            {
                // This is the first event received, so compute initial parameters
                _last = input.timestamp;
	
                var hash = (uint)input.deviceId.GetHashCode();
                _heading = hash % 360;
                _altitude = 10000.0 + ((hash % 25) * 1000);
                _latitude = 36.7 + ((double)((hash / 100) % 100) / 100.0);
                _longitude = -116.8 + ((double)(hash % 100) / 50.0);

                var output = new Output { deviceId = input.deviceId, timestamp = input.timestamp, temperature = input.temperature, humidity = input.humidity, airspeed = _airspeed, altitude = _altitude, heading = _heading, latitude = _latitude, longitude = _longitude, pitch = _pitch, roll = _roll };
                outputPayload = JsonConvert.SerializeObject(output);
	                
                log.LogInformation("First event received");
            }
	
            await outputMessage.AddAsync(outputPayload);
            log.LogInformation(String.Format("Heading={0}, Altitude={1}, Latitude={2}, Longitude={3}", _heading, _altitude, _latitude, _longitude));
            return;
        }
    }

    class Input
    {
        public string deviceId;
        public DateTime timestamp;
        public string messageId;
        public double temperature;
        public double humidity;
        public double x;
        public double y;
        public double z;
    }
	
    class Output
    {
        public string deviceId;
        public DateTime timestamp;
        public double airspeed;
        public double heading;
        public double altitude;
        public double latitude;
        public double longitude;
        public double pitch;
        public double roll;
        public double temperature;
        public double humidity;
    }
}