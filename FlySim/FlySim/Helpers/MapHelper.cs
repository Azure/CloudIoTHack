using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Maps;

namespace FlySim.Helpers
{
    public static class MapHelper
    {
        public enum GeoCodeCalcMeasurement
        {
            Miles = 0,
            Kilometers = 1
        }

        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        private const double Wgs84A = 6378137.0; // Major semiaxis [m]
        private const double Wgs84B = 6356752.3; // Minor semiaxis [m]

        public static Geopoint AreaCenter => new Geopoint(new BasicGeoposition
        {
            Latitude = 37.242,
            Longitude = -115.8191
        });

        public static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        public static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        public static GeoboundingBox GenerateGeoboundingBox(this BasicGeoposition point, double altitude,
            double halfSideInKm)
        {
            // Bounding box surrounding the point at given coordinates,
            // assuming local approximation of Earth surface as a sphere
            // of radius given by WGS84
            var lat = DegreesToRadians(point.Latitude);
            var lon = DegreesToRadians(point.Longitude);
            var halfSide = 1000 * halfSideInKm;

            // Radius of Earth at given latitude
            var radius = EarthRadius(lat);
            // Radius of the parallel at given latitude
            var pradius = radius * Math.Cos(lat);

            var latMin = lat - halfSide / radius;
            var latMax = lat + halfSide / radius;
            var lonMin = lon - halfSide / pradius;
            var lonMax = lon + halfSide / pradius;

            var nwCorner = new BasicGeoposition
            {
                Latitude = RadiansToDegrees(latMax),
                Longitude = RadiansToDegrees(lonMin),
                Altitude = altitude - 500
            };
            var seCorner = new BasicGeoposition
            {
                Latitude = RadiansToDegrees(latMin),
                Longitude = RadiansToDegrees(lonMax),
                Altitude = altitude + 500
            };

            return new GeoboundingBox(nwCorner, seCorner);
        }

        private static double DegreesToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        private static double RadiansToDegrees(double radians)
        {
            return 180.0 * radians / Math.PI;
        }

        private static double EarthRadius(double lat)
        {
            var an = Wgs84A * Wgs84A * Math.Cos(lat);
            var bn = Wgs84B * Wgs84B * Math.Sin(lat);
            var ad = Wgs84A * Math.Cos(lat);
            var bd = Wgs84B * Math.Sin(lat);
            return Math.Sqrt((an * an + bn * bn) / (ad * ad + bd * bd));
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            var radius = EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) radius = EarthRadiusInKilometers;
            return radius * 2 * Math.Asin(Math.Min(1,
                       Math.Sqrt(Math.Pow(Math.Sin(DiffRadian(lat1, lat2) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) *
                                 Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin(DiffRadian(lng1, lng2) / 2.0), 2.0))));
        }

        public static double TryGetDistance(this MapControl map)
        {
            var mapDistance = 0.0;

            try
            {
                map.GetLocationFromOffset(new Point(0, 0), out var northWest);
                map.GetLocationFromOffset(new Point(map.ActualWidth, map.ActualHeight), out var southEast);

                mapDistance = CalcDistance(northWest.Position.Latitude, northWest.Position.Longitude,
                    southEast.Position.Latitude, southEast.Position.Longitude, GeoCodeCalcMeasurement.Miles);

                App.ViewModel.CurrentGridScale = mapDistance; // / 12.0;
            }
            catch (Exception)
            {
            }

            return mapDistance;
        }


        public static List<BasicGeoposition> GetAreaBoundaries()
        {
            var areaBounds = new List<BasicGeoposition>
            {
                new BasicGeoposition {Latitude = 37.917388162022256, Longitude = -117.3781194436157},
                new BasicGeoposition {Latitude = 37.916074924119656, Longitude = -114.27100862192687},
                new BasicGeoposition {Latitude = 36.569446939023756, Longitude = -114.30248033933273},
                new BasicGeoposition {Latitude = 36.550956322695271, Longitude = -117.34996151882672}
            };
            
            return areaBounds;
        }
    }
}