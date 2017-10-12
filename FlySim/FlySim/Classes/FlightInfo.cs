using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim
{
    public class FlightInfo
    {
        public string deviceId { get; set; }
        public int messageId { get; set; }
        public string timestamp { get; set; }
        public float temperature { get; set; }
        public float humidity { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
