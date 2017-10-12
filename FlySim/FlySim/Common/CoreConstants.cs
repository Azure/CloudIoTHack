using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim.Common
{
    public class CoreConstants
    {
        public static string FlightActivityEventHubEndpoint = "EVENT_HUB_ENDPOINT";
        public static string FlightActivityEventHubName = "flysim";

        public static double AreaRadius = 80467.0;
        public static double InitialZoomLevel = 9.0;
        public static double AtRiskThreshold = 10560.0;
        
        public static Windows.Devices.Geolocation.BasicGeoposition DefaultStartingLocation = new Windows.Devices.Geolocation.BasicGeoposition()
        {            
            Latitude = 37.242,
            Longitude = -115.8191,
        };

        public static string MapServiceToken = "HvD1WpB9MyrWv30BaZCP~-DZBRQGBV2rbPRfQqjBZUQ~AkD5MLW_Sj2_nCY3sesLC3Ldua1j84cVqhnL2Pl5MpY1lbLLnVrmmHoqAVjjvbhO";
      
        public static string TranslatorTextSubscriptionKey = "797b91afb8e9402cb16f3dc3eb21691e";
        public static string TranslatorServicesBaseUrl = $"http://api.microsofttranslator.com/";

    }
}
