using FlySim.Helpers;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml.Linq;
using FlySim.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlySim
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string MapServiceToken => Common.CoreConstants.MapServiceToken;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel == null)
            {
                App.ViewModel = new MainViewModel(this.flightMap);

                App.ViewModel.InitializeSystem();
            }

            this.DataContext = App.ViewModel;

            EnsureMapView();
        }

        private async void EnsureMapView()
        {
            await System.Threading.Tasks.Task.Delay(2000);
 
            var toolbar = flightMap.FindDescendant<StackPanel>();
            if (toolbar != null)
            {
                toolbar.Visibility = Visibility.Collapsed;
            }

            var swapchainpanel = flightMap.FindDescendant<SwapChainPanel>();
            (swapchainpanel?.Parent as Grid)?.AddMapOverlay();

            flightMap.Style = MapStyle.Aerial3DWithRoads;
        }
 
        private void OnZoomChanged(MapControl sender, object args)
        {
            if (App.ViewModel != null) App.ViewModel.CurrentGridScale = flightMap.TryGetDistance();
        }
        
    }
}

