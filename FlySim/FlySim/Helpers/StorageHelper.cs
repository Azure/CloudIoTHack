using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim.Helpers
{
    public static class StorageHelper
    {
        public static List<string> GetRetrievedMarkerActivity()
        {
            List<string> retrievedMarkers = new List<string>();

            try
            {
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                var composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["RetrievedMarkerActivity"];

                if (composite != null)
                {
                    retrievedMarkers = ((string)composite["RetrievedMarkers"]).Split(',').ToList();
                }

            }
            catch (Exception)
            {
            }

            return retrievedMarkers;
        }

        public static bool SaveRetrievedMarkerActivity(IEnumerable<string> retrievedMarkers)
        {
            bool successful = false;

            try
            {
                Windows.Storage.ApplicationDataCompositeValue composite =
                    new Windows.Storage.ApplicationDataCompositeValue
                    {
                        ["RetrievedMarkers"] = string.Join(",", retrievedMarkers)
                    };


                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["RetrievedMarkerActivity"] = composite;

                successful = true;
            }
            catch (Exception)
            {
            }

            return successful;
        }
    }
}
