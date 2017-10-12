using FlySim.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FlySim
{
    public class ActivePlaneInformation : Common.ObservableBase
    {
        private string _displayName;
        public string DisplayName
        {
            get => this._displayName;
            set => this.SetProperty(ref this._displayName, value);
        }

        private Geopoint _location;
        public Geopoint Location
        {
            get => this._location;
            set => this.SetProperty(ref this._location, value);
        }

        private long _lastUpdated;
        public long LastUpdated
        {
            get => this._lastUpdated;
            set => this.SetProperty(ref this._lastUpdated, value);
        }

    }
}
