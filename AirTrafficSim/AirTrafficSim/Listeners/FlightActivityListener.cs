using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Collections.ObjectModel;

namespace AirTrafficSim.Listeners
{
    public class FlightActivityListener
    {
        private EventHubClient client { get; set; }
        private EventHubConsumerGroup consumerGroup { get; set; }
        private EventHubReceiver primaryReceiver { get; set; }
        private EventHubReceiver secondaryReceiver { get; set; }

        public List<NewFlightInfo> PendingFlightInfo = new List<NewFlightInfo>();

        public int CurrentDelay = 1;

        internal bool IsConfigured => Common.CoreConstants.SharedEventHubEndpoint.ToLower().Contains("endpoint=sb://");

        public async void StartListeningAsync()
        {
            this.client = EventHubClient.CreateFromConnectionString(Common.CoreConstants.SharedEventHubEndpoint, Common.CoreConstants.SharedFlightActivityHubName);

            this.consumerGroup = this.client.GetDefaultConsumerGroup();
            this.primaryReceiver = this.consumerGroup.CreateReceiver("0", DateTime.Now.ToUniversalTime());
            this.secondaryReceiver = this.consumerGroup.CreateReceiver("1", DateTime.Now.ToUniversalTime());

            await Task.Run(() => StartListeningForNavigationCommands());
        }

        private async void StartListeningForNavigationCommands()
        {
            while (true)
            {
                try
                {
                    var primaryEventData = this.primaryReceiver.Receive();

                    if (primaryEventData != null)
                    {
                        byte[] bytes = primaryEventData.GetBytes();

                        var payload = Encoding.UTF8.GetString(bytes);

                        try
                        {
                            var flightInfo = JsonConvert.DeserializeObject<NewFlightInfo>(payload);

                            this.PendingFlightInfo.Add(flightInfo);

                            if (this.PendingFlightInfo.Count >= App.ViewModel.ActivePlaneCount)
                            {
                                CurrentDelay = 1000;

                                App.ViewModel.UpdateFlightInformation(this.PendingFlightInfo);

                                this.PendingFlightInfo.Clear();
                            }
                            
                        }
                        catch
                        {
                        }

                    }

                    var secondaryEventData = this.secondaryReceiver.Receive();

                    if (secondaryEventData != null)
                    {
                        byte[] bytes = secondaryEventData.GetBytes();

                        var payload = Encoding.UTF8.GetString(bytes);

                        try
                        {
                            var flightInfo = JsonConvert.DeserializeObject<NewFlightInfo>(payload);

                            this.PendingFlightInfo.Add(flightInfo);

                            if (this.PendingFlightInfo.Count >= App.ViewModel.ActivePlaneCount)
                            {
                                CurrentDelay = 1000;

                                App.ViewModel.UpdateFlightInformation(this.PendingFlightInfo);

                                this.PendingFlightInfo.Clear();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                    
                }

                await Task.Delay(CurrentDelay);

            }

        }

    }
}
