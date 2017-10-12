using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace FlySim.Helpers
{
    public static class ThemeHelper
    {
        public static void ApplyTitleSyling()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null){

                    var brandColor = (App.Current.Resources["AppAccentBrush"] as SolidColorBrush).Color;
                    var brandColorLight = (App.Current.Resources["AppAccentBrushLight"] as SolidColorBrush).Color;

                    titleBar.ButtonBackgroundColor = brandColor;
                    titleBar.ButtonInactiveBackgroundColor = brandColorLight;

                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonInactiveForegroundColor = Colors.Black;

                    titleBar.BackgroundColor = brandColor;
                    titleBar.InactiveBackgroundColor = (App.Current.Resources["AppAccentBrushLight"] as SolidColorBrush).Color;
                }

            }
        }
    }
}
