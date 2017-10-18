using FlySim.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FlySim
{
    public static class DirectionExtensions
    {
        private static readonly Dictionary<CompassDirection, string>
            mapping = new Dictionary<CompassDirection, string>
        {
        { CompassDirection.North, "N" },
        { CompassDirection.NorthEast, "NE" },
        { CompassDirection.East, "E" },
        { CompassDirection.SouthEast, "SE" },
        { CompassDirection.South, "S" },
        { CompassDirection.SouthWest, "SW" },
        { CompassDirection.West, "W" },
        { CompassDirection.NorthWest, "NW" }
        };

        public static CompassDirection AsDirectionLabel(this double heading)
        {
            var adjusted = (heading + 22) % 360;
            var sector = adjusted / 45;
            return (CompassDirection)sector;
        }

        public static string GetSuffix(this CompassDirection direction)
        {
            return mapping.ContainsKey(direction) ? mapping[direction] : "N";
        }

        public static Boolean IsWithin(this BasicGeoposition pt, BasicGeoposition nw, BasicGeoposition se)
        {
            return pt.Latitude <= nw.Latitude &&
                pt.Longitude >= nw.Longitude &&
                   pt.Latitude >= se.Latitude &&
                   pt.Longitude <= se.Longitude;
        }
    }
}
