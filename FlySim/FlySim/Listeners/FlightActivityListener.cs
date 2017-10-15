using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using FlySim.Common;
using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;

namespace FlySim.Listeners
{
    public class FlightActivityListener
    {
        private EventHubClient client { get; set; }
        private EventHubConsumerGroup consumerGroup { get; set; }
        private EventHubReceiver receiver { get; set; }
        private FlightInformation flightInformation { get; set; }
        private ObservableCollection<ActivePlaneInformation> activePlanes { get; set; }

        private bool isInitialized { get; set; }

        public async void StartListeningAsync(FlightInformation flightInformation,
            ObservableCollection<ActivePlaneInformation> activePlanes)
        {
            this.activePlanes = activePlanes;
            this.flightInformation = flightInformation;
             
            var connectionString = $"{CoreConstants.FlightActivityEventHubEndpoint};EntityPath={CoreConstants.FlightActivityEventHubName}";

            client = EventHubClient.CreateFromConnectionString(connectionString);
            
            consumerGroup = client.GetDefaultConsumerGroup();
            receiver = consumerGroup.CreateReceiver("0", DateTime.Now.ToUniversalTime());

            await Task.Run(() => StartListeningForFlightActivityCommands());
        }

        private async void StartListeningForFlightActivityCommands()
        {
            while (true)
            {
                await Task.Delay(1);

                var eventData = receiver.Receive();

                if (eventData != null)
                {
                    var bytes = eventData.GetBytes();

                    var payload = Encoding.UTF8.GetString(bytes);

                    var flightInfo = JsonConvert.DeserializeObject<NewFlightInfo>(payload);

                    UpdateFlightInformation(flightInfo);
                }
            }
        }

        private async void UpdateFlightInformation(NewFlightInfo info)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                flightInformation.Hydrate(info);
                
                activePlanes.Add(new ActivePlaneInformation
                {
                    DisplayName = info.deviceId,
                    Location = new Geopoint(new BasicGeoposition
                    {
                        Latitude = info.latitude,
                        Longitude = info.longitude
                    })
                });

                activePlanes.RemoveAt(0);

                App.ViewModel.SetFlightStatus(info.deviceId);

                if (!isInitialized)
                {
                    isInitialized = true;

                    App.ViewModel.BringPlaneIntoView(info.latitude, info.longitude);
                }
            });
        }
    }
}