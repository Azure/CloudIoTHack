using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace AirTrafficSim
{
    public static class ControlExtensions
    {
        public static void AddMapOverlay(this Grid grid)
        {
            var border = new Border()
            {
                IsHitTestVisible = false,
                Background = App.Current.Resources["AppAccentBrush"] as SolidColorBrush,
                Opacity = 0.4
            };

            grid.Children.Add(Helpers.ControlHelper.GenerateMapGrid());
            grid.Children.Add(border);

            TextBlock scaleLabel = new TextBlock()
            {
                Margin = new Windows.UI.Xaml.Thickness(10, 5, 10, 7),
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
            };

            scaleLabel.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Path = new Windows.UI.Xaml.PropertyPath("CurrentGridScale"),
                Source = App.ViewModel,
                Converter = new Common.MilesDisplayLabelConverter(),
            });

            Grid scaleGrid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                Margin = new Windows.UI.Xaml.Thickness(10),
                RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark,
            };

            scaleGrid.Children.Add(scaleLabel);

            grid.Children.Add(scaleGrid);
        }
    }
}
