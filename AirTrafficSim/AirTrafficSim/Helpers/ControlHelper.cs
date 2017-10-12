using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AirTrafficSim.Helpers
{
    public static class ControlHelper
    {
        public static Grid GenerateMapGrid()
        {
            Grid grid = new Grid();

            for (int i = 0; i <= 11; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(8.0, GridUnitType.Star) });
            }

            for (int i = 0; i <= 11; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8.0, GridUnitType.Star) });
            }

            Border border = null;

            for (int row = 0; row <= 11; row++)
            {
               
                for (int col = 0; col <= 11; col++)
                {
                    border = new Border() { BorderBrush = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(1.0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };


                    border.SetValue(Grid.RowProperty, row);
                    border.SetValue(Grid.ColumnProperty, col);

                    grid.Children.Add(border);
                }

               
            }

            return grid;
        }
    }
}
