using AirTrafficSim.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTrafficSim
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
            return mapping[direction];
        }
    }
}
