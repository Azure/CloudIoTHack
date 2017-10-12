 
using FlySim.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim.Translation
{
    public class LanguageInformation : ObservableBase
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { this.SetProperty(ref _displayName, value); }
        }

        private string _abbreviation;
        public string Abbreviation
        {
            get { return _abbreviation; }
            set { this.SetProperty(ref _abbreviation, value); }
        }
 
    }
}
