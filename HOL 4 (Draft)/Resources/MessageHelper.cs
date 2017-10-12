using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim.Helpers
{
    public static  class MessageHelper
    {
        public async static Task<bool> SendMessageToDeviceAsync(string message)
        {
            bool successful = false;

            try
            {
                var serviceClient = ServiceClient.CreateFromConnectionString(Common.CoreConstants.DeviceMessagingConnectionString);

                var commandMessage = new Message(Encoding.ASCII.GetBytes(message.ToUpper()));
                await serviceClient.SendAsync("AZ3166", commandMessage);

                successful = true;
            }
            catch { }

            return successful;
        }
    }
}
