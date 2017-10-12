using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTrafficSim
{
    public class SharedConfigurationInformation
    {
        public string SharedEventHubEndpoint { get; set; }
        public string SharedFlightActivityHubName { get; set; }
        public string SharedAirTrafficHubName { get; set; }        
    }
}
