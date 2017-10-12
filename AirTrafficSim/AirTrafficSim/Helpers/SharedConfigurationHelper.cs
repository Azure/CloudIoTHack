using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace AirTrafficSim.Helpers
{
    public static class SharedConfigurationHelper
    {
        public async static Task<bool> SetSharedConfigurationAsync()
        {
            HttpClient client = new HttpClient();

            SharedConfigurationInformation sharedConfiguration = new SharedConfigurationInformation()
            {
                SharedEventHubEndpoint = Common.CoreConstants.SharedEventHubEndpoint,
                SharedFlightActivityHubName = Common.CoreConstants.SharedFlightActivityHubName,
                SharedAirTrafficHubName = Common.CoreConstants.SharedAirTrafficHubName,
            };

            var payload = new HttpStringContent(JsonConvert.SerializeObject(sharedConfiguration), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
            var result = await client.PostAsync(new Uri(Common.CoreConstants.EndpointConfigurationServiceUrl), payload);

            return result.IsSuccessStatusCode;

        }
    }
}
