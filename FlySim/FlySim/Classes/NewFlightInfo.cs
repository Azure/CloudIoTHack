using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim
{
    public class NewFlightInfo
    {   
        public string deviceId { get; set; }
        public double airspeed { get; set; }
        public double heading { get; set; }
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double pitch { get; set; }
        public double roll { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
    }


}
