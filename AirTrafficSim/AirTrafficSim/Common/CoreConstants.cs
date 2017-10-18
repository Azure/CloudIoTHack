using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTrafficSim.Common
{
    public class CoreConstants
    {   
        public const string SharedEventHubEndpoint = "SHARED_EVENT_HUB_ENDPOINT";
        public const string SharedFlightActivityHubName = "flysim-shared-input-hub";
        public const string SharedAirTrafficHubName = "flysim-shared-output-hub";
        
        public static string MapServiceToken = "HvD1WpB9MyrWv30BaZCP~-DZBRQGBV2rbPRfQqjBZUQ~AkD5MLW_Sj2_nCY3sesLC3Ldua1j84cVqhnL2Pl5MpY1lbLLnVrmmHoqAVjjvbhO";
        public const double AreaRadius = 80467.0;
        public const double AtRiskThreshold = 10560.0;

        public static Windows.Devices.Geolocation.BasicGeoposition DefaultStartingLocation = new Windows.Devices.Geolocation.BasicGeoposition()
        {
            Latitude = 37.242,
            Longitude = -115.8191,
        };
    }
}

 