using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AirTrafficSim.Controls
{
    public sealed partial class ActivePlaneControl : UserControl
    {
        public string PilotId
        {
            get { return base.GetValue(PilotIdProperty) as string; }
            set { base.SetValue(PilotIdProperty, value); }
        }

        public static readonly DependencyProperty PilotIdProperty = DependencyProperty.Register("PilotId", typeof(string), typeof(ActivePlaneControl), null);

        public Visibility ZoomDeepLevel
        {
            get { return (Visibility)base.GetValue(ZoomDeepLevelProperty); }
            set { base.SetValue(ZoomDeepLevelProperty, value); }
        }

        public static readonly DependencyProperty ZoomDeepLevelProperty = DependencyProperty.Register("ZoomDeepLevel", typeof(Visibility), typeof(ActivePlaneControl), null);


        public ActivePlaneControl()
        {
            this.InitializeComponent();
            this.Loaded += ActivePlaneControl_Loaded;
        }

        private void ActivePlaneControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.DataContext = App.ViewModel;
        }
    }
}
