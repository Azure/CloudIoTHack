using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using FlySim.Common;
using FlySim.Helpers;
using FlySim.Translation;

namespace FlySim
{
    public class MainViewModel : ObservableBase
    {
        private ObservableCollection<ActivePlaneInformation> _activePlanes =
            new ObservableCollection<ActivePlaneInformation>();

        private ObservableCollection<LanguageInformation> _availableLanguages =
            new ObservableCollection<LanguageInformation>();

        private FlightInformation _currentFlightInformation = new FlightInformation(0);

        private double _currentGridScale;

        private DateTime _currentTime;

        private MapControl _flightMap;

        private double _planeRotation;

        private LanguageInformation _selectedLanguage;

        private FlightStatus _status;
       
        public List<string> AtRiskPlanes = new List<string>();


        public MainViewModel(MapControl map)
        {
            FlightMap = map;

            InitializeClocks();

            InitializePlanes();

            InitializeLanguages();
        }


        public ICommand TryCenterPlaneCommand { get; private set; }

        public TimerHelper CurrentClock { get; set; }

        public double CurrentGridScale
        {
            get => _currentGridScale;
            set => SetProperty(ref _currentGridScale, value);
        }

        public DateTime CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public FlightInformation CurrentFlightInformation
        {
            get => _currentFlightInformation;
            set => SetProperty(ref _currentFlightInformation, value);
        }

        public FlightStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public MapControl FlightMap
        {
            get => _flightMap;
            set => SetProperty(ref _flightMap, value);
        }

        public double PlaneRotation
        {
            get => _planeRotation;
            set => SetProperty(ref _planeRotation, value);
        }

        public ObservableCollection<ActivePlaneInformation> ActivePlanes
        {
            get => _activePlanes;
            set => SetProperty(ref _activePlanes, value);
        }

        public LanguageInformation SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ObservableCollection<LanguageInformation> AvailableLanguages
        {
            get => _availableLanguages;
            set => SetProperty(ref _availableLanguages, value);
        }

        internal async void BringPlaneIntoView(double latitude, double longitude)
        {
            await FlightMap.TrySetViewAsync(new Geopoint(new BasicGeoposition
            {
                Latitude = latitude,
                Longitude = longitude
            }), CoreConstants.InitialZoomLevel);

            this.CurrentGridScale = this.FlightMap.TryGetDistance();
        }

        public void InitializeSystem()
        {
        }

        private void InitializeClocks()
        {
            CurrentClock = new TimerHelper();
            CurrentClock.StartClock();

            CurrentTime = DateTime.Now.ToLocalTime();
        }

        private void InitializePlanes()
        {
            ActivePlanes.Add(new ActivePlaneInformation
            {
                DisplayName = "...",
                Location = new Geopoint(CoreConstants.DefaultStartingLocation)
            });

            TryCenterPlaneCommand = new RelayCommand(async () => { await TryCenterPlaneAsync(); });
        }
        
        public async void InitializeLanguages()
        {
            var languages = await TranslationHelper.GetTextTranslationLanguageNamesAsync();

            AvailableLanguages.Clear();

            foreach (var language in languages)
                AvailableLanguages.Add(language);

            SelectedLanguage = AvailableLanguages.FirstOrDefault(w =>
                w.DisplayName.Equals("English", StringComparison.OrdinalIgnoreCase));
        }

        public void SetFlightStatus(string deviceId)
        {
            var oldStatus = Status;

            Status = AtRiskPlanes.Contains(deviceId, StringComparer.OrdinalIgnoreCase)
                ? FlightStatus.AtRisk
                : FlightStatus.Ok;
            
            if (oldStatus == FlightStatus.Ok && Status == FlightStatus.AtRisk)
                SendWarningMessage();
        }

        private async void SendWarningMessage()
        {
            var message = "Warning";

            if (!SelectedLanguage.DisplayName.Equals("English"))
                message = await TranslationHelper.GetTextTranslationAsync(message, SelectedLanguage.Abbreviation);
        }

        public async Task TryCenterPlaneAsync()
        {
            var activePlane = ActivePlanes.FirstOrDefault();

            if (activePlane != null)
                await FlightMap.TrySetViewAsync(activePlane.Location, 8.0);
        }
    }
}