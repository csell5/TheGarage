using System;
using Marknic.NdGarageDoorLightsController.Utility;

namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public class Status : IStatus
    {
        private long _lastTickCount;

        public Status(long currentTickCount)
            : this("initialized", currentTickCount)
        {
        }

        public Status(string state, long currentTickCount)
        {
            State = state;
            _lastTickCount = currentTickCount;
            Duration = new TimeSpan(0);
        }

        public string State { get; private set; }

        public TimeSpan Duration { get; private set; }

        public TimeSpan UpdateState(string newState, long currentTickCount)
        {
            if (newState == null) return new TimeSpan(0);

            if (newState != State)
            {
                Duration = new TimeSpan(0);
                State = newState;
            }
            else
            {
                var tickDifference = Math.Abs(currentTickCount - _lastTickCount);
                Duration = Duration.Add(new TimeSpan((long)tickDifference));
            }

            _lastTickCount = currentTickCount;

            return Duration;
        }

        public string SerializeJson()
        {
            return "{\"State\": \"" + State + "\", \"Duration\": " + TimeUtility.ConvertTimeSpanToSeconds(Duration) + " }";
        }
    }
}