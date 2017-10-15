using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim
{
    public class PlaneStatusInformation : Common.ObservableBase
    {
        private string _plane1;
        public string Plane1
        {
            get => this._plane1;
            set => this.SetProperty(ref this._plane1, value);
        }

        private string _plane2;
        public string Plane2
        {
            get => this._plane2;
            set => this.SetProperty(ref this._plane2, value);
        }

        private double _distance;
        public double Distance
        {
            get => this._distance;
            set => this.SetProperty(ref this._distance, value);
        }
    }
}
