using AirTrafficSim.Common;
using AirTrafficSim.Helpers;
using AirTrafficSim.Listeners;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using AirTrafficSim.Controls;
using Windows.UI.Core;

namespace AirTrafficSim
{
    public class MainViewModel : Common.ObservableBase
    {
        public MainViewModel(MapControl map)
        {
            this.FlightMap = map;

            InitializeClocks();

            LoadInitializer();
        }

        private async void LoadInitializer()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.ActivePlanes.Add(new ActivePlaneInformation()
                {
                    DisplayName = $" ",
                    FlightInformation = new FlightInformation(0),
                    Location = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = 0,
                        Longitude = 0,
                    }),
                    Status = Common.FlightStatus.Inactive,
                });
            });
            
        }

        private void InitializeClocks()
        {
            this.CurrentClock = new Helpers.TimerHelper();

            this.CurrentClock.StartLocalTimer();

            this.CurrentTime = DateTime.Now.ToLocalTime();
        }

        public ICommand SetConfigurationCommand { get; private set; }

        private MapControl _flightMap;
        public MapControl FlightMap
        {
            get { return this._flightMap; }
            set { this.SetProperty(ref this._flightMap, value); }
        }

        public void ResetTrafficTimer()
        {
            this.CurrentClock.StopTrafficTimer();
            this.CurrentClock.StartTrafficTimer();
        }

        private string _lastTrafficTimestamp;
        public string LastTrafficTimestamp
        {
            get { return this._lastTrafficTimestamp; }
            set { this.SetProperty(ref this._lastTrafficTimestamp, value); }
        }


        public List<string> AtRiskPlanes = new List<string>();

        private ObservableCollection<FlightAnalysisData> _averageMaxAltitudes = new ObservableCollection<FlightAnalysisData>();
        public ObservableCollection<FlightAnalysisData> AverageMaxAltitudes
        {
            get { return this._averageMaxAltitudes; }
            set { this.SetProperty(ref this._averageMaxAltitudes, value); }
        }

        private ObservableCollection<FlightAnalysisData> _averageMinAltitudes = new ObservableCollection<FlightAnalysisData>();
        public ObservableCollection<FlightAnalysisData> AverageMinAltitudes
        {
            get { return this._averageMinAltitudes; }
            set { this.SetProperty(ref this._averageMinAltitudes, value); }
        }

        public FlightActivityListener FlightActivityListener = new FlightActivityListener();
        public AirTrafficListener AirTrafficListener = new AirTrafficListener();

        public TimerHelper CurrentClock { get; set; }

        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return this._currentTime; }
            set { this.SetProperty(ref this._currentTime, value); }
        }

        private double _currentGridScale;
        public double CurrentGridScale
        {
            get { return this._currentGridScale; }
            set
            {
                this.SetProperty(ref this._currentGridScale, value);

                if (value > 0.0) UpdatePlaneZoomLevel(value);
            }
        }
        
        private int _activePlaneCount;
        public int ActivePlaneCount
        {
            get { return this._activePlaneCount; }
            set { this.SetProperty(ref this._activePlaneCount, value); }
        }

        private int _atRiskPlaneCount;
        public int AtRiskPlaneCount
        {
            get { return this._atRiskPlaneCount; }
            set { this.SetProperty(ref this._atRiskPlaneCount, value); }
        }

        private int _safePlaneCount;
        public int SafePlaneCount
        {
            get { return this._safePlaneCount; }
            set { this.SetProperty(ref this._safePlaneCount, value); }
        }

        private ObservableCollection<ActivePlaneInformation> _activePlanes = new ObservableCollection<ActivePlaneInformation>();
        public ObservableCollection<ActivePlaneInformation> ActivePlanes
        {
            get { return this._activePlanes; }
            set { this.SetProperty(ref this._activePlanes, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return this._isBusy; }
            set { this.SetProperty(ref this._isBusy, value); }
        }

        private void UpdatePlaneZoomLevel(double value)
        {
            foreach (var plane in ActivePlanes)
            {
                plane.ZoomDeepLevel = (value < 90) ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        public async Task<bool> BringPlanesIntoViewAsync(MapControl map)
        {
            var activePlaneLocations = this.ActivePlanes.Where(w => w.Status != Common.FlightStatus.Inactive).Select(s => s.Location).ToList();

            if (activePlaneLocations.Count == 1)
            {
                return await map.TrySetViewAsync(activePlaneLocations[0], 13);
            }
            else if (activePlaneLocations.Count > 1)
            {
                activePlaneLocations.Add(new Geopoint(Common.CoreConstants.DefaultStartingLocation));

                var boundingBox = GeoboundingBox.TryCompute(activePlaneLocations.Select(s => s.Position));

                return await map.TrySetViewBoundsAsync(boundingBox, new Windows.UI.Xaml.Thickness(5), MapAnimationKind.Bow);
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> ViewAreaBoundingBox(MapControl map)
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            if (map.MapElements.Count == 0)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    map.MapElements.Add(new MapPolygon()
                    {
                        StrokeThickness = 2,
                        FillColor = Windows.UI.Color.FromArgb(150, 184, 212, 50),
                        StrokeColor = Windows.UI.Color.FromArgb(200, 255, 255, 255),
                        StrokeDashed = true,
                        Path = new Geopath(Helpers.MapHelper.GetAreaBoundaries()),

                        Visible = true,
                    });
                });
            }
            else
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    map.MapElements.RemoveAt(0);
                    map.MapElements.Clear();
                });

            }

            return true;
        }

        private void Goo(NewFlightInfo info)
        {
            var activePlane = this.ActivePlanes.FirstOrDefault(w => w.DisplayName.Equals(info.deviceId));

            if (activePlane == null)
            {
                this.ActivePlanes.Add(new ActivePlaneInformation()
                {
                    DisplayName = info.deviceId,
                    ZoomDeepLevel = Visibility.Collapsed,
                    Location = new Windows.Devices.Geolocation.Geopoint(
                        new Windows.Devices.Geolocation.BasicGeoposition()
                        {
                            Latitude = info.latitude,
                            Longitude = info.longitude,
                        }),

                    FlightInformation = new FlightInformation(info.altitude)
                    {
                        CurrentAltitude = info.altitude,
                        CurrentHeading = info.heading,
                        CurrentTemperature = info.temperature,
                        CurrentHumidity = info.humidity,
                        CurrentRoll = info.roll,
                        CurrentSpeed = info.airspeed,
                        CurrentPitch = info.pitch,
                        CurrentLatitude = info.latitude,
                        CurrentLongitude = info.longitude,
                    },

                    Status = (App.ViewModel.AtRiskPlanes.Contains(info.deviceId, StringComparer.OrdinalIgnoreCase))
                        ? Common.FlightStatus.AtRisk
                        : Common.FlightStatus.Ok,
                });

            }
            else
            {
                activePlane.DisplayName = info.deviceId;

                activePlane.FlightInformation.CurrentAltitude = info.altitude;
                activePlane.FlightInformation.CurrentHeading = info.heading;
                activePlane.FlightInformation.CurrentTemperature = info.temperature;
                activePlane.FlightInformation.CurrentHumidity = info.humidity;

                activePlane.FlightInformation.CurrentRoll = info.roll;
                activePlane.FlightInformation.CurrentSpeed = info.airspeed;
                activePlane.FlightInformation.CurrentPitch = info.pitch;

                activePlane.FlightInformation.CurrentLatitude = info.latitude;
                activePlane.FlightInformation.CurrentLongitude = info.longitude;

                activePlane.Status =
                    (App.ViewModel.AtRiskPlanes.Contains(info.deviceId, StringComparer.OrdinalIgnoreCase))
                        ? Common.FlightStatus.AtRisk
                        : Common.FlightStatus.Ok;

                var planeControl = this.FlightMap.FindDescendants<ActivePlaneControl>().FirstOrDefault(w =>
                    w.PilotId.Equals(activePlane.DisplayName, StringComparison.OrdinalIgnoreCase));

                if (planeControl != null)
                {
                     

                        MapControl.SetLocation(planeControl, new Geopoint(new BasicGeoposition()
                        {
                            Latitude = info.latitude,
                            Longitude = info.longitude,
                        }));

                    


                }
            }

            this.ActivePlaneCount = this.ActivePlanes.Count(w => w.Status == FlightStatus.Ok);
            this.AtRiskPlaneCount = this.ActivePlanes.Count(w => w.Status == FlightStatus.AtRisk);
            this.SafePlaneCount = this.ActivePlaneCount - this.AtRiskPlaneCount;

            this.AverageMaxAltitudes.Add(new FlightAnalysisData()
            {
                Label = "Max Altitude",
                Value = this.ActivePlanes.Where(w => w.Status != Common.FlightStatus.Inactive).Max(m => Convert.ToInt32(m.FlightInformation.CurrentAltitude))
            });

            this.AverageMinAltitudes.Add(new FlightAnalysisData()
            {
                Label = "Min Altitude",
                Value = this.ActivePlanes.Where(w => w.Status != Common.FlightStatus.Inactive).Min(m => Convert.ToInt32(m.FlightInformation.CurrentAltitude))
            });
        }

        public async void UpdateFlightInformation(List<NewFlightInfo> infos)
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            foreach (var info in infos.Take(100).ToList())
            {
                var activePlane = this.ActivePlanes.FirstOrDefault(w => w.DisplayName.Equals(info.deviceId));

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (activePlane == null)
                    {
                        this.ActivePlanes.Add(new ActivePlaneInformation()
                        {
                            DisplayName = info.deviceId,
                            ZoomDeepLevel = Visibility.Collapsed,
                            Location = new Windows.Devices.Geolocation.Geopoint(
                                new Windows.Devices.Geolocation.BasicGeoposition()
                                {
                                    Latitude = info.latitude,
                                    Longitude = info.longitude,
                                }),

                            FlightInformation = new FlightInformation(info.altitude)
                            {
                                CurrentAltitude = info.altitude,
                                CurrentHeading = info.heading,
                                CurrentTemperature = info.temperature,
                                CurrentHumidity = info.humidity,
                                CurrentRoll = info.roll,
                                CurrentSpeed = info.airspeed,
                                CurrentPitch = info.pitch,
                                CurrentLatitude = info.latitude,
                                CurrentLongitude = info.longitude,
                            },

                            Status = (App.ViewModel.AtRiskPlanes.Contains(info.deviceId,
                                StringComparer.OrdinalIgnoreCase))
                                ? Common.FlightStatus.AtRisk
                                : Common.FlightStatus.Ok,
                        });

                    }
                    else
                    {
                        activePlane.DisplayName = info.deviceId;

                        activePlane.FlightInformation.CurrentAltitude = info.altitude;
                        activePlane.FlightInformation.CurrentHeading = info.heading;
                        activePlane.FlightInformation.CurrentTemperature = info.temperature;
                        activePlane.FlightInformation.CurrentHumidity = info.humidity;

                        activePlane.FlightInformation.CurrentRoll = info.roll;
                        activePlane.FlightInformation.CurrentSpeed = info.airspeed;
                        activePlane.FlightInformation.CurrentPitch = info.pitch;

                        activePlane.FlightInformation.CurrentLatitude = info.latitude;
                        activePlane.FlightInformation.CurrentLongitude = info.longitude;

                        activePlane.Status =
                            (App.ViewModel.AtRiskPlanes.Contains(info.deviceId, StringComparer.OrdinalIgnoreCase))
                                ? Common.FlightStatus.AtRisk
                                : Common.FlightStatus.Ok;

                        var planeControl = this.FlightMap.FindDescendants<ActivePlaneControl>().FirstOrDefault(w =>
                            w.PilotId.Equals(activePlane.DisplayName, StringComparison.OrdinalIgnoreCase));

                        if (planeControl != null)
                        {   
                            MapControl.SetLocation(planeControl, new Geopoint(new BasicGeoposition()
                            {
                                Latitude = info.latitude,
                                Longitude = info.longitude,
                            }));

                        }
                    }
                });
            }


            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.ActivePlaneCount = this.ActivePlanes.Count(w => w.Status == FlightStatus.Ok);
                this.AtRiskPlaneCount = this.ActivePlanes.Count(w => w.Status == FlightStatus.AtRisk);
                this.SafePlaneCount = this.ActivePlaneCount - this.AtRiskPlaneCount;

                this.AverageMaxAltitudes.Add(new FlightAnalysisData()
                {
                    Label = "Max Altitude",
                    Value = this.ActivePlanes.Where(w => w.Status != Common.FlightStatus.Inactive).Max(m => Convert.ToInt32(m.FlightInformation.CurrentAltitude))
                });

                this.AverageMinAltitudes.Add(new FlightAnalysisData()
                {
                    Label = "Min Altitude",
                    Value = this.ActivePlanes.Where(w => w.Status != Common.FlightStatus.Inactive).Min(m => Convert.ToInt32(m.FlightInformation.CurrentAltitude))
                });

            });

                
            FlightActivityListener.CurrentDelay = 1;
        }
        
        public void UpdateAllActivePlaneStatus()
        {
            this.ActivePlanes.Where(w => w.Status != FlightStatus.Inactive).ForEach(f => f.Status = (App.ViewModel.AtRiskPlanes.Contains(f.DisplayName, StringComparer.OrdinalIgnoreCase)) ? Common.FlightStatus.AtRisk : Common.FlightStatus.Ok);
        }

        public void InitializeSystem()
        {
            if (this.FlightActivityListener.IsConfigured) this.FlightActivityListener.StartListeningAsync();
            if (this.AirTrafficListener.IsConfigured) this.AirTrafficListener.StartListeningAsync();
        }
    }
}
