using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySim.Helpers
{
    public class TimerHelper
    {
        //IDisposable MissionTimer = null;
        IDisposable CurrentTimer = null;

        //public void StartMissionTimer()
        //{
        //    MissionTimer = Observable
        //     .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        //     .Subscribe(q =>
        //     {
        //         UpdateTimer();
        //     });
        //}

        public void StartClock()
        {
            CurrentTimer = Observable
             .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
             .Subscribe(q =>
             {
                 UpdateClock();
             });
        }

        private async void UpdateClock()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                App.ViewModel.CurrentTime = DateTime.Now;
                
            });

        }

        //private async void UpdateTimer()
        //{
        //    var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

        //    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //    {
        //        App.ViewModel.CurrentTime = DateTime.Now;
        //    });

        //}
    }

}
