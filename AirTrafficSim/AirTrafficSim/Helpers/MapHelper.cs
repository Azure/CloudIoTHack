using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Maps;

namespace AirTrafficSim.Helpers
{
    public static class MapHelper
    {
        public static Random Randomizer = new Random();

        public static double EarthRadiusInMiles = 3956.0;
        public static double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = EarthRadiusInKilometers; }
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }


        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }

        public static double TryGetDistance(this MapControl map)
        {
            double mapDistance = 0.0;

            Geopoint northWest;
            Geopoint southEast;

            if (map != null)
            {
                try
                {
                    map.GetLocationFromOffset(new Point(0, 0), out northWest);
                    map.GetLocationFromOffset(new Point(map.ActualWidth, map.ActualHeight), out southEast);

                    mapDistance = Helpers.MapHelper.CalcDistance(northWest.Position.Latitude, northWest.Position.Longitude, southEast.Position.Latitude, southEast.Position.Longitude, MapHelper.GeoCodeCalcMeasurement.Miles);

                    App.ViewModel.CurrentGridScale = mapDistance;// / 12.0;
                }
                catch (Exception ex)
                {

                }
            }

            return mapDistance;
        }

        public static void LoadFakes(ObservableCollection<ActivePlaneInformation> activePlanes)
        {
            double altitude = 20000;
            double latitude = 0;
            double longitude = 0;

            for (int i = 0; i < 15; i++)
            {
                altitude = Randomizer.Next(20000, 40000);

                latitude = 36.555484545272044 + Randomizer.NextDouble() * (37.954954874206614 - 36.555484545272044);
                longitude = 114.26115391658938 + Randomizer.NextDouble() * (117.42832986446284 - 114.26115391658938);

                activePlanes.Add(new ActivePlaneInformation()
                {
                    DisplayName = $"Pilot {i + 1}",
                    FlightInformation = new FlightInformation(altitude)
                    {
                        CurrentAltitude = altitude,
                        CurrentHeading = Randomizer.Next(1, 360),
                    },
                    Location = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = latitude,
                        Longitude = longitude * -1,
                    }),
                    Status = Common.FlightStatus.Ok,
                });
            }

            activePlanes.Add(new ActivePlaneInformation()
            {
                DisplayName = $"Pilot {16}",
                FlightInformation = new FlightInformation(altitude)
                {
                    CurrentAltitude = altitude,
                    CurrentHeading = Randomizer.Next(1, 360),
                },
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 37.197944481053035,
                    Longitude = -115.56497959266275
                }),
                Status = Common.FlightStatus.AtRisk,
            });
            
            activePlanes.Add(new ActivePlaneInformation()
            {
                DisplayName = $"Pilot {17}",
                FlightInformation = new FlightInformation(altitude)
                {
                    CurrentAltitude = altitude,
                    CurrentHeading = Randomizer.Next(1, 360),
                },
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 37.3,
                    Longitude = -115.6
                }),
                Status = Common.FlightStatus.AtRisk,
            });
        }

        public static List<BasicGeoposition> GetAreaBoundaries()
        {
            List<BasicGeoposition> areaBounds = new List<BasicGeoposition>();

            areaBounds.Add(new BasicGeoposition() { Latitude = 37.917388162022256, Longitude = -117.3781194436157 });
            areaBounds.Add(new BasicGeoposition() { Latitude = 37.916074924119656, Longitude = -114.27100862192687 });
            areaBounds.Add(new BasicGeoposition() { Latitude = 36.569446939023756, Longitude = -114.30248033933273 });
            areaBounds.Add(new BasicGeoposition() { Latitude = 36.550956322695271, Longitude = -117.34996151882672 });

            return areaBounds;
        }

        public static Geopoint AreaCenter
        {
            get
            {
                return new Geopoint(new BasicGeoposition()
                {
                    Latitude = 37.242,
                    Longitude = -115.8191

                });
            }
        }


    }
}
