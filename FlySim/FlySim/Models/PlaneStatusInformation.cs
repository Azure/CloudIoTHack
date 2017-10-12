using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim
{
    public class PlaneStatusInformation : Common.ObservableBase
    {
        private string _displayName;
        public string DisplayName
        {
            get => this._displayName;
            set => this.SetProperty(ref this._displayName, value);
        }

        private double _distance;
        public double Distance
        {
            get => this._distance;
            set => this.SetProperty(ref this._distance, value);
        }

        private DateTime _eventTime;
        public DateTime EventTime
        {
            get => this._eventTime;
            set => this.SetProperty(ref this._eventTime, value);
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => this._timestamp;
            set => this.SetProperty(ref this._timestamp, value);
        }
    }
}
