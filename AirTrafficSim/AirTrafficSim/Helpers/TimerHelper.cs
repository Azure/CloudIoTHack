using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTrafficSim.Helpers
{
    public class TimerHelper
    {
        IDisposable TrafficTimer = null;
        IDisposable ClockTimer = null;

        public void StartTrafficTimer()
        {
            TrafficTimer = Observable
             .Timer(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
             .Subscribe(q =>
             {
                 UpdateTrafficTimer();
             });
        }

        private async void UpdateTrafficTimer()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                App.ViewModel.LastTrafficTimestamp = DateTime.Now.ToString("hh:mm:ss");
            });
        }

        public void StopTrafficTimer()
        {
            if (this.TrafficTimer != null) this.TrafficTimer.Dispose();
        }

        public void StartLocalTimer()
        {
            ClockTimer = Observable
             .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
             .Subscribe(q =>
             {
                 UpdateClock();
             });
        }

        private async void UpdateClock()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                App.ViewModel.CurrentTime = DateTime.Now;
                
            });

        }

       
    }

}
