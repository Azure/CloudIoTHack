using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace FlySim.Listeners
{
    public class AirTrafficListener
    {
        private EventHubClient client { get; set; }
        private EventHubConsumerGroup consumerGroup { get; set; }
        private EventHubReceiver receiver { get; set; }
        private FlightInformation flightInformation { get; set; }
        private ObservableCollection<ActivePlaneInformation> activePlanes { get; set; }

        public async void StartListeningAsync(FlightInformation flightInformation, ObservableCollection<ActivePlaneInformation> activePlanes)
        {
            this.activePlanes = activePlanes;
            this.flightInformation = flightInformation;
            this.client = EventHubClient.CreateFromConnectionString(Common.CoreConstants.SharedAirTrafficEventHubEndpoint, Common.CoreConstants.SharedAirTrafficHubName);

            this.consumerGroup = this.client.GetDefaultConsumerGroup();
            this.receiver = this.consumerGroup.CreateReceiver("0", DateTime.Now.ToUniversalTime());

            await System.Threading.Tasks.Task.Run(() => StartListeningForTrafficCommands());
        }

        private async void StartListeningForTrafficCommands()
        {
            List<PlaneStatusInformation> statusInfo = new List<PlaneStatusInformation>();

            while (true)
            {
                await Task.Delay(1);

                try
                {

                    var eventData = this.receiver.Receive();

                    if (eventData != null)
                    {
                        byte[] bytes = eventData.GetBytes();

                        var payload = Encoding.UTF8.GetString(bytes);

                        statusInfo.Clear();

                        try
                        {
                            foreach (var info in payload.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {
                                var status = JsonConvert.DeserializeObject<PlaneStatusInfo>(info);

                                statusInfo.Add(new PlaneStatusInformation()
                                {
                                    DisplayName = status.deviceId,
                                    Distance = Convert.ToDouble(status.distance),
                                    Timestamp = DateTime.ParseExact(status.endtime, @"yyyy-MM-dd\THH:mm:ss.fffffff\Z", System.Globalization.CultureInfo.InvariantCulture),
                                    EventTime = DateTime.ParseExact(status.eventtime, @"yyyy-MM-dd\THH:mm:ss.fffffff\Z", System.Globalization.CultureInfo.InvariantCulture),

                                });
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    var atRiskPlanes = (from info in statusInfo
                                        where info.Distance < Common.CoreConstants.AtRiskThreshold
                                        select info.DisplayName);

                    App.ViewModel.AtRiskPlanes = atRiskPlanes.Distinct().ToList();
                }
                catch { }

            }
        }
    }
}
 
