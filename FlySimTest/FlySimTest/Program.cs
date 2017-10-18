using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySimTest
{
    class Program
    {
        static int _count = 20;
        static List<Airplane> _airplanes = new List<Airplane>();
        static EventHubClient _client;

        static void Main(string[] args)
        {
            Task.Run(async () => { await Test(args); }).GetAwaiter().GetResult();
        }

        static async Task Test(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    _count = Convert.ToInt32(args[0]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Usage: FlySimLoadTest [count]");
                }
            }

            for (int i = 0; i < _count; i++)
            {
                var airplane = new Airplane();
                airplane.deviceId = Guid.NewGuid().ToString().Substring(24, 12);
                var hash = (uint)airplane.deviceId.GetHashCode();
                airplane.heading = hash % 360;
                airplane.altitude = 30000.0 + ((hash % 5) * 1000); // All planes within 5,000 feet for testing
                airplane.latitude = 36.7 + ((double)((hash / 100) % 100) / 100.0);
                airplane.longitude = -116.8 + ((double)(hash % 100) / 50.0);
                airplane.airspeed = 384.0;
                airplane.pitch = 0.0;
                airplane.roll = 0.0;
                airplane.timestamp = DateTime.Now;
                airplane.temperature = 72.0;
                airplane.humidity = 50.0;
                _airplanes.Add(airplane);
            }

            _client = EventHubClient.CreateFromConnectionString("SHARED_EVENT_HUB_ENDPOINT;EntityPath=flysim-shared-input-hub");

            while (true)
            {
                foreach (var airplane in _airplanes)
                {
                    var result = TransmitFlightDataAsync(airplane); // Do not await
                    Console.WriteLine(String.Format("Name={0}, Latitude={1}, Longitude={2}",
                        airplane.deviceId, airplane.latitude, airplane.longitude));
                }

                Console.WriteLine();
                await Task.Delay(2000);

                foreach (var airplane in _airplanes)
                {
                    UpdateFlightData(airplane);
                }
            }
        }

        static async Task TransmitFlightDataAsync(Airplane airplane)
        {
            var message = JsonConvert.SerializeObject(airplane);
            await _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        }

        static void UpdateFlightData(Airplane airplane)
        {
            var now = DateTime.Now;
            var milliseconds = (now - airplane.timestamp).TotalMilliseconds;
            var radians = airplane.heading * Math.PI / 180.0;
            var distance = (milliseconds / 1000) * (airplane.airspeed * 0.000277778); // 1 MPH == 0.000277778 miles per second
            var dx = distance * Math.Sin(radians);
            var dy = distance * Math.Cos(radians);
            airplane.latitude += (dy / 69.0); // Assume 69 miles per 1 degree of latitude
            airplane.longitude += (dx / 69.0); // Assume 69 miles per 1 degree of longitude
            airplane.timestamp = now;
        }
    }

    class Airplane
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
