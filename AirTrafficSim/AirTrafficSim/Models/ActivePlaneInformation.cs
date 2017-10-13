using AirTrafficSim.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace AirTrafficSim
{
    public class ActivePlaneInformation : Common.ObservableBase
    {
        private string _displayName;
        public string DisplayName
        {
            get { return this._displayName; }
            set { this.SetProperty(ref this._displayName, value); }
        }

        private Geopoint _location;
        public Geopoint Location
        {
            get { return this._location; }
            set { this.SetProperty(ref this._location, value); }
        }

        private FlightInformation _flightInformation;
        public FlightInformation FlightInformation
        {
            get { return this._flightInformation; }
            set { this.SetProperty(ref this._flightInformation, value); }
        }

        private FlightStatus _status;
        public FlightStatus Status
        {
            get { return this._status; }
            set { this.SetProperty(ref this._status, value); }
        }
        
        private Visibility _zoomDeepLevel;
        public Visibility ZoomDeepLevel
        {
            get { return this._zoomDeepLevel; }
            set { this.SetProperty(ref this._zoomDeepLevel, value); }
        }
    }
}
