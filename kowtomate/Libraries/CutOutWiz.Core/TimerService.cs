using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CutOutWiz.Core
{
    public class TimerService
    {
        public const string Default_Time = "00:00:00";
        public string elapsedTime = Default_Time;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        //bool isRunning = false;
        public async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}";
            //StateHasChanged();
        }
        public async void StartTimer()
        {
            startTime = DateTime.Now;
            timer = new System.Timers.Timer(1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            //isRunning = true;
        }
        public async void StopTimer()
        {
            //isRunning = false;
            timer.Enabled = false;
            elapsedTime = Default_Time;
        }

        public TimeSpan stopWatchValue = new TimeSpan();
        public bool isStopWatchRunning = false;
        
        public async Task StopWatch()
        {
            isStopWatchRunning=true;
            while (isStopWatchRunning)
            { 
                await Task.Delay(1000);
                if (isStopWatchRunning)
                {
                    stopWatchValue= stopWatchValue.Add(new TimeSpan(0,0,1));
                }
            }
        }

    }
}
