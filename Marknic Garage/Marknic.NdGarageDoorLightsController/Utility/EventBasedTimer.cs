using System.Collections;
using System.Threading;
using Microsoft.SPOT;

namespace Marknic.NdGarageDoorLightsController.Utility
{
    public enum TimerExecutionType
    {
        RunOnce,
        Constant
    }

    public delegate void TickEventHandler(EventBasedTimer timer);

    public class EventBasedTimer
    {
        // Timer tick event
        public event TickEventHandler TickEventHandler;

        // Static to hold all of the active timers
        private static readonly ArrayList ActiveTimersList = new ArrayList();

        private readonly Timer _eventTimer;
        
        public TimerExecutionType ExecutionType { get; set; }

        /// <summary>
        /// Timer interval in milliseconds
        /// </summary>
        public int Interval { get; private set; }
        
        /// <summary>
        /// Is the timer ticking?
        /// </summary>
        public bool IsRunning { get; private set; }



        public EventBasedTimer(int interval, TimerExecutionType timerExecutionType, TickEventHandler tickEventHandler)
        {
            Interval = interval;

            TickEventHandler = tickEventHandler;

            ExecutionType = timerExecutionType;

            IsRunning = false;

            // Set up timer but don't start it yet
            _eventTimer = new Timer(TimerTick, null, Timeout.Infinite, interval);
        }

        private void TimerTick(object state)
        {
            try
            {
                if (ExecutionType == TimerExecutionType.RunOnce)
                {
                    Stop();
                }

                if (TickEventHandler != null)
                {
                    TickEventHandler(this);
                }
            }
            catch
            {
                Debug.Print("Timer Tick Failed");
            }
        }


       public void Start()
        {
            if (!ActiveTimersList.Contains(this))
            {
                ActiveTimersList.Add(this);
            }

           IsRunning = true;

            _eventTimer.Change(0, Interval);
        }

        public void Stop()
        {
            if (ActiveTimersList.Contains(this))
            {
                ActiveTimersList.Remove(this);
            }

            IsRunning = false;

            _eventTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

    }
}
