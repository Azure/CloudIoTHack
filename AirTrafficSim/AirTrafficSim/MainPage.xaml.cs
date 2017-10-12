using AirTrafficSim.Helpers;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AirTrafficSim
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string MapServiceToken
        {
            get { return Common.CoreConstants.MapServiceToken; }
        }

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
                toolbar.Visibility = Visibility.Visible;
            }

            var swapchainpanel = flightMap.FindDescendant<SwapChainPanel>();
            if (swapchainpanel != null && swapchainpanel.Parent != null)
            {
                (swapchainpanel.Parent as Grid).AddMapOverlay();
            }

            flightMap.Style = MapStyle.Aerial3DWithRoads;

            await flightMap.TrySetSceneAsync(MapScene.CreateFromLocationAndRadius(Helpers.MapHelper.AreaCenter,
                Common.CoreConstants.AreaRadius));
        }

        private async void OnIntoViewClicked(object sender, RoutedEventArgs e)
        {
            await App.ViewModel.BringPlanesIntoViewAsync(this.flightMap);
        }

        private async void OnViewAreaClicked(object sender, RoutedEventArgs e)
        {
            await App.ViewModel.ViewAreaBoundingBox(this.flightMap);
        }

        private void OnZoomChanged(MapControl sender, object args)
        {
            if (App.ViewModel != null) App.ViewModel.CurrentGridScale = flightMap.TryGetDistance();
        }
        
    }
}
 
